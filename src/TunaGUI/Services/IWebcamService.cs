using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace TunaGUI.Services;

public interface IWebcamService
{
    Task<bool> InitializeAsync();
    Task<Bitmap?> GetFrameAsync();
    Task StartCaptureAsync(Action<Bitmap> frameCallback, CancellationToken cancellationToken);
    void StopCapture();
    bool IsRunning { get; }
    string LastError { get; }
    double CurrentFps { get; }
    int DeviceId { get; set; }
    Task<List<string>> GetAvailableCamerasAsync();
    string GetCameraStatus();
}
