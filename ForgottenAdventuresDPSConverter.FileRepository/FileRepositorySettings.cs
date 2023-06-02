using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
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
        private const string DpsFolderRepositoryFileName = "Temp_ForgottenAdventuresDPSConverter_File_Repository_DpsFolders.fac";
        private const string DpsNumbersRepositoryFileName = "Temp_ForgottenAdventuresDPSConverter_File_Repository_DpsNumbers.fac";
        private const string DpsSubfolderRepositoryFileName = "Temp_ForgottenAdventuresDPSConverter_File_Repository_DpsSubfolders.fac";
        private const string FAFolderRepositoryFileName = "Temp_ForgottenAdventuresDPSConverter_File_Repository_FAFolders.fac";

        public string DpsFolderRepositoryFilePath => directoryPath + DpsFolderRepositoryFileName;

        public string DpsNumberRepositoryFilePath => directoryPath + DpsNumbersRepositoryFileName;

        public string DpsSubfolderRepositoryFilePath => directoryPath + DpsSubfolderRepositoryFileName;

        public string FAFolderRepositoryFilePath => directoryPath + FAFolderRepositoryFileName;

        public Repository<DpsFolder> DpsFolderRepository => new DpsFolderRepository(this);

        public Repository<DpsNumber> DpsNumberRepository => new DpsNumberRepository(this);

        public Repository<DpsSubfolder> DpsSubfolderRepository => new DpsSubfolderRepository(this);

        public Repository<FAFolder> FAFolderRepository => new FAFolderRepository(this);
    }
}
