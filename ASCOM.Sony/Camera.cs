using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;

namespace ASCOM.Sony
{

    public enum ImageFormat
    {
        CFA,
        Debayered,
        JPG
    }

    public partial class Camera
    {
        private ImageDataProcessor _imageDataProcessor = new ImageDataProcessor();

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;

        private SonyCamera _sonyCamera;
        
        private CameraStates _cameraState = CameraStates.cameraIdle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ASCOM.Sony"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Camera()
        {
            tl = new TraceLogger("", "DSLR.Sony");
            ReadProfile(); // Read device configuration from the ASCOM Profile store

            tl.LogMessage("Camera", "Starting initialization");

            connectedState = false; // Initialise connected to false
            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            //TODO: Implement your additional construction here

            tl.LogMessage("Camera", "Completed initialization");
        }

        #region ICamera Methods Implementation
        public void StartExposure(double Duration, bool Light)
        {
            CheckConnected("Camera not connected");

            //todo: add code to avoid possible race condition which could theoretically happen if client application calls StartExposure and then immediately disconnects from camera; so checkConnected above passes but access to _sonlyCamera below would throw NullReferenceException

            if (Duration < 0.0) throw new InvalidValueException("StartExposure", Duration.ToString(), "0.0 upwards");
            ushort readoutWidth = cameraModel.Sensor.GetReadoutWidth(imageFormat);
            ushort readoutHeight = cameraModel.Sensor.GetReadoutHeight(imageFormat);

            if (cameraNumX > readoutWidth) throw new InvalidValueException("StartExposure", cameraNumX.ToString(), readoutWidth.ToString());
            if (cameraNumY > readoutHeight) throw new InvalidValueException("StartExposure", cameraNumY.ToString(), readoutHeight.ToString());
            if (cameraStartX > readoutWidth) throw new InvalidValueException("StartExposure", cameraStartX.ToString(), readoutWidth.ToString());
            if (cameraStartY > readoutHeight) throw new InvalidValueException("StartExposure", cameraStartY.ToString(), readoutHeight.ToString());

            if (_cameraState != CameraStates.cameraIdle) throw new InvalidOperationException("Cannot start exposure - camera is not idle");

            tl.LogMessage("StartExposure", $"Duration: {Duration} s. ISO: {Gains[Gain]}. {(Light ? "Light" : "Dark")} frame.");

            cameraImageReady = false;
            cameraImageArray = null;
            cameraLastExposureDuration = Duration;
            exposureStart = DateTime.Now;
            _cameraState = CameraStates.cameraExposing;

            SubscribeCameraEvents();
            _sonyCamera.StartExposure((short)Gains[Gain], Duration, Light);
        }

        private void SubscribeCameraEvents()
        {
            _sonyCamera.ExposureCompleted += _sonyCamera_ExposureCompleted;
            _sonyCamera.ExposureReady += _sonyCamera_ExposureReady;
        }

        private void UnsubscribeCameraEvents()
        {
            if (_sonyCamera != null)
            {
                _sonyCamera.ExposureCompleted -= _sonyCamera_ExposureCompleted;
                _sonyCamera.ExposureReady -= _sonyCamera_ExposureReady;
            }
        }

        private void _sonyCamera_ExposureReady(object sender, ExposureReadyEventArgs e)
        {
            tl.LogMessage("_sonyCamera_ExposureReady", string.Format("ImageArray Length: {0}",e.ImageArray.Length));

            if (IsConnected == false)
            {
                _cameraState = CameraStates.cameraIdle;
                return;
            }
            
            _cameraState = CameraStates.cameraDownload;

            try
            {
                try
                {
                    //report stats to log (if tracing enabled)
                    if (tl.Enabled)
                    {
                        var stats = _imageDataProcessor.GetImageStatistics(e.ImageArray);
                        if (stats != null)
                        {
                            tl.LogMessage("_sonyCamera_ExposureReady", $"Image statistics: ADU min/max/mean/median: {stats.MinADU}/{stats.MaxADU}/{stats.MeanADU}/{stats.MedianADU}.");
                        }
                    }
                    cameraImageArray = _imageDataProcessor.CutImageArray(e.ImageArray, StartX, StartY, NumX, NumY, CameraXSize, CameraYSize);
                    _cameraState = CameraStates.cameraIdle;
                    cameraImageReady = true;
                }
                catch (Exception ex)
                {
                    tl.LogIssue(ex.Message, ex.StackTrace);
                    _cameraState = CameraStates.cameraError;
                }
            }
            finally
            {
                UnsubscribeCameraEvents();
            }
        }

