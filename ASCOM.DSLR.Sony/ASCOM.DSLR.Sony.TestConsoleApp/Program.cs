using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.DSLR.Sony.TestConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            SonyCamera camera = new SonyCamera(CameraModel.Models.First(m=>m.ID=="SLTA99"), ImageFormat.CFA, false);
            
            camera.ExposureReady += Camera_ExposureReady;
            camera.ExposureCompleted += Camera_ExposureCompleted;

            camera.Connect();

            Console.WriteLine("Press S to take exposure. Press E to exit program.");
            do
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.S)
                {
                    camera.StartExposure(400, 2, true);
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
            Console.WriteLine("Exposure completed. Image array length : {0}",e.ImageArray.Length);
        }
    }
}
