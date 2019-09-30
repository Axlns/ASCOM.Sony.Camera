using System;
using System.Threading;

namespace TestShutterApp
{
    class Program
    {
        

        static void Main(string[] args)
        {

            SonyRemoteAppInterop sonyRemoteAppInterop = new SonyRemoteAppInterop();
            sonyRemoteAppInterop.Connect();


            Console.WriteLine($"Current ISO: {sonyRemoteAppInterop.GetCurrentISO()}");
            Console.WriteLine($"Current Shutter Speed: {sonyRemoteAppInterop.GetCurrentShutterSpeed()}");

            sonyRemoteAppInterop.IncreaseISO();
            sonyRemoteAppInterop.DecreaseShutterSpeed();
            sonyRemoteAppInterop.DecreaseShutterSpeed();
            sonyRemoteAppInterop.DecreaseShutterSpeed();
            sonyRemoteAppInterop.DecreaseShutterSpeed();

            sonyRemoteAppInterop.TakeExposure(0, 5000);

            Console.WriteLine($"Current ISO: {sonyRemoteAppInterop.GetCurrentISO()}");
            Console.WriteLine($"Current Shutter Speed: {sonyRemoteAppInterop.GetCurrentShutterSpeed()}");


            //sonyRemoteAppInterop.TakeExposure(100, 5000);

        }
    }
}
