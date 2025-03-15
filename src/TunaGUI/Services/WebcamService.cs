using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using OpenCvSharp;

namespace TunaGUI.Services;

public class WebcamService : IWebcamService, IDisposable
{
    private VideoCapture? _capture;
    private CancellationTokenSource? _captureTokenSource;
    private bool _isRunning;
    private int _deviceId = 0;
    private string _lastError = string.Empty;
    private DateTime _lastFrameTime = DateTime.Now;
    private double _currentFps = 0;
    
    // Simplified to use just the most common APIs
    private readonly VideoCaptureAPIs[] _apisToTry = { 
        VideoCaptureAPIs.DSHOW,  // First try DirectShow (Windows)
        VideoCaptureAPIs.ANY     // Then try auto-detection
    };

    public bool IsRunning => _isRunning;
    public string LastError => _lastError;
    public double CurrentFps => _currentFps;
    
    public int DeviceId 
    { 
        get => _deviceId;
        set 
        {
            if (_deviceId != value)
            {
                _deviceId = value;
                if (_isRunning)
                {
                    _ = ReinitializeAsync();
                }
            }
        }
    }

    // Get list of available cameras
    public async Task<List<string>> GetAvailableCamerasAsync()
    {
        // Run camera detection on a background thread to avoid blocking UI
        return await Task.Run(() =>
        {
            var cameras = new List<string>();
            
            try
            {
                // Try to find cameras
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        using var tempCapture = new VideoCapture(i);
                        if (tempCapture.IsOpened())
                        {
                            cameras.Add($"Camera {i}");
                            tempCapture.Release();
                        }
                    }
                    catch { /* Skip failed cameras */ }
                }
            }
            catch (Exception ex)
            {
                _lastError = $"Error finding cameras: {ex.Message}";
            }
            
            if (cameras.Count == 0)
            {
                cameras.Add("No cameras found");
            }
            
            return cameras;
        });
    }

    private async Task<bool> ReinitializeAsync()
    {
        StopCapture();
        return await InitializeAsync();
    }

    public async Task<bool> InitializeAsync()
    {
        StopCapture();
        
        foreach (var api in _apisToTry)
        {
            try
            {
                _lastError = string.Empty;
                
                // Clean up previous capture if any
                _capture?.Dispose();
                _capture = null;
                
                // Open camera with current API
                _capture = new VideoCapture(_deviceId, api);
                
                if (_capture == null || !_capture.IsOpened())
                {
                    _lastError = $"Failed to open camera {_deviceId}";
                    continue; // Try next API
                }
                
                // Configure camera resolution
                try
                {
                    _capture.Set(VideoCaptureProperties.FrameWidth, 640);
                    _capture.Set(VideoCaptureProperties.FrameHeight, 480);
                }
                catch { /* Ignore if camera doesn't support these settings */ }
                
                // Test capture
                using var testFrame = new Mat();
                if (!_capture.Read(testFrame) || testFrame.Empty())
                {
                    _lastError = "Camera opened but could not read frames";
                    continue; // Try next API
                }
                
                return true; // Successfully initialized
            }
            catch (Exception ex)
            {
                _lastError = $"Camera error: {ex.Message}";
            }
        }
        
        return false; // Failed to initialize with any API
    }

    public async Task<Bitmap?> GetFrameAsync()
    {
        if (_capture == null || !_capture.IsOpened())
        {
            return null;
        }

        try
        {
            // Perform frame capture on a background thread
            return await Task.Run(() =>
            {
                using var mat = new Mat();
                if (!_capture.Read(mat) || mat.Empty())
                {
                    return null;
                }

                // Calculate FPS
                var now = DateTime.Now;
                _currentFps = 1000.0 / Math.Max(1, (now - _lastFrameTime).TotalMilliseconds);
                _lastFrameTime = now;

                // Convert to bitmap
                return ConvertMatToBitmap(mat);
            });
        }
        catch (Exception ex)
        {
            _lastError = ex.Message;
            return null;
        }
    }

    public async Task StartCaptureAsync(Action<Bitmap> frameCallback, CancellationToken cancellationToken)
    {
        if (_capture == null || !_capture.IsOpened())
        {
            bool initialized = await InitializeAsync();
            if (!initialized)
            {
                return;
            }
        }

        _isRunning = true;
        _captureTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        await Task.Run(async () =>
        {
            while (!_captureTokenSource.Token.IsCancellationRequested && _isRunning)
            {
                try
                {
                    var bitmap = await GetFrameAsync();
                    if (bitmap != null)
                    {
                        frameCallback(bitmap);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Ignore frame errors
                }
                
                await Task.Delay(16, _captureTokenSource.Token); // ~60fps target
            }
        }, _captureTokenSource.Token);
    }

    public void StopCapture()
    {
        _isRunning = false;
        
        try
        {
            _captureTokenSource?.Cancel();
        }
        catch { /* Ignore cancellation errors */ }
    }

    private Bitmap ConvertMatToBitmap(Mat mat)
    {
        using var memoryStream = new MemoryStream();
        mat.WriteToStream(memoryStream);
        memoryStream.Position = 0;
        return new Bitmap(memoryStream);
    }

    public void Dispose()
    {
        StopCapture();
        _captureTokenSource?.Dispose();
        _capture?.Dispose();
        _capture = null;
    }
    
    public string GetCameraStatus()
    {
        if (!_isRunning)
            return "Camera stopped";
        
        if (_capture == null || !_capture.IsOpened())
            return "Camera disconnected";
            
        return "Camera running";
    }
}