        private void _sonyCamera_ExposureCompleted(object sender, ExposureCompletedEventArgs e)
        {
            tl.LogMessage("_sonyCamera_ExposureCompleted", "cameraReading"); 
            _cameraState = CameraStates.cameraReading;
        }

        public void AbortExposure()
        {
            CheckConnected("Camera not connected");
            tl.LogMessage("AbortExposure","");
            if (_cameraState == CameraStates.cameraExposing)
            {
                _sonyCamera.AbortExposure();
            }

            _cameraState = CameraStates.cameraIdle;
        }

        public void StopExposure()
        {
            CheckConnected("Camera not connected");
            tl.LogMessage("StopExposure", "");
            if (_cameraState == CameraStates.cameraExposing)
            {
                _sonyCamera.StopExposure();
            }
        }
        #endregion

        #region ICamera Connected


        public bool Connected
        {
            get
            {
                LogMessage("Connected", "Get {0}", IsConnected);
                return IsConnected;
            }
            set
            {
                tl.LogMessage("Connected", "Set {0}", value);
                if (value == IsConnected)
                    return;

                if (value)
                {
                    connectedState = true;
                    LogMessage("Connected Set", "Connecting to Sony Imaging Edge Remote app");

                    try
                    {
                        if (_sonyCamera == null)
                        {
                            _sonyCamera = new SonyCamera(cameraModel,imageFormat,autoDeleteImageFile);
                        }
                        _sonyCamera.Connect();
                    }
                    catch (Exception e)
                    {
                        connectedState = false;
                        throw new ASCOM.NotConnectedException(e);
                    }
                }
                else
                {
                    connectedState = false;
                    _sonyCamera = null;
                    LogMessage("Connected Set", "Disconnecting from Sony Imaging Edge Remote app");
                }
            }
        }

        #endregion

        #region ICamera Properties Implementation

        //private const int ccdWidth = 6038; // Constants to define the ccd pixel dimensions
        //private const int ccdHeight = 4025;
        //private const double pixelSize = 5.97; // Constant for the pixel physical dimension

        private int cameraNumX;
        private int cameraNumY;
        private int cameraStartX = 0;
        private int cameraStartY = 0;
        private DateTime exposureStart = DateTime.MinValue;
        private double cameraLastExposureDuration = 0.0;
        private bool cameraImageReady = false;
        private Array cameraImageArray; //sony interop component will return it as UInt16, needs to be converted to Int32 before returned to driver users
        private object[,] cameraImageArrayVariant;
        private short _binX = 1;
        private short _binY = 1;

        public short BayerOffsetX
        {
            get
            {
                tl.LogMessage("BayerOffsetX Get Get", 0.ToString());
                return 0;
            }
        }

        public short BayerOffsetY
        {
            get
            {
                tl.LogMessage("BayerOffsetY Get Get", 0.ToString());
                return 0;
            }
        }

        public short BinX
        {
            get
            {
                tl.LogMessage("BinX Get", _binX.ToString());
                return _binX;
            }
            set
            {
                tl.LogMessage("BinX Set", value.ToString());
                _binX = value;
            }
        }

        public short BinY
        {
            get
            {
                tl.LogMessage("BinY Get", _binY.ToString());
                return _binY;
            }
            set
            {
                tl.LogMessage("BinY Set", value.ToString());
                _binY = value;
            }
        }

        public double CCDTemperature
        {
            get
            {
                //normally we should throw  ASCOM.PropertyNotImplementedException because we cannot control sensor temperature; but some apps seems to try to read that value and do not work correctly if they cant (example NINA)
                //so as workaround user can set fixed temperature in cameramodel.JSON file

                if (cameraModel.Sensor.CCDTemperature.HasValue)
                {
                   return cameraModel.Sensor.CCDTemperature.Value;
                }
                
                tl.LogMessage("CCDTemperature Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CCDTemperature", false);
            }
        }

        
        public CameraStates CameraState
        {
            get
            {
                tl.LogMessage("CameraState Get", _cameraState.ToString());
                return _cameraState;
            }
        }

        public int CameraXSize
        {
            get
            {
                ushort readoutWidth = cameraModel.Sensor.GetReadoutWidth(imageFormat);
                
                tl.LogMessage("CameraXSize Get", readoutWidth.ToString());
                return readoutWidth;
            }
        }

