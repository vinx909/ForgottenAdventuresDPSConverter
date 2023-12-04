using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels
{
    public class FAFoldersViewModel : BaseViewModel
    {
        private const char searchSplitChar = ' ';

        private readonly IFAFolderService folderService;
        private readonly IDpsNumberService dpsNumberService;
        private readonly IDpsFolderService dpsFolderService;
        private readonly IDpsSubfolderService dpsSubfolderService;

        private readonly ISettingsGetter settings;

        public void SaveFolder()
        {
            Core.Reports.FAFolderCanExistReport result = folderService.CanUpdate(selectedFolder).Result;
            if (result.CanExist)
            {
                folderService.Update(selectedFolder);
            }
            else
            {

            }
        }

        private FAFolder selectedFolder;
        private DpsNumberEntityViewModel selectedDpsNumber;
        private DpsFolderEntityViewModel selectedDpsFolder;
        private DpsFolderEntityViewModel selectedDpsSubfolder;
        private bool editBasicData;
        private string folderSearchBar;
        private string subfolderSearchBar;

        public FATreeviewFolder FATreeviewFolder { get; set; }

        public FAFolder SelectedFolder { get => selectedFolder; set
            {
                if (value != SelectedFolder)
                {
                    selectedFolder = value;
                    RaiseProppertyChanged(); 
                }
            }
        }

        public DpsNumberEntityViewModel SelectedDpsNumber { get => selectedDpsNumber; set
            {
                if (value != SelectedDpsNumber)
                {
                    selectedDpsNumber = value;
                    if (value != null && SelectedDpsFolder != null && SelectedFolder.NumberId != value.Id)
                    {
                        SelectedFolder.NumberId = value.Id; 
                    }
                    RaiseProppertyChanged(); 
                }
            }
        }

        public DpsFolderEntityViewModel SelectedDpsFolder { get => selectedDpsFolder; set
            {
                if (selectedDpsFolder != value)
                {
                    selectedDpsFolder = value;
                    if (value != null && SelectedDpsFolder != null && SelectedFolder.FolderId != value.Id)
                    {
                        SelectedFolder.FolderId = value.Id;
                    }
                    RaiseProppertyChanged(); 
                }
            }
        }

        public DpsFolderEntityViewModel SelectedDpsSubfolder { get => selectedDpsSubfolder; set
            {
                if (selectedDpsSubfolder != value)
                {
                    selectedDpsSubfolder = value;
                    if (value != null && SelectedDpsFolder != null && SelectedFolder.SubfolderId != value.Id)
                    {
                        SelectedFolder.SubfolderId = value.Id;
                    }
                    RaiseProppertyChanged(); 
                }
            }
        }

        public bool EditBasicData { get => editBasicData; set
            {
                if (editBasicData != value)
                {
                    editBasicData = value;
                    RaiseProppertyChanged(); 
                }
            }
        }

        public ObservableCollection<string> SelectedFolderImagesPaths { get; set; }

        public ObservableCollection<DpsNumberEntityViewModel> Numbers { get; set; }

        public string FolderSearchBar { get => folderSearchBar; set
            {
                if (folderSearchBar != value)
                {
                    folderSearchBar = value;
                    RaiseProppertyChanged(); 
                }
            }
        
        }
        public ObservableCollection<DpsFolderEntityViewModel> FoundFolders { get; set; }

        public string SubfolderSearchBar { get => subfolderSearchBar; set
            {
                if (subfolderSearchBar != value)
                {
                    subfolderSearchBar = value;
                    RaiseProppertyChanged(); 
                }
            }
        }
        public ObservableCollection<DpsFolderEntityViewModel> FoundSubfolders { get; set; }


        public FAFoldersViewModel()
        {
            FATreeviewFolder = new FATreeviewFolder()
            {
                Id = 0,
                Name = "base folder"
            };
            SelectedFolder = new FAFolder();
            folderSearchBar = string.Empty;
            subfolderSearchBar = string.Empty;

            SelectedFolderImagesPaths = new();
            Numbers = new();
            FoundFolders = new();
            FoundSubfolders = new();

            Numbers.Add(new() { Id = null, Number = 0, Name = "none selected", Description = "the item to select if no number is to be selected" });
            FoundFolders.Add(new() { Id = null, Name = "none selected", Description = "the item to select if no number is to be selected" });
            FoundSubfolders.Add(new() { Id = null, Name = "none selected", Description = "the item to select if no number is to be selected" });
        }

        public FAFoldersViewModel(IFAFolderService FAFolderService, IDpsNumberService dpsNumberService, IDpsFolderService dpsFolderService, IDpsSubfolderService dpsSubfolderService, ISettingsGetter settings) : this()
        {
            this.folderService = FAFolderService;
            this.dpsNumberService = dpsNumberService;
            this.dpsFolderService = dpsFolderService;
            this.dpsSubfolderService = dpsSubfolderService;
            this.settings = settings;
            UpdateNumbers();
        }


        #region methods for regathering all FAFolders
        public void GetAllFAFolders()
        {
            GetAllFAFolders(null, f => true);
            UpdateNumbers();
        }

        private void GetAllFAFolders(Expression<Func<FATreeviewFolder, bool>> query)
        {
            GetAllFAFolders(null, query);
        }

        public void GetAllFAFolders(IProgress<double> progressPersentageDone)
        {
            GetAllFAFolders(progressPersentageDone, f => true);
        }

        private void GetAllFAFolders(IProgress<double>? progressPersentageDone, Expression<Func<FATreeviewFolder, bool>> query)
        {
            const int progressTimerPeriod = 100; //the amount of miliseconds between progressPersentageDone gets reported

            Func<FATreeviewFolder, bool>? queryFunc = query.Compile();

            Task<IEnumerable<FAFolder>> getParentlessTask = folderService.GetParentless(); //set up a task getting all the folders that don't have a parent folder and should thus be in the base folder

            Timer persentageTimer = null;
            List<Task> tasksToDo = null;
            if (progressPersentageDone != null)
            {
                tasksToDo = new();
                progressPersentageDone.Report(0);//initial progress report
                persentageTimer = new(updateProgressPersentageDone, null, 0, progressTimerPeriod);//set a timer to periodically update the progressPersentageDone
            }//if progressPersentageDone isn't null set up a timer and a list of all tasks. the timer will periodically update the IProgress, and the list is used to make the progress as accurate as possible

            UpdateNumbers();

            List<Task<FATreeviewFolder>> baseFolderTasks = new();//a list of tasks that will contain the FATreeviewFolders that should fill the basefolder

            foreach (FAFolder folder in getParentlessTask.Result)
            {
                Task<FATreeviewFolder> baseFolderTask = GetTreeviewFolder(folder);

                if (tasksToDo != null)
                {
                    tasksToDo.Add(baseFolderTask);
                }

                baseFolderTasks.Add(baseFolderTask);
            }//for each parentless folder create a tasks that sets up their folder. add them to the list of folders that need to be added to the basefolder. and if there is progress reporting add them to the list so there is a count of all tasks that need to be done for the total task to be done.

            for (int i = 0; i < baseFolderTasks.Count(); i++)
            {
                baseFolderTasks[i].Wait();
            }//wait for each task for the basefolder to be done. this is done before the old children are cleared so it stays filled as long as possible

            List<FATreeviewFolder> childeren = new();
            for (int i = 0; i < baseFolderTasks.Count(); i++)
            {
                FATreeviewFolder folder = baseFolderTasks[i].Result;
                if (queryFunc(folder))
                {
                    childeren.Add(folder);
                }
            }//add each parentless folder to the child list

            FATreeviewFolder.Children.Clear();//remove all the previous folders from the basefolder.
            foreach(FATreeviewFolder child in childeren.OrderBy(c => c.Name))
            {
                FATreeviewFolder.Children.Add(child);
            }//add each parentless folder to the basefolder in alphabetical order.

            //await updateNumbersTask;

            if (persentageTimer != null)
            {
                persentageTimer.Dispose();
                progressPersentageDone.Report(1);
            }//end the update timer if it exists and send the final report

            return;

            async Task<FATreeviewFolder> GetTreeviewFolder(FAFolder folder)
            {
                Task<IEnumerable<FAFolder>> getChildrenTask = folderService.GetChildren(folder.Id); //set up the task to get the children

                FATreeviewFolder returnFolder = new(folder.Id, folder.Name, folder.NumberId != null, folder.FolderId != null, folder.SubfolderId != null, folder.HasItems);//create the folder that's to be returned.

                List<Task<FATreeviewFolder>> childFolderTasks = new();//list of tasks to set up the children folders

                foreach (FAFolder childFolder in await getChildrenTask)
                {
                    Task<FATreeviewFolder> baseFolderTask = GetTreeviewFolder(childFolder);

                    if (tasksToDo != null)
                    {
                        tasksToDo.Add(baseFolderTask);
                    }

                    childFolderTasks.Add(baseFolderTask);
                }//for each child create a tasks to set up their folder. add them to the list of folders that need to be done for this folder to be set up. and if there is progress reporting add them to the list so there is a count of all tasks that need to be done for the total task to be done.


                List<FATreeviewFolder> children = new();
                while (childFolderTasks.Count > 0)
                {
                    FATreeviewFolder child = await childFolderTasks[0];
                    if (queryFunc(child))
                    {
                        children.Add(child);
                    }
                    childFolderTasks.RemoveAt(0);
                }//while there are tasks setting up children wait for each task to be done and add it to the children list
                
                foreach(FATreeviewFolder child in children.OrderBy(c => c.Name))
                {
                    returnFolder.Children.Add(child);
                }//add each child to returnFolder children in alphabetical order

                return returnFolder;//return the folder now set up with all it's children
            }

            void updateProgressPersentageDone(object? args)
            {
                int done = 0;
                int todo = tasksToDo.Count;
                for (int i = 0; i < todo; i++)
                {
                    if (tasksToDo[i].IsCompleted)
                    {
                        done++;
                    }
                }
                progressPersentageDone.Report(1.0 * done / todo);
            }
        }

        public void GetAllFAFoldersWithoutNumber()
        {
            GetAllFAFolders(null, (f => (f.HasNumber == false && f.HasItem == true ) || f.HasChildWithoutNumber));
        }

        public void GetAllFAFoldersWithoutNumber(IProgress<double> progressPersentageDone)
        {
            GetAllFAFolders(progressPersentageDone, f => (f.HasNumber == false && f.HasItem == true) || f.HasChildWithoutNumber);
        }

        public void GetAllFAFoldersWithoutFolder()
        {
            GetAllFAFolders(null, f => (f.HasFolder == false && f.HasItem == true) || f.HasChildWithoutFolder);
        }

        public void GetAllFAFoldersWithoutFolder(IProgress<double> progressPersentageDone)
        {
            GetAllFAFolders(progressPersentageDone, f => (f.HasFolder == false && f.HasItem == true) || f.HasChildWithoutFolder);
        }

        public void GetAllFAFoldersWithoutSubfolder()
        {
            GetAllFAFolders(null, f => (f.HasSubfolder == false && f.HasItem == true) || f.HasChildWithoutSubfolder);
        }

        public void GetAllFAFoldersWithoutSubfolder(IProgress<double> progressPersentageDone)
        {
            GetAllFAFolders(progressPersentageDone, f => (f.HasSubfolder == false && f.HasItem == true) || f.HasChildWithoutSubfolder);
        }
        #endregion

        public void SelectNewFolder(int id)
        {

            EditBasicData = false;
            FolderSearchBar = string.Empty;
            SubfolderSearchBar = string.Empty;

            UpdateFolders(false);
            UpdateSubfolders(false);

            SelectedFolder = folderService.Get(id).Result;
            UpdateSelectedFolderImages();

            int? selectedDpsNumberId = SelectedFolder.NumberId;
            int? selectedDpsFolderId = SelectedFolder.FolderId;
            int? selectedDpsSubfolderId = SelectedFolder.SubfolderId;

            SelectedDpsNumber = Numbers.FirstOrDefault(n => n.Id == selectedDpsNumberId);
            SelectedDpsFolder = FoundFolders.FirstOrDefault(f => f.Id == selectedDpsFolderId);
            SelectedDpsSubfolder = FoundSubfolders.FirstOrDefault(f => f.Id == selectedDpsSubfolderId);
        }

        public void UpdateSelectedFolderImages()
        {
            const int minNumberOfItems = 7;

            SelectedFolderImagesPaths.Clear();

            string path;
            if(settings.FADownloadFolderPath.EndsWith('\\') && selectedFolder.RelativePath.StartsWith('\\'))
            {
                path = settings.FADownloadFolderPath.Trim('\\') + SelectedFolder.RelativePath;
            }
            else
            {
                path = settings.FADownloadFolderPath + SelectedFolder.RelativePath;
            }
            foreach (string filePath in Directory.EnumerateFiles(path))
            {
                SelectedFolderImagesPaths.Add(filePath);
            }
            if(SelectedFolderImagesPaths.Count < minNumberOfItems && SelectedFolderImagesPaths.Count > 0)
            {
                while(SelectedFolderImagesPaths.Count < minNumberOfItems)
                {
                    SelectedFolderImagesPaths.Add(SelectedFolderImagesPaths[0]);
                }
            }
        }

        public void UpdateFolders(bool keepSelectedFolder = true)
        {
            #region remove old folders
            if (keepSelectedFolder)
            {
                bool found = false;
                while (FoundFolders.Count > 2)
                {
                    if (!found)
                    {
                        if (FoundFolders[1] == SelectedDpsFolder)
                        {
                            found = true;
                        }
                        else
                        {
                            FoundFolders.RemoveAt(1);
                        }
                    }
                    else
                    {
                        FoundFolders.RemoveAt(2);
                    }
                }
            }
            else
            {
                while (FoundFolders.Count > 1)
                {
                    FoundFolders.RemoveAt(1);
                }
            }
            #endregion

            IEnumerable<DpsFolder> folders;

            if (string.IsNullOrWhiteSpace(FolderSearchBar))
            {
                folders = dpsFolderService.GetAll().Result.OrderBy(f => f.Name);
            }
            else
            {
                List<DpsFolder> foldersList = new();
                foldersList.AddRange(dpsFolderService.GetAllWhereNameContains(FolderSearchBar).Result.OrderBy(f=>f.Name));
                foldersList.AddRange(dpsFolderService.GetAllWhereDescriptionContains(FolderSearchBar).Result.OrderBy(f => f.Name));
                folders = foldersList;
            }

            foreach (DpsFolder folder in folders)
            {
                if (FoundFolders.FirstOrDefault(f => f.Id == folder.Id) == null)
                {
                    FoundFolders.Add(new(folder));
                }
            }
        }

        public void UpdateSubfolders(bool keepSelectedSubfolder = true)
        {
            #region remove old subfolders
            if (keepSelectedSubfolder)
            {
                bool found = false;
                while (FoundSubfolders.Count > 2)
                {
                    if (!found)
                    {
                        if (FoundSubfolders[1] == SelectedDpsSubfolder)
                        {
                            found = true;
                        }
                        else
                        {
                            FoundSubfolders.RemoveAt(1);
                        }
                    }
                    else
                    {
                        FoundSubfolders.RemoveAt(2);
                    }
                }
            }
            else
            {
                while (FoundSubfolders.Count > 1)
                {
                    FoundSubfolders.RemoveAt(1);
                }
            }
            #endregion

            IEnumerable<DpsSubfolder> subfolders;

            if (string.IsNullOrWhiteSpace(FolderSearchBar))
            {
                subfolders = dpsSubfolderService.GetAll().Result.OrderBy(f => f.Name);
            }
            else
            {
                List<DpsSubfolder> subfoldersList = new();
                subfoldersList.AddRange(dpsSubfolderService.GetAllWhereNameContains(SubfolderSearchBar).Result.OrderBy(f => f.Name).OrderBy(f => f.Name));
                subfoldersList.AddRange(dpsSubfolderService.GetAllWhereDescriptionContains(SubfolderSearchBar).Result.OrderBy(f => f.Name).OrderBy(f => f.Name));
                subfolders = subfoldersList;
            }

            foreach (DpsSubfolder folder in subfolders)
            {
                if (FoundSubfolders.FirstOrDefault(f => f.Id == folder.Id) == null)
                {
                    FoundSubfolders.Add(new(folder));
                }
            }
        }



        private void UpdateNumbers()
        {
            while(Numbers.Count > 1)
            {
                Numbers.RemoveAt(1);
            }

            foreach (DpsNumber number in dpsNumberService.GetAll().Result)
            {
                Numbers.Add(new DpsNumberEntityViewModel(number));
            }
        }
    }
}
