using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IFAFolderReader
    {
        //todo: update folders summery
        public Task<FAFolderUpdateReport> UpdateFolders(string folderPath);

        /// <summary>
        /// test method to see if i can gather all the folders from a master folder
        /// </summary>
        /// <param name="folderPath">the path to the masterfolder</param>
        /// <returns>a list of strings with the folder names</returns>
        public List<string> GetFoldersWithItems(string folderPath);

        /// <summary>
        /// creates a List of Folders from the given path that have Name, RelativePath, and HasItems correctly set, and ParentID with no reference to previous systems.
        /// </summary>
        /// <param name="folderPath">the path from where folders need to be read</param>
        /// <returns>a List of Folders</returns>
        public List<FAFolder> GetFolders(string folderPath);
    }
}