        public int CameraYSize
        {
            get
            {
                ushort readoutHeight = cameraModel.Sensor.GetReadoutHeight(imageFormat);
                tl.LogMessage("CameraYSize Get", readoutHeight.ToString());
                return readoutHeight;
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                tl.LogMessage("CanAbortExposure Get", true.ToString());
                return true;
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                tl.LogMessage("CanAsymmetricBin Get", false.ToString());
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                tl.LogMessage("CanFastReadout Get", false.ToString());
                return false;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                tl.LogMessage("CanGetCoolerPower Get", false.ToString());
                return false;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                tl.LogMessage("CanPulseGuide Get", false.ToString());
                return false;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                tl.LogMessage("CanSetCCDTemperature Get", false.ToString());
                return false;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                tl.LogMessage("CanStopExposure Get", true.ToString());
                return true;
            }
        }

        public bool CoolerOn
        {
            get
            {
                tl.LogMessage("CoolerOn Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", false);
            }
            set
            {
                tl.LogMessage("CoolerOn Set Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", true);
            }
        }

        public double CoolerPower
        {
            get
            {
                tl.LogMessage("CoolerPower Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerPower", false);
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                tl.LogMessage("ElectronsPerADU Get", cameraModel.Sensor.ElectronsPerADU.ToString());
                return cameraModel.Sensor.ElectronsPerADU;
            }
        }

        public double ExposureMax
        {
            get
            {
                tl.LogMessage("ExposureMax Get", cameraModel.ExposureMax.ToString());
                return cameraModel.ExposureMax;
            }
        }

        public double ExposureMin
        {
            get
            {
                tl.LogMessage("ExposureMin Get", cameraModel.ExposureMin.ToString());
                return cameraModel.ExposureMin;
            }
        }

        public double ExposureResolution
        {
            get
            {
                tl.LogMessage("ExposureResolution Get", cameraModel.ExposureResolution.ToString());
                return cameraModel.ExposureResolution;
            }
        }

        public bool FastReadout
        {
            get
            {
                tl.LogMessage("FastReadout Get", "Not implemented");
                CheckConnected("Not Connected");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
            }
            set
            {
                tl.LogMessage("FastReadout Set", "Not implemented");
                CheckConnected("Not Connected");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", true);
            }
        }

        public double FullWellCapacity
        {
            get
            {
                tl.LogMessage("FullWellCapacity Get", cameraModel.Sensor.FullWellCapacity.ToString());
                return cameraModel.Sensor.FullWellCapacity;
            }
        }

        private short _gain;
        public short Gain
        {
            get
            {
                tl.LogMessage("Gain Get", _gain.ToString());
                return _gain;
            }
            set
            {
                tl.LogMessage("Gain Set", value.ToString());
                _gain = value;
            }
        }

        public short GainMax
        {
            get
            {
                tl.LogMessage("GainMax Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("GainMax", false);
            }
        }

        public short GainMin
        {
            get
            {
                tl.LogMessage("GainMin Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("GainMin", true);
            }
        }

        public ArrayList Gains
        {
            get
            {
                tl.LogMessage("Gains Get", cameraModel.Gains.ToString());//todo: log values of array
                return new ArrayList(cameraModel.Gains);//todo; should we create new array list every time?
            }
        }

        public bool HasShutter
        {
            get
            {
                tl.LogMessage("HasShutter Get", false.ToString());
                return false;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                tl.LogMessage("HeatSinkTemperature Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("HeatSinkTemperature", false);
            }
        }

        public object ImageArray
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("ImageArray Get", "Throwing InvalidOperationException because of a call to ImageArray before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArray before the first image has been taken!");
                }

                return cameraImageArray;
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("ImageArrayVariant Get", "Throwing InvalidOperationException because of a call to ImageArrayVariant before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArrayVariant before the first image has been taken!");
                }

                return _imageDataProcessor.ToVariantArray(cameraImageArray);
            }
        }

        public bool ImageReady
        {
            get
            {
                tl.LogMessage("ImageReady Get", cameraImageReady.ToString());
                return cameraImageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                tl.LogMessage("IsPulseGuiding Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("IsPulseGuiding", false);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("LastExposureDuration Get", "Throwing InvalidOperationException because of a call to LastExposureDuration before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureDuration before the first image has been taken!");
                }
                tl.LogMessage("LastExposureDuration Get", cameraLastExposureDuration.ToString());
                return cameraLastExposureDuration;
            }
        }

        public string LastExposureStartTime
        {
            get
            {
                if (!cameraImageReady)
                {
                    tl.LogMessage("LastExposureStartTime Get", "Throwing InvalidOperationException because of a call to LastExposureStartTime before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureStartTime before the first image has been taken!");
                }
                string exposureStartString = exposureStart.ToString("yyyy-MM-ddTHH:mm:ss");
                tl.LogMessage("LastExposureStartTime Get", exposureStartString.ToString());
                return exposureStartString;
            }
        }

