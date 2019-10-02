using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASCOM.Sony
{
    public interface ISonyCamera
    {
        void StartExposure(short iso, double durationSeconds, bool isLightFrame);
        void AbortExposure();
        void StopExposure();
        void Connect();
    }

    public class SonyCamera:ISonyCamera
    {
        private readonly ImagingEdgeRemoteInterop _remoteApp;

        private CameraModel _cameraModel;

        public event EventHandler<ExposureReadyEventArgs> ExposureReady;
        public event EventHandler<ExposureCompletedEventArgs> ExposureCompleted;
        public event EventHandler<ExposureFailedEventArgs> ExposureFailed;

        public SonyCamera(CameraModel cameraModel, ImageFormat imageFormat, bool autoDeleteImageFile)
        {
            _cameraModel = cameraModel;
            _remoteApp = new ImagingEdgeRemoteInterop(cameraModel, imageFormat, autoDeleteImageFile);

            _remoteApp.ExposureCompleted += _remoteApp_ExposureCompleted;
            _remoteApp.ExposureReady += _remoteApp_ExposureReady;
            _remoteApp.ExposureFailed += _remoteApp_ExposureFailed;
        }

        private void _remoteApp_ExposureFailed(object sender, ExposureFailedEventArgs e)
        {
            ExposureFailed?.Invoke(this,e);
        }

        private void _remoteApp_ExposureReady(object sender, ExposureReadyEventArgs e)
        {
            ExposureReady?.Invoke(this,e);
        }

        private void _remoteApp_ExposureCompleted(object sender, ExposureCompletedEventArgs e)
        {
            ExposureCompleted?.Invoke(this,e);
        }

        public void StartExposure(short iso, double durationSeconds, bool isLightFrame)
        {
            try
            {
                _remoteApp.StartExposure(iso, durationSeconds, _cameraModel.ShutterSpeeds.Select(s=>s.DurationSeconds).Max());
            }
            catch (Exception e)
            {
                ExposureFailed?.Invoke(this, new ExposureFailedEventArgs(e));
            }
        }

        public void AbortExposure()
        {
            _remoteApp.AbortExposure();
        }

        public void StopExposure()
        {
            _remoteApp.StopExposure();
        }

        public void Connect()
        {
            _remoteApp.Connect();
        }

        public void Disconnect()
        {
            _remoteApp.Disconnect();
        }

        public bool IsConnected()
        {
            return _remoteApp.IsConnected;
        }
    }
}
