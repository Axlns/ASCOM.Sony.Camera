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
            //SonyCamera camera = new SonyCamera(CameraModel.Models.First(m=>m.ID=="SLTA99"), ImageFormat.CFA, false);
            
            //camera.ExposureReady += Camera_ExposureReady;
            //camera.ExposureCompleted += Camera_ExposureCompleted;

            //camera.Connect();

            //Console.WriteLine("Press S to take exposure. Press E to exit program.");
            //do
            //{
            //    var key = Console.ReadKey();
            //    if (key.Key == ConsoleKey.S)
            //    {
            //        camera.StartExposure(400, 2, true);
            //    }
            //    else if (key.Key == ConsoleKey.E)
            //    {
            //        break;
            //    }
            //} while (true);

            ImageDataProcessor dataProcessor = new ImageDataProcessor();

            var array = dataProcessor.ReadRaw("D:\\astrophoto\\test\\DSC08256.ARW");

            Console.WriteLine(array.LongLength);

            Console.WriteLine("Saving to tiff...");
            SaveToTiff("D:\\astrophoto\\test\\test.tiff",array);

            Console.WriteLine("Completed. Prease any key to exit.");
            Console.ReadKey(true);
            //D:\astrophoto\test\FLAT_M81_00479.ARW
        }

        private static void Camera_ExposureCompleted(object sender, ExposureCompletedEventArgs e)
        {
            Console.WriteLine("Exposure completed. Downloading ...");
        }

        private static void Camera_ExposureReady(object sender, ExposureReadyEventArgs e)
        {
            Console.WriteLine("Exposure completed. Image array length : {0}",e.ImageArray.Length);
        }

        private static void SaveToTiff(string filename, int[,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new ushort[width*height];

            for (int i = 0; i < width; i++)
            {
                for (int j=0; j < height; j++)
                {
                    array[j*width + i] = (ushort)imageArray[i, j];
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
