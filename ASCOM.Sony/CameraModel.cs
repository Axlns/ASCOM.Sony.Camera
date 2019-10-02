using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ASCOM.Utilities;
using Newtonsoft.Json;

namespace ASCOM.Sony
{
    public class ShutterSpeed
    {
        public double DurationSeconds { get; set; }
        public string Name { get; set; }

        public ShutterSpeed()
        {

        }

        public ShutterSpeed(string name, double durationSeconds)
        {
            Name = name;
            DurationSeconds = durationSeconds;
        }
    }

    public class Sensor
    {

        public string Name { get; set; }

        /// <summary>
        /// Sensor width in millimeters
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Sensor height in millimeters
        /// </summary>
        public double Height { get; set; }

        ///// <summary>
        ///// Total sensor width in pixels (including reserved sensor areas)
        ///// </summary>
        //public ushort RawWidth { get; set; }
        ///// <summary>
        ///// Total sensor height in pixels (including reserved sensor areas)
        ///// </summary>
        //public ushort RawHeight { get; set; }

        /// <summary>
        /// Usable sensor width in pixels
        /// </summary>
        public ushort FrameWidth { get; set; }

        /// <summary>
        /// Usable sensor height in pixels
        /// </summary>
        public ushort FrameHeight { get; set; }

        /// <summary>
        /// Camera crop width (used for JPG files)
        /// </summary>
        public ushort CropWidth { get; set; }
        /// <summary>
        /// Camera crop height (used for JPG files)
        /// </summary>
        public ushort CropHeight { get; set; }

        public double PixelSizeWidth { get; set; }
        public double PixelSizeHeight { get; set; }

        public ushort GetReadoutWidth(ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.CFA:
                case ImageFormat.Debayered:
                    return FrameWidth;
                case ImageFormat.JPG:
                    return CropWidth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageFormat), imageFormat, null);
            }
        }

        public ushort GetReadoutHeight(ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.CFA:
                case ImageFormat.Debayered:
                    return FrameHeight;
                case ImageFormat.JPG:
                    return CropHeight;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageFormat), imageFormat, null);
            }
        }

    }

    public class CameraModel
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public Sensor Sensor { get; set; }
        
        public double ExposureMin { get; set; }

        public double ExposureMax { get; set; }

        public short[] Gains { get; set; }

        public ShutterSpeed[] ShutterSpeeds { get; set; }
        
        public double ElectronsPerADU { get; set; }
        public double ExposureResolution { get; set; }
        public double FullWellCapacity { get; set; }
        
        public static CameraModel[] Models { get; set; }
    }
}
