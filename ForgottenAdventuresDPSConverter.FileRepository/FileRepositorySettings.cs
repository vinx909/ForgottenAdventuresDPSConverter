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
        public virtual string DirectoryPath => directoryPath;

        private const string DpsFolderRepositoryFileName = "ForgottenAdventuresDPSConverter_File_Repository_DpsFolders.fac";
        private const string DpsNumbersRepositoryFileName = "ForgottenAdventuresDPSConverter_File_Repository_DpsNumbers.fac";
        private const string DpsSubfolderRepositoryFileName = "ForgottenAdventuresDPSConverter_File_Repository_DpsSubfolders.fac";
        private const string FAFolderRepositoryFileName = "ForgottenAdventuresDPSConverter_File_Repository_FAFolders.fac";

        private readonly IServiceProvider serviceProvider;

        public FileRepositorySettings(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string DpsFolderRepositoryFilePath => DirectoryPath + DpsFolderRepositoryFileName;

        public string DpsNumberRepositoryFilePath => DirectoryPath + DpsNumbersRepositoryFileName;

        public string DpsSubfolderRepositoryFilePath => DirectoryPath + DpsSubfolderRepositoryFileName;

        public string FAFolderRepositoryFilePath => DirectoryPath + FAFolderRepositoryFileName;

        public FileRepository<DpsNumber> DpsFolderRepository
        {
            get
            {
                FileRepository<DpsNumber>? repository = serviceProvider.GetService(typeof(IRepository<DpsNumber>)) as FileRepository<DpsNumber>;
                if (repository == null)
                {
                    throw new NotSupportedException("if you use ForgottenAdventuresDpsConverter.FileRepository as the repository then all repositories must be from there and the service IRepository<DpsFolder> must provide a type of Repository<DpsFolder>");
                }
                else
                {
                    return repository;
                }
            }
        }

        public FileRepository<DpsNumber> DpsNumberRepository
        {
            get
            {
                FileRepository<DpsNumber>? repository = serviceProvider.GetService(typeof(IRepository<DpsNumber>)) as FileRepository<DpsNumber>;
                if (repository == null)
                {
                    throw new NotSupportedException("if you use ForgottenAdventuresDpsConverter.FileRepository as the repository then all repositories must be from there and the service IRepository<DpsNumber> must provide a type of Repository<DpsNumber>");
                }
                else
                {
                    return repository;
                }
            }
        }

        public FileRepository<DpsFolder> DpsSubfolderRepository
        {
            get
            {
                FileRepository<DpsFolder>? repository = serviceProvider.GetService(typeof(IRepository<DpsFolder>)) as FileRepository<DpsFolder>;
                if (repository == null)
                {
                    throw new NotSupportedException("if you use ForgottenAdventuresDpsConverter.FileRepository as the repository then all repositories must be from there and the service IRepository<DpsSubfolder> must provide a type of Repository<DpsSubfolder>");
                }
                else
                {
                    return repository;
                }
            }
        }

        public FileRepository<FAFolder> FAFolderRepository
        {
            get
            {
                FileRepository<FAFolder>? repository = serviceProvider.GetService(typeof(IRepository<FAFolder>)) as FileRepository<FAFolder>;
                if (repository == null)
                {
                    throw new NotSupportedException("if you use ForgottenAdventuresDpsConverter.FileRepository as the repository then all repositories must be from there and the service IRepository<FAFolder> must provide a type of Repository<FAFolder>");
                }
                else
                {
                    return repository;
                }
            }
        }
    }
}
