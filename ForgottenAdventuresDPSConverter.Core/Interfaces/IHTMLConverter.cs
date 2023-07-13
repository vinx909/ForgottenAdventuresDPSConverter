using ForgottenAdventuresDPSConverter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IHtmlConverter
    {
        /// <summary>
        /// makes a file with the HTML to make a table based on the folders read from the FA mapmaking pack or similar
        /// </summary>
        /// <param name="folders">a list of folders to make the table out of</param>
        /// <param name="targetPath">the location the HTML file will be made</param>
        public void FAHtmlConvert(List<FAFolder> folders, string targetPath);
    }
}
