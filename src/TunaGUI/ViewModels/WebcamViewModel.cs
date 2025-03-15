using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TunaGUI.Services;

namespace TunaGUI.ViewModels
{
    public partial class WebcamViewModel : ViewModelBase, IDisposable
    {
        private readonly IWebcamService _webcamService;
        private CancellationTokenSource? _cancellationTokenSource;
        private DispatcherTimer _fpsUpdateTimer;
        private bool _isInitialized = false;

        [ObservableProperty]
        private Bitmap? _currentFrame;

        [ObservableProperty]
        private bool _isWebcamActive;

        [ObservableProperty]
        private string _cameraStatus = "Camera not initialized";

        [ObservableProperty]
        private double _fps;
        
        [ObservableProperty]
        private ObservableCollection<string> _availableCameras = new();
        
        [ObservableProperty]
        private int _selectedCameraIndex = 0;

        public WebcamViewModel(IWebcamService webcamService)
        {
            _webcamService = webcamService;
            
            // Set up FPS update timer
            _fpsUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            _fpsUpdateTimer.Tick += (s, e) =>
            {
                if (_isWebcamActive)
                {
                    Fps = Math.Round(_webcamService.CurrentFps, 1);
                    CameraStatus = _webcamService.GetCameraStatus();
                    
                    // Sync UI with actual camera state
                    if (!_webcamService.IsRunning && IsWebcamActive)
                    {
                        IsWebcamActive = false;
                    }
                }
            };
            _fpsUpdateTimer.Start();
            
            // Initialize camera on startup
            if (!_isInitialized)
            {
                _ = InitializeAsync();
                _isInitialized = true;
            }
        }
        
        private async Task InitializeAsync()
        {
            await LoadCamerasAsync();
            await StartWebcamAsync();
        }
        
        private async Task LoadCamerasAsync()
        {
            try
            {
                CameraStatus = "Searching for cameras...";
                var cameras = await _webcamService.GetAvailableCamerasAsync();
                
                AvailableCameras.Clear();
                foreach (var camera in cameras)
                {
                    AvailableCameras.Add(camera);
                }
                
                if (AvailableCameras.Count > 0)
                {
                    CameraStatus = $"Found {AvailableCameras.Count} camera(s)";
                    await InitializeWebcamAsync();
                }
                else
                {
                    CameraStatus = "No cameras found";
                }
            }
            catch (Exception ex)
            {
                CameraStatus = $"Error: {ex.Message}";
            }
        }

        partial void OnSelectedCameraIndexChanged(int value)
        {
            if (value >= 0 && value < AvailableCameras.Count)
            {
                _webcamService.DeviceId = value;
                _ = InitializeWebcamAsync();
            }
        }

        [RelayCommand]
        public async Task InitializeWebcamAsync()
        {
            CameraStatus = "Initializing camera...";
            bool initialized = await _webcamService.InitializeAsync();
            CameraStatus = initialized ? "Camera ready" : "Failed to initialize camera";
        }

        [RelayCommand]
        private async Task StartWebcamAsync()
        {
            if (IsWebcamActive) return;

            try
            {
                CameraStatus = "Starting camera...";
                
                // Clean up previous token
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }
                
                _cancellationTokenSource = new CancellationTokenSource();
                
                await _webcamService.StartCaptureAsync(frame =>
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        CurrentFrame = frame;
                        IsWebcamActive = true;
                        CameraStatus = _webcamService.GetCameraStatus();
                    });
                }, _cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                IsWebcamActive = false;
                CameraStatus = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        private Task StopWebcamAsync()
        {
            if (!IsWebcamActive) return Task.CompletedTask;

            try
            {
                CameraStatus = "Stopping camera...";
                _webcamService.StopCapture();
                IsWebcamActive = false;
                CameraStatus = "Camera stopped";
            }
            catch (Exception ex)
            {
                CameraStatus = $"Error stopping: {ex.Message}";
            }
            
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task ToggleWebcamAsync()
        {
            if (IsWebcamActive)
                await StopWebcamAsync();
            else
                await StartWebcamAsync();
        }

        public async Task RestartIfNeeded()
        {
            if (_webcamService.IsRunning != IsWebcamActive)
            {
                if (_webcamService.IsRunning)
                {
                    IsWebcamActive = true;
                }
                else if (IsWebcamActive)
                {
                    await StartWebcamAsync();
                }
            }
        }

        public void Dispose()
        {
            _fpsUpdateTimer?.Stop();
            
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
            
            if (_webcamService?.IsRunning == true)
            {
                _webcamService.StopCapture();
            }
        }
    }
}
