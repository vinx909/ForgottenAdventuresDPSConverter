using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Services
{
    public class ConverterService : IConverterService
    {
        private const char nextFolderSign = '\\';
        private const char dpsFolderBreakChar = '.';
        private const string floorsFolder = "floors";
        private const string objectsFolder = "objects";
        private const string wallsFolder = "walls";
        private const string floorInFolderName = "img.png";
        private const string expectedFileType = ".png";
        private readonly string[] possibleFileTypes = new string[] { ".png", ".jpg" };

        private readonly IFAFolderService fAFolderService;
        private readonly IDpsNumberService dpsNumberService;
        private readonly IDpsFolderService dpsFolderService;
        private readonly IDpsSubfolderService dpsSubfolderService;
        private readonly ICommandsService commandsService;

        public ConverterService(IFAFolderService fAFolderService, IDpsNumberService dpsNumberService, IDpsFolderService dpsFolderService, IDpsSubfolderService dpsSubfolderService, ICommandsService commandsService)
        {
            this.fAFolderService = fAFolderService;
            this.dpsNumberService = dpsNumberService;
            this.dpsFolderService = dpsFolderService;
            this.dpsSubfolderService = dpsSubfolderService;
            this.commandsService = commandsService;
        }

        public Task ConvertToDpsFolders(string SourceDirectory, string TargetDirectory)
        {
            if(SourceDirectory == null || TargetDirectory == null || !Directory.Exists(SourceDirectory) || !Directory.Exists(TargetDirectory))
            {
                const string nextLine = "\r\n";

                bool argumentException = false;
                string ExceptionMessage = string.Empty;

                if(SourceDirectory == null)
                {
                    argumentException = true;
                    ExceptionMessage = "the source directory was not set";
                }
                if (TargetDirectory == null)
                {
                    argumentException = true;
                    if (!string.IsNullOrEmpty(ExceptionMessage))
                    {
                        ExceptionMessage += nextLine;
                    }
                    ExceptionMessage = "the target directory was not set";
                }
                if (SourceDirectory != null && !Directory.Exists(SourceDirectory))
                {
                    if (!string.IsNullOrEmpty(ExceptionMessage))
                    {
                        ExceptionMessage += nextLine;
                    }
                    ExceptionMessage = "the source directory of " + SourceDirectory + " does not exist";
                }
                if (TargetDirectory != null && !Directory.Exists(TargetDirectory))
                {
                    if (!string.IsNullOrEmpty(ExceptionMessage))
                    {
                        ExceptionMessage += nextLine;
                    }
                    ExceptionMessage = "the target directory of " + TargetDirectory + " does not exist";
                }

                if (argumentException)
                {
                    throw new ArgumentException(ExceptionMessage);
                }
                else
                {
                    throw new DirectoryNotFoundException(ExceptionMessage);
                }
            }

            if (SourceDirectory.EndsWith('\\'))
            {
                SourceDirectory = SourceDirectory.Substring(0, SourceDirectory.Length - 1);
            }
            if (TargetDirectory.EndsWith('\\'))
            {
                TargetDirectory = TargetDirectory.Substring(0, TargetDirectory.Length - 1);
            }

            foreach (FAFolder folder in fAFolderService.GetAll().Result)
            {
                if(folder != null && Directory.Exists(SourceDirectory + folder.RelativePath) && folder.HasItems && folder.FolderId != null && folder.SubfolderId != null && folder.NumberId != null) //if the folder exists, has items and folderId, subfolderId, and NumberId are set
                {

                    string DpsFolderDirectoryBase = TargetDirectory + nextFolderSign
                                                + dpsNumberService.Get((int)folder.NumberId).Result.Number + dpsFolderBreakChar
                                                + dpsFolderService.Get((int)folder.FolderId).Result.NameAbriviation + dpsFolderBreakChar
                                                + dpsSubfolderService.Get((int)folder.SubfolderId).Result.Name; // get the folder name from it's parts
                    string DpsFolderDirectory = string.Empty;
                    if (folder.IsFloor)
                    {
                        DpsFolderDirectory += DpsFolderDirectoryBase + nextFolderSign + floorsFolder;
                    } // if it's a floor add it to the floor folder
                    else
                    {
                        DpsFolderDirectory += DpsFolderDirectoryBase + nextFolderSign + objectsFolder;
                    } // otherwise add it to the objects folder

                    bool doDefaultCopyOver = true;

                    if (folder.OtherCommands)
                    {
                        if (commandsService.HasWallCommand(folder))
                        {
                            List<CommandWall> wallCommands = commandsService.WallCommandGetElements(folder);

                            string DpsFolderDirectoryWall = DpsFolderDirectoryBase + nextFolderSign + wallsFolder;
                            CheckForOrCreateDirectory(DpsFolderDirectoryWall);

                            foreach(string path in Directory.GetFiles(SourceDirectory + folder.RelativePath))
                            {
                                foreach(var wallCommand in wallCommands)
                                {
                                    if (string.Equals(path.Split(nextFolderSign).Last(), wallCommand.ImageName))
                                    {
                                        string newName = DpsFolderDirectory + nextFolderSign + ConvertImageName(wallCommand.ImageName);

                                        if (File.Exists(newName))
                                        {
                                            File.Delete(newName);
                                        }

                                        using (Bitmap origional = new(path))
                                        {
                                            int width = origional.Width;
                                            int height = origional.Height;

                                            //make sure the trims actually fit within the image
                                            int leftTrim = Math.Min(wallCommand.LeftTrim, width - 1);
                                            int rightTrim = Math.Min(wallCommand.RightTrim, height - 1 - leftTrim);
                                            int topBottomTrim = Math.Min(wallCommand.TopBottomTrim, (height - 1) / 2);

                                            int trimmedWidth = width - leftTrim - rightTrim;
                                            int trimmedHeight = height - topBottomTrim * 2;

                                            using (Bitmap trimmed = new(trimmedWidth, trimmedHeight))
                                            {
                                                using (Graphics g = Graphics.FromImage(trimmed))
                                                {
                                                    Rectangle sourceRect = new(leftTrim, topBottomTrim, trimmedWidth, trimmedHeight);
                                                    Rectangle destinationRect = new(0, 0, trimmedWidth, trimmedHeight);

                                                    g.DrawImage(origional, destinationRect, sourceRect, GraphicsUnit.Pixel);
                                                }

                                                trimmed.Save(newName);
                                            }
                                        }

                                        break;
                                    }
                                }
                            }

                            if(Directory.GetFiles(DpsFolderDirectoryWall).Length == 0)
                            {
                                Directory.Delete(DpsFolderDirectoryWall);
                            }//if at the end no folders were created delete the directory
                        }
                        if (commandsService.HasSeperatorCommand(folder))
                        {
                            doDefaultCopyOver = false; //since the elements are getting seperated set doDefaultCopyOver to true so things are not copied over both seperated and all together

                            bool replaceName = commandsService.SeperateCommandGetReplaceInsteadOfAddName(folder); //find out if the inner folder name is to be replaced or just added to
                            List<CommandSeparatorElement> separatorElements = commandsService.SeperateCommandGetElements(folder);
                            List<string> excludedSearchTerms = new();
                            for (int i = 0; i < separatorElements.Count; i++)
                            {
                                string ElementDpsFolderDirectory = DpsFolderDirectory; //create the path that items will be copied over to
                                if (replaceName)
                                {
                                    ElementDpsFolderDirectory += nextFolderSign + CorrectStringForFolderPlacement(separatorElements[i].SeperatorName);
                                }
                                else
                                {
                                    ElementDpsFolderDirectory += nextFolderSign + GetInnerFolder(folder) + ' ' + CorrectStringForFolderPlacement(separatorElements[i].SeperatorName);
                                }

                                CheckForOrCreateDirectory(ElementDpsFolderDirectory);

                                foreach (string path in Directory.GetFiles(SourceDirectory + folder.RelativePath))
                                {
                                    string imageName = ConvertImageName(path.Split(nextFolderSign).Last());

                                    if (string.IsNullOrWhiteSpace(separatorElements[i].SearchTerm) || imageName.Contains(separatorElements[i].SearchTerm))
                                    {
                                        bool inPreviousGrouping = false;
                                        foreach (string excludedSearchTerm in excludedSearchTerms)
                                        {
                                            if (imageName.Contains(excludedSearchTerm))
                                            {
                                                inPreviousGrouping = true;
                                                break;
                                            }
                                        }
                                        if(inPreviousGrouping == false)
                                        {
                                            string newName = ElementDpsFolderDirectory + nextFolderSign + imageName;

                                            //todo: maybe copy over the not doubling up on floors, though probably unnessisary

                                            if (File.Exists(newName))
                                            {
                                                File.Delete(newName);
                                            }
                                            File.Copy(path, newName);
                                        }
                                    }
                                }

                                if(Directory.GetFiles(ElementDpsFolderDirectory).Length == 0) 
                                {
                                    Directory.Delete(ElementDpsFolderDirectory);
                                }//if a directory is empty delete it

                                if (string.IsNullOrWhiteSpace(separatorElements[i].SearchTerm))
                                {
                                    break;
                                } //if the search term was null, empty or white spaces all remaining files were added to it, so so keep looking would be useless
                                else
                                {
                                    excludedSearchTerms.Add(separatorElements[i].SearchTerm);
                                } //if it wasn't null, empty of white spaces add it to the search terms to avoid for future elements
                            }
                        }
                    }

                    if (doDefaultCopyOver)
                    {
                        DpsFolderDirectory += nextFolderSign + GetInnerFolder(folder); //get the inner folder name;

                        CheckForOrCreateDirectory(DpsFolderDirectory);

                        foreach (string path in Directory.GetFiles(SourceDirectory + folder.RelativePath))
                        {

                            string newName = DpsFolderDirectory + nextFolderSign + ConvertImageName(path.Split(nextFolderSign).Last());

                            #region avoid doubling up on floors
                            //explination: dps does something intersting with floor textures. to make the program faster it makes a folder with the origional texture as well as a preview image. this means the default method of checking doesn't work. and i don't want to always remove these folders as they must be rebuild each time and otherwise the textures don't work. so instead if it's floors i'll check if the img.png and the new file have the same content and if they do this file will be skipped.
                            if (folder.IsFloor) //if it is of the type floor
                            {
                                string directoryPath = String.Empty;
                                foreach (string fileType in possibleFileTypes)
                                {
                                    if (newName.EndsWith(fileType))
                                    {
                                        directoryPath = newName.Remove(newName.Length - fileType.Length);
                                        break;
                                    }
                                }
                                //try to figure out what the directories path would be by substracting the extionsion

                                if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))//if the directory path can be figured out and it exists
                                {
                                    if (path.EndsWith(expectedFileType) && File.Exists(directoryPath + nextFolderSign + floorInFolderName))
                                    {
                                        bool samefile = FileCompare(path, directoryPath + nextFolderSign + floorInFolderName);
                                    }
                                    continue;
                                    //todo: a better way of skipping these files, maybe see if i can convert jpg images to png images and maybe improve the comparing abilities so i can compare the content of the files. currently it just skips any textures that have folders generated which means texture folders have to be manually deleted to replace them. or just a toggle.
#if false
                                if (path.EndsWith(expectedFileType) && File.Exists(directoryPath + nextFolderSign + floorInFolderName) && FileCompare(path, directoryPath + nextFolderSign + floorInFolderName)) //if the path ends with the expected extension .png, and the directory with the expected file name contains a file img.png, and the content of the new file and the old img.png are the same skip this file
                                {
                                    continue;
                                }
                                else
                                {
                                    Directory.Delete(directoryPath, true);
                                }
#endif
                                }
                            }
                            #endregion

                            if (File.Exists(newName))
                            {
                                File.Delete(newName);
                            }
                            File.Copy(path, newName);
                        }
                    }
                }
            }

            return Task.CompletedTask;

            
        }

        private static void CheckForOrCreateDirectory(string DpsFolderDirectory)
        {
            Directory.CreateDirectory(DpsFolderDirectory); //create the directory if it didn't already exist.
            if (!Directory.Exists(DpsFolderDirectory))
            {
                throw new DirectoryNotFoundException("the directory " + DpsFolderDirectory + "does not exist and failed to be created");
            }//make sure the directory exists
        }

        private string GetInnerFolder(FAFolder folder)
        {

            if (folder.CustomInnerFolderName)
            {
                return CorrectStringForFolderPlacement(folder.CustomInnerFolderNameString);
            }
            else if (folder.InParentInnerFolder)
            {
                return GetInnerFolder(fAFolderService.Get((int)folder.ParentId).Result);
            }
            else
            {
                return CorrectStringForFolderPlacement(folder.Name);
            }
        }

        private static string CorrectStringForFolderPlacement(string toCheckString)
        {
            const char toReplace = '_';
            const char replaceWith = ' ';
            const char mustNotStartWith = ' ';

            toCheckString = toCheckString.Replace(toReplace, replaceWith);

            while (toCheckString.StartsWith(mustNotStartWith))
            {
                toCheckString=toCheckString.Substring(1);
            }

            return toCheckString;
        }

        public string ConvertImageName(string Name)
        {
            return Name.Replace('_', ' ');
        }

#region compare files
        // method taken from https://learn.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/create-file-compare because i was a bit lazy
        // This method accepts two strings the represent two files to
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the
        // files are not the same.
        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1byte - file2byte) == 0);
        }
#endregion
    }
}
