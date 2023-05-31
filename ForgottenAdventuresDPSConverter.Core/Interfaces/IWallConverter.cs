using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IWallConverter
    {
        /// <summary>
        /// this converts an image into a format that dungeon painter studio can better use as a wall by removing as many pixels as possible from above and below the actually filled in image.
        /// </summary>
        /// <param name="filePath">the path to the file to convert</param>
        /// <param name="wallPath">the name and path to where the new image is to be stored</param>
        /// <returns>a task returns true is the file was successfully converted, and false if it wasn't</returns>
        public Task<bool> ConvertWall(string filePath, string wallPath);
    }
}
