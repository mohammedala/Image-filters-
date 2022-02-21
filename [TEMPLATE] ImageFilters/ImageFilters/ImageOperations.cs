using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageFilters
{
    public class ImageOperations
    {
        public const int Range = 256;
        /// <summary>
        /// Open an image, convert it to gray scale and load it into 2D array of size (Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of gray values</returns>
        public static byte[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            byte[,] Buffer = new byte[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x] = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x] = (byte)((int)(p[0] + p[1] + p[2]) / 3);
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(byte[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(byte[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = p[1] = p[2] = ImageMatrix[i, j];
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }

        void swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }

        int divide(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            int i = low - 1;

            for (int j = low; j <= high - 1; j++)
            {
                if (arr[j] < pivot)
                {
                    i++;
                    swap(ref arr[i], ref arr[j]);
                }
            }
            swap(ref arr[i + 1], ref arr[high]);
            return (i + 1);
        }

        void quickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {
                int pivot = divide(arr, low, high);
                quickSort(arr, low, pivot - 1);
                quickSort(arr, pivot + 1, high);
            }
        }

        // o(n + 255) = o(n)
        void countingSort(int[] arr)
        {
            int[] output = new int[arr.Length];
            int[] range = new int[Range];

            for (int i = 0; i < arr.Length; i++)
            {
                range[arr[i]]++;
            }

            for (int i = 1; i < range.Length; i++)
            {
                range[i] += range[i - 1];
            }

            for (int i = 0; i < arr.Length; i++)
            {
                output[range[arr[i]] - 1] = arr[i];
                range[arr[i]]--;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = output[i];
            }
        }

        // o(n)
        void kth(int[] arr)
        {
            int small = 255;
            int big = 0;
            int index = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0)
                    continue;

                else if (small > arr[i])
                {
                    small = arr[i];
                    index = i;
                }
            }
            arr[index] = 0; // smallest

            index = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (big < arr[i])
                {
                    big = arr[i];
                    index = i;
                }
            }
            arr[index] = 0; // biggest
        }

        // o(n^2)
        void creatingWindow(int[] window, byte[,] imageMatrix, int t, ref int rowFlag, ref int colFlag, ref int shifter)
        {
            int counter = 0;
            for (int i = rowFlag; i < t + rowFlag; i++)
            {
                for (int j = 0; j < t; j++)
                {
                    window[counter] = imageMatrix[i, j + shifter];
                    counter++;
                    colFlag = j + shifter;
                }

            }
            shifter++;
        }

        // n 4
        public byte[,] AlphaFilter(int t, int Size, byte[,] imageMatrix, int method)
        {
            int rowflag = 0;
            int colflag = 0;
            int shifter = 0;
            int[] window = new int[Size];
            byte[,] newImage = new byte[imageMatrix.GetLength(0), imageMatrix.GetLength(1)];

            while (true)
            {
                int sum = 0;

                // o(m^2) where m < n
                creatingWindow(window, imageMatrix, t, ref rowflag, ref colflag, ref shifter);

                // n
                if (method == 1)
                {
                    // order of m+k where k = 255 so, order of m
                    countingSort(window);

                    // order of m
                    for (int i = 0; i < window.Length - 2 * t; i++)
                    {
                        sum += window[i + t];
                    }
                }
                else if (method == 2)
                {
                    // m square
                    for (int i = 0; i < t; i++)
                        kth(window);
                    for (int i = 0; i < window.Length; i++)
                        sum += window[i];
                }


                int newPixel = sum / window.Length - 2 * t;
                newImage[rowflag + t / 2, shifter] = (byte)newPixel;

                if (colflag == (imageMatrix.GetLength(1) - 1) && rowflag == imageMatrix.GetLength(0) - t)
                {
                    return newImage;

                }
                else if (colflag == (imageMatrix.GetLength(1) - 1))
                {
                    colflag = 0;
                    shifter = 0;
                    rowflag++;
                }

            }


        }

        // n3 log n
        public void Adeptive_median(int Ws, int Size, byte[,] imageMatrix, int t, int method)
        {

            int rowflag = 0;
            int colflag = 0;
            int shifter = 0;
            int Zmin, Zmax, Zmed, Zxy;
            int[] window = new int[Size];
            //byte[,] newImage = new byte[imageMatrix.GetLength(0), imageMatrix.GetLength(1)];

            while (true)
            {
                creatingWindow(window, imageMatrix, t, ref rowflag, ref colflag, ref shifter);


                if (method == 0)
                    countingSort(window);
                //nlogon
                else if (method == 1)
                    quickSort(window, 0, window.Length - 1);

                Zmin = window[0];
                Zmax = window[window.Length - 1];
                Zmed = window[(window.Length + 1) / 2];
                Zxy = imageMatrix[rowflag + t / 2, shifter];

                int A1 = Zmed - Zmin;
                int A2 = Zmax - Zmed;
                int newpixelVal = 0;
                if (A1 > 0 && A2 > 0)
                {
                    int B1 = Zxy - Zmin;
                    int B2 = Zmax - Zxy;
                    if (B1 > 0 && B2 > 0)
                    {
                        newpixelVal = Zxy;
                    }
                    else
                    {
                        newpixelVal = Zmed;
                    }
                }
                else
                {
                    if (t + 2 <= Ws)
                    {
                        if (colflag == (imageMatrix.GetLength(1) - 2))
                        {
                            shifter--;
                        }
                        else if (colflag == (imageMatrix.GetLength(1) - 1))
                        {
                            shifter -= 2;
                        }

                        if (rowflag == imageMatrix.GetLength(0) - t)
                        {
                            rowflag -= 2;
                        }
                        else if (rowflag == imageMatrix.GetLength(0) - t - 1)
                        {
                            rowflag -= 1;
                        }
                        t = t + 2;
                        window = new int[t * t];
                        continue;
                    }
                    else
                    {
                        newpixelVal = Zmed;
                    }
                }

                //newImage[rowflag + t / 2, shifter] = (byte)newpixelVal;
                imageMatrix[rowflag + t / 2, shifter] = (byte)newpixelVal;

                if (colflag == (imageMatrix.GetLength(1) - 1) && rowflag == imageMatrix.GetLength(0) - t)
                {
                    break;
                }
                else if (colflag == (imageMatrix.GetLength(1) - 1))
                {
                    colflag = 0;
                    shifter = 0;
                    rowflag++;
                }
            }
        }
    }


}
