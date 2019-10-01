using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ASCOM.DSLR.Sony.TestConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            SonyCamera camera = new SonyCamera(CameraModel.Models.First(m => m.ID == "SLTA99"), ImageFormat.CFA, false);

            camera.ExposureReady += Camera_ExposureReady;
            camera.ExposureCompleted += Camera_ExposureCompleted;

            camera.Connect();

            Console.WriteLine("Press S to take exposure. Press E to exit program.");
            do
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.S)
                {
                    camera.StartExposure(400, 3, true);
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
            var array = (int[,]) e.ImageArray;

            int width = array.GetLength(0);
            int height = array.GetLength(1);

            var flatArray = new int[width*height];

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
            SaveToTiff("F:\\astrophoto\\~test\\test.tiff", array);

        }


        private static void SaveToTiff(string filename, int[,] imageArray)
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
    }
}
