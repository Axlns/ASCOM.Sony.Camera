using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ASCOM.Sony
{
    public class ImageStatistics
    {
        public uint MaxADU { get; set; }
        public uint MinADU { get; set; }
        public uint MeanADU { get; set; }

        public uint MedianADU { get; set; }
        public uint Count { get; set; }
    }

    public class ImageDataProcessor
    {
        public ImageStatistics GetImageStatistics(Array imageArray)
        {
            ulong sum = 0;
            uint count = 0;
            uint min = uint.MaxValue;
            uint max = 0;

            int rank = imageArray.Rank;

            uint[] aduValues = new uint[imageArray.Length];

            for (int i = 0; i < imageArray.GetLength(0); i++)
            {
                for (int j = 0; j < imageArray.GetLength(1); j++)
                {
                    if (rank == 3)
                    {
                        for (int k=0; k < imageArray.GetLength(2); k++)
                        {
                            uint aduValue = (uint) imageArray.GetValue(i, j, k);

                            if (aduValue < min)
                            {
                                min = aduValue;
                            }

                            if (aduValue > max)
                            {
                                max = aduValue;
                            }

                            sum += aduValue;
                            aduValues[count++] = aduValue;
                        }
                    }
                    else
                    {
                        uint aduValue = (uint)imageArray.GetValue(i, j);

                        if (aduValue < min)
                        {
                            min = aduValue;
                        }

                        if (aduValue > max)
                        {
                            max = aduValue;
                        }

                        sum += aduValue;
                        aduValues[count++] = aduValue;
                    }
                }
            }
            
            uint mean = (uint)(((double)sum) / (imageArray.Length));

            return  new ImageStatistics()
            {
                MaxADU = max,
                MinADU = min,
                MeanADU = mean,
                MedianADU = GetMedian(aduValues),
                Count = count
            };
        }

        public unsafe uint[,] ReadRaw(string fileName)
        {
            IntPtr data = LoadRaw(fileName);

            var dataStructure = GetStructure<libraw_data_t>(data);

            var colorsStr = dataStructure.idata.cdesc;

            if (colorsStr != "RGBG")
                throw new NotImplementedException();

            int xOffset = 0;
            int yOffset = 0;

            string cameraPattern = "";
            cameraPattern += colorsStr[NativeMethods.libraw_COLOR(data, 0, 0)];
            cameraPattern += colorsStr[NativeMethods.libraw_COLOR(data, 0, 1)];
            cameraPattern += colorsStr[NativeMethods.libraw_COLOR(data, 1, 0)];
            cameraPattern += colorsStr[NativeMethods.libraw_COLOR(data, 1, 1)];

            switch (cameraPattern)
            {
                case "RGGB":
                    break;
                case "GRBG":
                    xOffset = 1;
                    break;
                case "BGGR":
                    xOffset = 1;
                    yOffset = 1;
                    break;
                case "GBRG":
                    yOffset = 1;
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            ushort rawWidth = dataStructure.rawdata.sizes.raw_width;
            ushort width = dataStructure.rawdata.sizes.width;
            ushort height = dataStructure.rawdata.sizes.height;
            //ushort cropWidth = dataStructure.rawdata.sizes.raw_crop.cwidth;
            //ushort cropHeight = dataStructure.rawdata.sizes.raw_crop.cheight;

            var pixels = new uint[width, height];

            ushort* ptr = (ushort*)dataStructure.rawdata.raw_image.ToPointer();

            for (int y = 0; y < height - yOffset; y++)
            {
                for (int x = 0; x < width - xOffset; x += 1)
                {
                    ushort ADU = *(ptr + rawWidth * y + x);
                    pixels[x + xOffset, y + yOffset] = ADU;
                }
            }

            NativeMethods.libraw_close(data);

            return pixels;
        }

        public uint[,,] ReadAndDebayerRaw(string fileName)
        {
            IntPtr data = LoadRaw(fileName);

            //CheckError(NativeMethods.libraw_raw2image(data), "raw2image");
            CheckError(NativeMethods.libraw_dcraw_process(data), "dcraw_process");

            var dataStructure = GetStructure<libraw_data_t>(data);
            ushort width = dataStructure.sizes.iwidth;
            ushort height = dataStructure.sizes.iheight;

            var pixels = new uint[width, height, 3];

            for (int rc = 0; rc < height * width; rc++)
            {
                var r = (ushort)Marshal.ReadInt16(dataStructure.image, rc * 8);
                var g = (ushort)Marshal.ReadInt16(dataStructure.image, rc * 8 + 2);
                var b = (ushort)Marshal.ReadInt16(dataStructure.image, rc * 8 + 4);

                int row = rc / width;
                int col = rc - width * row;
                //int rowReversed = height - row - 1;
                pixels[col, row, 0] = b;
                pixels[col, row, 1] = g;
                pixels[col, row, 2] = r;
            }

            NativeMethods.libraw_close(data);

            return pixels;
        }

        public uint[,,] ReadJpeg(string fileName)
        {
            using (Bitmap img = new Bitmap(fileName))
            {
                var result = ReadBitmap(img);

                return result;
            }
        }

        private uint[,,] ReadBitmap(Bitmap img)
        {
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            IntPtr ptr = data.Scan0;
            int bytesCount = Math.Abs(data.Stride) * img.Height;
            

            byte[] bytesArray = new byte[bytesCount];
            Marshal.Copy(ptr, bytesArray, 0, bytesCount);
            img.UnlockBits(data);

            var width = img.Width;
            var height = img.Height;

            var result = new uint[width, height, 3];

            for (int rc = 0; rc < width * height; rc++)
            {
                var b = bytesArray[rc * 3];
                var g = bytesArray[rc * 3 + 1];
                var r = bytesArray[rc * 3 + 2];

                int row = rc / width;
                int col = rc - width * row;

                //var rowReversed = height - row - 1;
                result[col, row, 0] = b;
                result[col, row, 1] = g;
                result[col, row, 2] = r;
            }

            return result;
        }

        

        public Array ToVariantArray(Array data)
        {
            int xLength = data.GetLength(0);
            int yLength = data.GetLength(1);

            Array result = data.Rank == 3? Array.CreateInstance(typeof(object), xLength, yLength, 3) : Array.CreateInstance(typeof(object), xLength, yLength);

            for (int x = 0; x < xLength; x++)
            for (int y = 0; y < yLength; y++)
            {
                if (data.Rank == 3)
                {
                    for (int r = 0; r < 3; r++)
                    {
                        result.SetValue((object)data.GetValue(x, y, r), x, y, r);
                    }
                }
                else
                {
                    result.SetValue((object)data.GetValue(x, y), x, y);
                }
            }

            return result;
        }

        public Array CutImageArray(Array data, int StartX, int StartY, int NumX, int NumY, int CameraXSize, int CameraYSize)
        {
            Array result = null;
            int rank = data.Rank;

            int xLength = data.GetLength(0);
            int yLength = data.GetLength(1);

            if (IsCutRequired(xLength, yLength, StartX, StartY, NumX, NumY, CameraXSize, CameraYSize))
            {
                int startXCorrected = StartX % 2 == 0 ? StartX : StartX - 1;
                int startYCorrected = StartY % 2 == 0 ? StartY : StartY - 1;

                xLength = Math.Min(xLength, NumX);
                yLength = Math.Min(yLength, NumY);

                result = rank == 3 ? (Array) new uint[xLength, yLength, 3] : new uint[xLength, yLength];

                for (int x = 0; x < xLength; x++)
                { 
                    for (int y = 0; y < yLength; y++)
                    {
                        int dataX = startXCorrected + x;
                        int dataY = startYCorrected + y;
                        if (rank == 3)
                        {
                            for (int r = 0; r < 3; r++)
                            {
                                result.SetValue(data.GetValue(dataX, dataY, r), x, y, r);
                            }
                        }
                        else
                        {
                            result.SetValue(data.GetValue(dataX, dataY), x, y);
                        }
                    }
                }
            }
            else
            {
                result = data;
            }
            return result;
        }

        private void exif_parser_callback(IntPtr context, int tag, int type, int len, uint ord, IntPtr ifp)
        {

        }
        private IntPtr LoadRaw(string fileName)
        {
            var data = NativeMethods.libraw_init(LibRaw_constructor_flags.LIBRAW_OPIONS_NO_DATAERR_CALLBACK);

            CheckError(NativeMethods.libraw_open_file(data, fileName), "open file");
            CheckError(NativeMethods.libraw_unpack(data), "unpack");

            return data;
        }

        private void CheckError(int errorCode, string action)
        {
            if (errorCode != 0)
                throw new Exception($"LibRaw error {errorCode} when {action}");
        }

        private bool IsCutRequired(int dataXsize, int dataYsize, int StartX, int StartY, int NumX, int NumY, int CameraXSize, int CameraYSize)
        {
            bool sizeMatches = StartX == 0 && StartY == 0 && NumX == CameraXSize && NumY == CameraYSize
                && dataXsize == CameraXSize && dataYsize == CameraYSize;

            bool cut = !(sizeMatches || NumX == 0 || NumY == 0);
            return cut;
        }

        private uint GetMedian(uint[] aduValues)
        {
            Array.Sort(aduValues);
            uint size = (uint)aduValues.Length;
            uint mid = size / 2;
            uint median = (size % 2 != 0) ? aduValues[mid] : (aduValues[mid] + aduValues[mid - 1]) / 2;
            return median;
        }

        private int GetSum(IEnumerable<int> sourceNumbers, int binx, int biny)
        {
            int binCount = binx * biny;
            var sum = sourceNumbers.Sum();

            if (binCount > 4)
            {
                sum = sum >> 2;
            }

            return sum;
        }

        private T GetStructure<T>(IntPtr ptr) where T : struct
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        //public int From8To16Bit(int value)
        //{
        //    int result = value << 5;

        //    return result;
        //}
    }
}
