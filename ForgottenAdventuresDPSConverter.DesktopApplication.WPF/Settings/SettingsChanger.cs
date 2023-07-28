using ForgottenAdventuresDPSConverter.FileRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Settings
{
    public static class SettingsChanger
    {
        public static bool ChangeRepositoryDirectory(IFileRepositorySettings fileRepositorySettings, string newDirectory)
        {
            if (!newDirectory.EndsWith('\\'))
            {
                newDirectory += '\\';
            }
            if (!Directory.Exists(newDirectory))
            {
                return false; //if the new given directory does not exist don't try to change it.
            }

            string oldDirectory = Properties.Settings.Default.RepositoryDirectoryPath;

            Properties.Settings.Default.RepositoryDirectoryPath = newDirectory;
            Properties.Settings.Default.Save();

            if (File.Exists(fileRepositorySettings.FAFolderRepositoryFilePath) && 
                File.Exists(fileRepositorySettings.DpsFolderRepositoryFilePath) && 
                File.Exists(fileRepositorySettings.DpsNumberRepositoryFilePath) && 
                File.Exists(fileRepositorySettings.DpsSubfolderRepositoryFilePath))
            {
                //the files must all exist or cancel changing
                return true;
            }
            else
            {
                Properties.Settings.Default.RepositoryDirectoryPath = oldDirectory;
                Properties.Settings.Default.Save();
                return false;
            }
        }

        public static bool ChangeRepositoryDirectoryAndMoveFiles(IFileRepositorySettings fileRepositorySettings, string newDirectory)
        {
            if (!newDirectory.EndsWith('\\'))
            {
                newDirectory += '\\';
            }
            if (!Directory.Exists(newDirectory))
            {
                return false; //if the new given directory does not exist don't try to change it.
            }

            string oldDirectory = fileRepositorySettings.DirectoryPath;
            string oldFAFolderRepositoryFilePath = fileRepositorySettings.FAFolderRepositoryFilePath;
            string oldDpsFolderRepositoryFilePath = fileRepositorySettings.DpsFolderRepositoryFilePath;
            string oldDpsNumberRepositoryFilePath = fileRepositorySettings.DpsNumberRepositoryFilePath;
            string oldDpsSubfolderRepositoryFilePath = fileRepositorySettings.DpsSubfolderRepositoryFilePath;

            Properties.Settings.Default.RepositoryDirectoryPath = newDirectory;
            Properties.Settings.Default.Save();

            string newFAFolderRepositoryFilePath = fileRepositorySettings.FAFolderRepositoryFilePath;
            string newDpsFolderRepositoryFilePath = fileRepositorySettings.DpsFolderRepositoryFilePath;
            string newDpsNumberRepositoryFilePath = fileRepositorySettings.DpsNumberRepositoryFilePath;
            string newDpsSubfolderRepositoryFilePath = fileRepositorySettings.DpsSubfolderRepositoryFilePath;

            if(File.Exists(newFAFolderRepositoryFilePath) || File.Exists(newDpsFolderRepositoryFilePath) || File.Exists(newDpsNumberRepositoryFilePath) || File.Exists(newDpsSubfolderRepositoryFilePath))
            {
                //if files already exist then undo the change of directory and return false
                Properties.Settings.Default.RepositoryDirectoryPath = oldDirectory;
                Properties.Settings.Default.Save();
                return false;
            }

            bool copyTasksSuccessful = true;
            Task FAFolderRepositoryCopyTask = Task.Run(() => copyTasksSuccessful = copyTasksSuccessful && copyFile(oldFAFolderRepositoryFilePath, newFAFolderRepositoryFilePath));
            Task DpsFolderRepositoryCopyTask = Task.Run(() => copyTasksSuccessful = copyTasksSuccessful && copyFile(oldDpsFolderRepositoryFilePath, newDpsFolderRepositoryFilePath));
            Task DpsNumberRepositoryCopyTask = Task.Run(() => copyTasksSuccessful = copyTasksSuccessful && copyFile(oldDpsNumberRepositoryFilePath, newDpsNumberRepositoryFilePath));
            Task DpsSubfolderRepositoryCopyTask = Task.Run(() => copyTasksSuccessful = copyTasksSuccessful && copyFile(oldDpsSubfolderRepositoryFilePath, newDpsSubfolderRepositoryFilePath));
            FAFolderRepositoryCopyTask.Wait();
            DpsFolderRepositoryCopyTask.Wait();
            DpsNumberRepositoryCopyTask.Wait();
            DpsSubfolderRepositoryCopyTask.Wait();

            if (copyTasksSuccessful)
            {
                File.Delete(oldFAFolderRepositoryFilePath);
                File.Delete(oldDpsFolderRepositoryFilePath);
                File.Delete(oldDpsNumberRepositoryFilePath);
                File.Delete(oldDpsSubfolderRepositoryFilePath);
            }
            else
            {
                File.Delete(newFAFolderRepositoryFilePath);
                File.Delete(newDpsFolderRepositoryFilePath);
                File.Delete(newDpsNumberRepositoryFilePath);
                File.Delete(newDpsSubfolderRepositoryFilePath);
            }
            return copyTasksSuccessful;

            bool copyFile(string oldFilePath, string newFilePath)
            {
                bool successful = false;
                StreamReader reader = new(oldFilePath);
                StreamWriter writer = File.CreateText(newFilePath);
                string? line;
                try
                {
                    line = reader.ReadLine();
                    while(line != null)
                    {
                        writer.WriteLine(line);
                        line = reader.ReadLine();
                    }
                    successful = true;
                }
                finally
                {
                    reader.Dispose();
                    writer.Dispose();
                }
                return successful;
            }
        }

        internal static bool ChangeDownloadFolderPath(string newDownloadFolderPath)
        {
            if (!newDownloadFolderPath.EndsWith('\\'))
            {
                newDownloadFolderPath += '\\';
            }
            if (!Directory.Exists(newDownloadFolderPath))
            {
                return false; //if the new given directory does not exist don't try to change it.
            }

            Properties.Settings.Default.FADownloadFolderPath = newDownloadFolderPath;
            Properties.Settings.Default.Save();

            return true;
        }
    }
}