        public int MaxADU
        {
            get
            {
                int maxADU;
                
                switch (imageFormat)
                {
                    case ImageFormat.CFA:
                        maxADU = cameraModel.Sensor.MaxADU;
                        break;
                    case ImageFormat.Debayered: //note: we dont use sensor characteristic here because we use LibRaw to debayer RAW image and it is doing lot of things with the file, including I believe expanding color values to full 16bit
                        maxADU = ushort.MaxValue;
                        break;
                    case ImageFormat.JPG:
                        maxADU = byte.MaxValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                tl.LogMessage("MaxADU Get", maxADU.ToString());
                return maxADU;
            }
        }

        public short MaxBinX
        {
            get
            {
                tl.LogMessage("MaxBinX Get", "1");
                return 1;
            }
        }

        public short MaxBinY
        {
            get
            {
                tl.LogMessage("MaxBinY Get", "1");
                return 1;
            }
        }

        public int NumX
        {
            get
            {
                tl.LogMessage("NumX Get", cameraNumX.ToString());
                return cameraNumX;
            }
            set
            {
                cameraNumX = value;
                tl.LogMessage("NumX set", value.ToString());
            }
        }

        public int NumY
        {
            get
            {
                tl.LogMessage("NumY Get", cameraNumY.ToString());
                return cameraNumY;
            }
            set
            {
                cameraNumY = value;
                tl.LogMessage("NumY set", value.ToString());
            }
        }

        public short PercentCompleted
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new NotSupportedException("PercentCompleted (not supported for Interface V1)");
                }

                CheckConnected("Can't get PercentCompleted when not connected");

                short percentCompleted = 0;
                switch (_cameraState)
                {
                    case CameraStates.cameraIdle:
                        percentCompleted = (short) (ImageReady ? 100 : 0);
                        break;
                    case CameraStates.cameraWaiting:
                    case CameraStates.cameraExposing:
                    case CameraStates.cameraReading:
                    case CameraStates.cameraDownload:
                        percentCompleted = (short) ((DateTime.Now-exposureStart).TotalSeconds/LastExposureDuration*100.00);
                        break;
                    case CameraStates.cameraError:
                        return (short) 0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                tl.LogMessage("PercentCompleted Get", "Not implemented");
                return percentCompleted;
            }
        }

        public double PixelSizeX
        {
            get
            {
                return cameraModel.Sensor.PixelSizeWidth*BinX;
            }
        }

        public double PixelSizeY
        {
            get
            {
                return cameraModel.Sensor.PixelSizeHeight*BinY;
            }
        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            tl.LogMessage("PulseGuide", "Not implemented");
            throw new ASCOM.MethodNotImplementedException("PulseGuide");
        }

        public short ReadoutMode
        {
            get
            {
                tl.LogMessage("ReadoutMode Get", $"{(short)ReadoutModes.IndexOf(imageFormat.ToString())} {imageFormat.ToString()}");
                return (short)ReadoutModes.IndexOf(imageFormat.ToString());
            }
            set
            {
                tl.LogMessage("ReadoutMode Set", value.ToString());
                imageFormat = (ImageFormat) Enum.Parse(typeof(ImageFormat), (string) ReadoutModes[value], true);
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                tl.LogMessage("ReadoutModes Get", "Not implemented");
                return new ArrayList(new [] { ImageFormat.CFA.ToString(), ImageFormat.Debayered.ToString(), ImageFormat.JPG.ToString() });
            }
        }

        public string SensorName
        {
            get
            {
                tl.LogMessage("SensorName Get", cameraModel.Sensor.Name);
                return cameraModel.Sensor.Name;
            }
        }

        public SensorType SensorType
        {
            get
            {
                tl.LogMessage("SensorType Get", SensorType.RGGB.ToString());
                switch (imageFormat)
                {
                    case ImageFormat.CFA:
                        return SensorType.RGGB;
                    case ImageFormat.Debayered:
                    case ImageFormat.JPG:
                        return SensorType.Color;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                tl.LogMessage("SetCCDTemperature Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", false);
            }
            set
            {
                tl.LogMessage("SetCCDTemperature Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", true);
            }
        }

        public int StartX
        {
            get
            {
                tl.LogMessage("StartX Get", cameraStartX.ToString());
                return cameraStartX;
            }
            set
            {
                cameraStartX = value;
                tl.LogMessage("StartX Set", value.ToString());
            }
        }

        public int StartY
        {
            get
            {
                tl.LogMessage("StartY Get", cameraStartY.ToString());
                return cameraStartY;
            }
            set
            {
                cameraStartY = value;
                tl.LogMessage("StartY set", value.ToString());
            }
        }
        
       
        #endregion
    }
}
