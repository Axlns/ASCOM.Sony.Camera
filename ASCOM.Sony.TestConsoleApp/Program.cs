using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ASCOM.Sony.TestConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            SonyCamera camera = new SonyCamera(CameraModel.Models.First(m => m.ID == "SLTA99"), ImageFormat.CFA, false);

            camera.ExposureReady += Camera_ExposureReady;
            camera.ExposureCompleted += Camera_ExposureCompleted;

            Console.WriteLine("Press S to take exposure.\nPress R to read ARW file and save to grayscale TIFF.\nPress D to read ARW file and save to color TIFF.\nPress J to read JPEG file and save to color TIFF.\nPress E to exit program.");
            do
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.S)
                {
                    if (camera.IsConnected() == false)
                    {
                        camera.Connect();
                    }
                    camera.StartExposure(400, 3, true);
                }
                else if (key.Key == ConsoleKey.R)
                {
                    ImageDataProcessor dataProcessor = new ImageDataProcessor();

                    Console.Write("Reading RAW file to array ...");
                    uint[,] array = dataProcessor.ReadRaw("D:\\astrophoto\\test\\DSC08256.ARW");
                    Console.WriteLine(" Done.");

                    Console.WriteLine($"Array length: {array.LongLength*4} bytes");

                    Console.Write("Saving to tiff...");
                    SaveToGrayscaleTiff("D:\\astrophoto\\test\\grayscale.tiff", array);
                    Console.WriteLine(" Done.");
                }
                else if(key.Key == ConsoleKey.D)
                {
                    ImageDataProcessor dataProcessor = new ImageDataProcessor();

                    Console.Write("Reading RAW file and debayer to array ...");
                    uint[,,] array = dataProcessor.ReadAndDebayerRaw("D:\\astrophoto\\test\\DSC08256.ARW");
                    Console.WriteLine(" Done.");

                    Console.WriteLine($"Array length: {array.LongLength*4} bytes");

                    Console.Write("Saving to tiff...");
                    SaveToColorTiff("D:\\astrophoto\\test\\color.tiff", array);
                    Console.WriteLine(" Done.");
                }
                else if (key.Key == ConsoleKey.J)
                {
                    ImageDataProcessor dataProcessor = new ImageDataProcessor();

                    Console.Write("Reading JPEG file array ...");
                    uint[,,] array = dataProcessor.ReadJpeg("D:\\astrophoto\\test\\DSC02292.JPG");
                    Console.WriteLine(" Done.");

                    Console.WriteLine($"Array length: {array.LongLength} bytes");

                    Console.Write("Saving to tiff...");

                    
                    SaveToColor8bitTiff("D:\\astrophoto\\test\\jpeg.tiff", array);
                        
                    
                    Console.WriteLine(" Done.");
                }
                else if (key.Key == ConsoleKey.E)
                {
                    break;
                }
            } while (true);
        }

        private static void Camera_ExposureCompleted(object sender, ExposureCompletedEventArgs e)
        {
            Console.WriteLine("Exposure completed. Downloading ...");
        }

        private static void Camera_ExposureReady(object sender, ExposureReadyEventArgs e)
        {
            var array = (uint[,]) e.ImageArray;

            int width = array.GetLength(0);
            int height = array.GetLength(1);

            var flatArray = new uint[width*height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    flatArray[j * width + i] = array[i, j];
                }
            }

            Console.WriteLine($"Image array length: {array.LongLength}");
            Console.WriteLine($"Image dimensions: {width}x{height}");
            Console.WriteLine($"ADU max/min: {flatArray.Min()}/{flatArray.Max()}");

            Console.WriteLine("Saving to tiff...");
            SaveToGrayscaleTiff("F:\\astrophoto\\~test\\test.tiff", array);

        }


        private static void SaveToGrayscaleTiff(string filename, uint[,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new ushort[width * height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    array[j * width + i] = (ushort)imageArray[i, j];
                }
            }

            var bitmapSrc = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray16, null, array, 4 * ((width * 2 + 3) / 4));
           
            using (var fs = File.OpenWrite(filename))
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private static void SaveToColorTiff(string filename, uint[,,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new ushort[width * height * 3];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i ++)
                {
                    array[j * width*3 + i*3] = (ushort)imageArray[i, j,2];
                    array[j * width * 3 + i*3+1] = (ushort)imageArray[i, j, 1];
                    array[j * width * 3 + i*3+2] = (ushort)imageArray[i, j, 0];
                }
            }

            var bitmapSrc = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb48, null, array, 4 * ((width * 6 + 3) / 4));

            using (var fs = File.OpenWrite(filename))
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private static void SaveToColor8bitTiff(string filename, uint[,,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new Byte[width * height * 3];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    array[j * width * 3 + i * 3] = (byte)imageArray[i, j, 2];
                    array[j * width * 3 + i * 3 + 1] = (byte)imageArray[i, j, 1];
                    array[j * width * 3 + i * 3 + 2] = (byte)imageArray[i, j, 0];
                }
            }

            var bitmapSrc = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, array, 4 * ((width * 3 + 3) / 4));

            using (var fs = File.OpenWrite(filename))
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
                encoder.Save(fs);
                fs.Close();
            }
        }
    }
}
