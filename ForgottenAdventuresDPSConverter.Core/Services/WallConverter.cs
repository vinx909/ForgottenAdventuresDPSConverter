using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Services
{
    public class WallConverter : IWallConverter
    {
        public async Task<bool> ConvertWall(string filePath, string wallPath)
        {
            return await Task.Run(() => ConverWallTopBottemAndEmptyLeftRight(filePath, wallPath));
        }

        private bool ConverWallOnlyTopBottem(string filePath, string wallPath)
        {
            // largely stolen from https://stackoverflow.com/a/17409449/16390053
            
            Bitmap source = new Bitmap(filePath);
            int sourceHeight = source.Height;
            int x = 0;
            int y = 0;//to figure out
            int width = source.Width;
            int height = sourceHeight;//to figure out

            bool isEmpty = true;
            for (int row = 0; row < sourceHeight/2 && isEmpty; row++) //for each row up to the middle
            {
                for (int column = 0; column < width; column++)//for each column
                {
                    if (source.GetPixel(column, row).A != 0 && source.GetPixel(column, sourceHeight - 1 - row).A != 0) //check each pixel on the row and the inverse (so first and last, second and second to last etc.) if the alfa is zero, if it isn't set isEmpty to false;
                    {
                        isEmpty = false;
                    }
                }

                if (isEmpty == false)
                {
                    y = row;
                    height = sourceHeight - row*2;
                }
            }
            if (isEmpty == true) //if it's empty after going over every pixel in the image it means the entire image is transparent and thus there's nothing that would remain of the image;
            {
                return false;
            }

            Bitmap CroppedImage = source.Clone(new System.Drawing.Rectangle(x, y, width, height), source.PixelFormat);

            try
            {
                CroppedImage.Save(wallPath, ImageFormat.Png);
                return true;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                return false;
            }
            
        }

        private bool ConverWallTopBottemAndEmptyLeftRight(string filePath, string wallPath)
        {
            // largely stolen from https://stackoverflow.com/a/17409449/16390053

            if(filePath == "F:\\Games\\Steam\\steamapps\\common\\Dungeon Painter Studio\\data\\collections\\.1ST.Wall\\objects\\Stone I\\Wall Stone Earthy I1 Straight B 2x1 400x200.png")
            {
                Console.WriteLine("breakpoint reached");
            }

            Bitmap source = new Bitmap(filePath);
            int sourceHeight = source.Height;
            int x = 0;//to figure out
            int y = 0;//to figure out
            int width = source.Width;
            int height = sourceHeight;//to figure out

            //determine how much needs to go off the top and bottom
            bool isEmpty = true;
            for (int row = 0; row < sourceHeight / 2 && isEmpty; row++) //for each row up to the middle
            {
                for (int column = 0; column < width; column++)//for each column
                {
                    if (source.GetPixel(column, row).A != 0) //check each pixel on the rows on the top if the alfa is zero, if it isn't set the y and height and isEmpty to false;
                    {
                        isEmpty = false;
                        y = row;
                        height = sourceHeight - row * 2;
                        break;
                    }
                }

                for (int column = 0; column < width; column++)//for each column
                {
                    if (source.GetPixel(column, sourceHeight - 1 - row).A != 0) //check each pixel on the rows on the bottem if the alfa is zero, if it isn't set the y and height and isEmpty to false;
                    {
                        isEmpty = false;
                        row = Math.Max(row - 1, 0);
                        y = row;
                        height = sourceHeight - (row) * 2;
                        break;
                    }
                }
            }
            if (isEmpty == true) //if it's empty after going over every pixel in the image it means the entire image is transparent and thus there's nothing that would remain of the image;
            {
                return false;
            }

            //determine how much needs to go off the left side
            isEmpty = true;
            
            for (int column = 0; column < width && isEmpty; column++)//for each column
            {
                for (int row = y; row < y + height; row++) //for each row up to the middle
                {
                    if (source.GetPixel(column, row).A != 0) //check each pixel on the column if the alfa is zero, if it isn't set x and isEmpty to false;
                    {
                        isEmpty = false;
                        x = column;
                        break;
                    }
                }
            }

            //determine how much needs to go off the right side
            isEmpty = true;

            for (int column = width - 1; column >= x && isEmpty; column--)//for each column
            {
                for (int row = y; row < y + height; row++) //for each row up to the middle
                {
                    if (source.GetPixel(column, row).A != 0) //check each pixel on the column if the alfa is zero, if it isn't set width and isEmpty to false;
                    {
                        isEmpty = false;
                        width = column + 1;
                        break;
                    }
                }
            }

            Bitmap CroppedImage = source.Clone(new System.Drawing.Rectangle(x, y, width, height), source.PixelFormat);

            try
            {
                CroppedImage.Save(wallPath, ImageFormat.Png);
                return true;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                return false;
            }

        }

        private bool ConverWallTopBottemAndDifferenceLeftRight(string filePath, string wallPath)
        {
            // largely stolen from https://stackoverflow.com/a/17409449/16390053

            if (filePath == "F:\\Games\\Steam\\steamapps\\common\\Dungeon Painter Studio\\data\\collections\\.1ST.Wall\\objects\\Stone I\\Wall Stone Earthy I1 Straight B 2x1 400x200.png")
            {
                Console.WriteLine("breakpoint reached");
            }

            Bitmap source = new Bitmap(filePath);
            int sourceHeight = source.Height;
            int x = 0;//to figure out
            int y = 0;//to figure out
            int width = source.Width;
            int height = sourceHeight;//to figure out

            //determine how much needs to go off the top and bottom
            bool isEmpty = true;
            for (int row = 0; row < sourceHeight / 2 && isEmpty; row++) //for each row up to the middle
            {
                for (int column = 0; column < width; column++)//for each column
                {
                    if (source.GetPixel(column, row).A != 0) //check each pixel on the rows on the top if the alfa is zero, if it isn't set the y and height and isEmpty to false;
                    {
                        isEmpty = false;
                        y = row;
                        height = sourceHeight - row * 2;
                        break;
                    }
                }

                for (int column = 0; column < width; column++)//for each column
                {
                    if (source.GetPixel(column, sourceHeight - 1 - row).A != 0) //check each pixel on the rows on the bottem if the alfa is zero, if it isn't set the y and height and isEmpty to false;
                    {
                        isEmpty = false;
                        row = Math.Max(row - 1, 0);
                        y = row;
                        height = sourceHeight - (row) * 2;
                        break;
                    }
                }
            }
            if (isEmpty == true) //if it's empty after going over every pixel in the image it means the entire image is transparent and thus there's nothing that would remain of the image;
            {
                return false;
            }

            //determine how much needs to go off the left side
            isEmpty = true;

            for (int column = 0; column < width && isEmpty; column++)//for each column
            {
                for (int row = y; row < y + height; row++) //for each row up to the middle
                {
                    if (source.GetPixel(column, row).A != 0) //check each pixel on the column if the alfa is zero, if it isn't set x and isEmpty to false;
                    {
                        isEmpty = false;
                        x = column;
                        break;
                    }
                }
            }

            //determine how much needs to go off the right side
            isEmpty = true;

            for (int column = width - 1; column >= x && isEmpty; column--)//for each column
            {
                for (int row = y; row < y + height; row++) //for each row up to the middle
                {
                    if (source.GetPixel(column, row).A != 0) //check each pixel on the column if the alfa is zero, if it isn't set width and isEmpty to false;
                    {
                        isEmpty = false;
                        width = column + 1;
                        break;
                    }
                }
            }

            Bitmap CroppedImage = source.Clone(new System.Drawing.Rectangle(x, y, width, height), source.PixelFormat);

            try
            {
                CroppedImage.Save(wallPath, ImageFormat.Png);
                return true;
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                return false;
            }

        }

        private bool PixelSignificantlyDifferent(Color pixelA, Color pixelB)
        {
            const int ASignificantDifference = 20;
            const int RGBSignificantDifference = 60;

            bool isDifferent = false;
            int ADifference = Math.Abs(pixelA.A - pixelB.A);
            if(ADifference > ASignificantDifference)
            {
                isDifferent = true;
            }
            int RDifference = Math.Abs(pixelA.R - pixelB.R);
            if(RDifference > RGBSignificantDifference)
            {
                isDifferent = true;
            }
            int GDifference = Math.Abs(pixelA.G - pixelB.G);
            if(GDifference > RGBSignificantDifference)
            {
                isDifferent |= true;
            }
            int BDifference = Math.Abs(pixelA.B - pixelB.B);
            if(BDifference > RGBSignificantDifference)
            {
                isDifferent &= true;
            }
            return isDifferent;
        }
    }
}
