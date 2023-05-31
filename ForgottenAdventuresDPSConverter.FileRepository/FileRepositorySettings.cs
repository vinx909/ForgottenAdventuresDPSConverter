using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.FileRepository
{
    public class FileRepositorySettings : IFileRepositorySettings
    {
        private const string directoryPath = @"C:\Users\Octavia\Desktop\";
        private const string FAFolderRepositoryFileName = "Temp_ForgottenAdventuresDPSConverter_File_Repository_FAFolders.fac";

        public string FAFolderRepositoryFilePath => directoryPath+FAFolderRepositoryFileName;
    }
}
