using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels
{
    public class WallCommandsViewModel : BaseViewModel, IDisposable
    {
        const char nextFolderChar = '\\';

        private readonly CommandViewModel commandViewModel;
        private readonly ICommandsService commandsService;

        private readonly ISettingsGetter settings;

        private string exampleImagePath;
        private string path;
        private string fileName;
        private int topBottomTrim;
        private int leftTrim;
        private int rightTrim;

        private readonly List<string> toDeleteFiles;

        public string ExampleImagePath
        {
            get => exampleImagePath;
            set
            {
                if (value != exampleImagePath)
                {
                    exampleImagePath = value;
                    RaiseProppertyChanged();
                }
            }
        }

        public string Path
        {
            get => path;
            private set
            {
                if (value != path)
                {
                    path = value;
                    RaiseProppertyChanged();
                }
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                if (value != fileName)
                {
                    fileName = value;
                    RaiseProppertyChanged();
                }
            }
        }

        public int TopBottomTrim
        {
            get => topBottomTrim;
            set
            {
                if (value != topBottomTrim && value >= 0)
                {
                    topBottomTrim = value;
                    RaiseProppertyChanged();
                }
            }
        }

        public int LeftTrim
        {
            get => leftTrim;
            set
            {
                if (value != leftTrim && value >= 0)
                {
                    leftTrim = value;
                    RaiseProppertyChanged();
                }
            }
        }

        public int RightTrim
        {
            get => rightTrim;
            set
            {
                if (value != rightTrim && value >= 0)
                {
                    rightTrim = value;
                    RaiseProppertyChanged();
                }
            }
        }

        public WallCommandsViewModel()
        {
            toDeleteFiles = new List<string>();
        }

        public WallCommandsViewModel(CommandViewModel commandViewModel, ICommandsService commandsService, ISettingsGetter settings) : this()
        {
            this.commandViewModel = commandViewModel;
            this.commandsService = commandsService;
            this.settings = settings;

            this.PropertyChanged += FileNameChangedEventHandler;
            this.PropertyChanged += UpdateTrimUpdateExampleImageEventHandler;
        }

        public void AutoTopBottomTrim()
        {
            if(!string.IsNullOrWhiteSpace(Path) && File.Exists(Path))
            {
                using(Bitmap image = new Bitmap(Path))
                {
                    int height = image.Height;
                    int width = image.Width;
                    int maxTopBottomTrim = (height - 1) / 2;

                    bool allTransparent = true;
                    topBottomTrim = 0;
                    do
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color topPixel = image.GetPixel(x, topBottomTrim);
                            Color bottomPixel = image.GetPixel(x, height - 1 - topBottomTrim);

                            if (topPixel.A != 0 || bottomPixel.A != 0)
                            {
                                allTransparent = false;
                                break;
                            }
                        }

                        if (allTransparent)
                        {
                            topBottomTrim++;
                        }
                    }while (allTransparent && topBottomTrim < maxTopBottomTrim);

                    RaiseProppertyChanged(nameof(TopBottomTrim));
                }
            }
        }

        private void UpdateTrimUpdateExampleImageEventHandler(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(TopBottomTrim) || e.PropertyName == nameof(LeftTrim) || e.PropertyName == nameof(RightTrim))
            {
                if (File.Exists(path))
                {
                    string newExampleImagePath = GetTempFile();

                    using(Bitmap origional = new(path))
                    {
                        int width = origional.Width;
                        int height = origional.Height;

                        //make sure the trims actually fit within the image
                        leftTrim = Math.Min(LeftTrim, width - 1);
                        rightTrim = Math.Min(RightTrim, height - 1 - LeftTrim);
                        topBottomTrim = Math.Min(TopBottomTrim, (height - 1) / 2);

                        int trimmedWidth = width - LeftTrim - RightTrim;
                        int trimmedHeight = height - TopBottomTrim * 2;

                        using(Bitmap trimmed = new(trimmedWidth, trimmedHeight))
                        {
                            using(Graphics g = Graphics.FromImage(trimmed))
                            {
                                Rectangle sourceRect = new(LeftTrim, TopBottomTrim, trimmedWidth, trimmedHeight);
                                Rectangle destinationRect = new(0, 0, trimmedWidth, trimmedHeight);

                                g.DrawImage(origional, destinationRect, sourceRect, GraphicsUnit.Pixel);
                            }

                            trimmed.Save(newExampleImagePath);
                        }
                    }


                    if (File.Exists(newExampleImagePath))
                    {
                        if (!string.IsNullOrWhiteSpace(ExampleImagePath) && File.Exists(ExampleImagePath)) //if old example image is a true path that points towards a file and a new file exists to replace it remove the old
                        {
                            toDeleteFiles.Add(exampleImagePath);
                        }//remove the old example

                        ExampleImagePath = newExampleImagePath;

                        TryDeletingFiles();
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(ExampleImagePath) && File.Exists(ExampleImagePath)) //if old example image is a true path that points towards a file and a new file exists to replace it remove the old
                    {
                        toDeleteFiles.Add(exampleImagePath);

                        ExampleImagePath = String.Empty;

                        TryDeletingFiles();
                    }
                }
            }
        }

        private string GetTempFile()
        {
            string tempFileName = System.IO.Path.GetTempFileName(); //get the temp file name
            FileInfo tempFileInfo = new FileInfo(tempFileName); //get the attrubutes so they can be set
            tempFileInfo.Attributes = FileAttributes.Temporary; //set them to temporary
            return tempFileName;
        }

        private void FileChanged()
        {
            foreach (CommandWall wallCommand in commandsService.WallCommandGetElements(commandViewModel.FAFoldersViewModel.SelectedFolderCommands))//for every saved wall command
            {
                if (wallCommand.ImageName == FileName)//if a wall with the same name it found set the trims to the corrosponding values
                {
                    TopBottomTrim = wallCommand.TopBottomTrim;
                    LeftTrim = wallCommand.LeftTrim;
                    RightTrim = wallCommand.RightTrim;
                    return;
                }
            }
            TopBottomTrim = 0;
            LeftTrim = 0;
            RightTrim = 0;
        }

        private void FileNameChangedEventHandler(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FileName))
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    Path = string.Empty;
                }
                else
                {
                    string path;
                    string downloadPath = settings.FADownloadFolderPath;
                    string relativePath = commandViewModel.FAFoldersViewModel.SelectedFolder.RelativePath;
                    if (downloadPath.EndsWith(nextFolderChar) && relativePath.StartsWith(nextFolderChar))
                    {
                        path = downloadPath.Trim(nextFolderChar) + relativePath;
                    }
                    else
                    {
                        path = downloadPath + relativePath;
                    }
                    //get the path to the download path to check if the name exists.

                    string filePath = path + nextFolderChar + fileName;

                    if (File.Exists(filePath))
                    {
                        Path = filePath;
                    }
                    else
                    {
                        Path = string.Empty;
                    }
                }

                foreach (CommandWall wallCommand in commandsService.WallCommandGetElements(commandViewModel.FAFoldersViewModel.SelectedFolderCommands))//for every saved wall command
                {
                    if (wallCommand.ImageName == FileName)//if a wall with the same name it found set the trims to the corrosponding values
                    {
                        TopBottomTrim = wallCommand.TopBottomTrim;
                        LeftTrim = wallCommand.LeftTrim;
                        RightTrim = wallCommand.RightTrim;
                        return;
                    }
                }
                TopBottomTrim = 0;
                LeftTrim = 0;
                RightTrim = 0;
            }
        }

        private void TryDeletingFiles()
        {
            for (int i = toDeleteFiles.Count-1; i >= 0; i--)
            {
                try
                {
                    File.Delete(toDeleteFiles[i]);
                    toDeleteFiles.RemoveAt(i);
                }
                catch (IOException e)
                {

                }
            }
        }

        public void Dispose()
        {
            List<IOException> exceptions = new();

            for (int i = 0; i < toDeleteFiles.Count; i++)
            {
                try
                {
                    File.Delete(toDeleteFiles[1]);
                }
                catch (IOException e)
                {
                    exceptions.Add(e);
                }
            }

            try
            {
                File.Delete(ExampleImagePath);
            }
            catch (IOException e)
            {
                exceptions.Add(e);
            }

            if (exceptions.Count > 0)
            {
                throw exceptions[0];
            }
        }
    }
}
