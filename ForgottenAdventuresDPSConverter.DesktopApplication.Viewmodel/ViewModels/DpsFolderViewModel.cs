using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels
{
    public class DpsFolderViewModel : BaseViewModel
    {
        private readonly IDpsFolderService folderService;

        private bool workingOnNewFolder;
        private DpsFolder selectedFolder;
        private readonly ObservableCollection<DpsFolder> folders;

        public bool WorkingOnNewFolder
        {
            get => workingOnNewFolder;
            set
            {
                if (workingOnNewFolder != value)
                {
                    workingOnNewFolder = value;
                    RaiseProppertyChanged();
                }
            }
        }
        public DpsFolder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                if (selectedFolder != value)
                {
                    selectedFolder = value;
                    RaiseProppertyChanged();
                }
            }
        }
        public ObservableCollection<DpsFolder> Folders
        {
            get => folders;
        }

        public DpsFolderViewModel()
        {
            selectedFolder = new();
            WorkingOnNewFolder = false;
            folders = new();
        }

        public DpsFolderViewModel(IDpsFolderService folderService) : this()
        {
            this.folderService = folderService;
        }

        public void SelectNewFolder(int? id)
        {
            if (id == null)
            {
                WorkingOnNewFolder = true;
                SelectedFolder = new();
            }
            else
            {
                WorkingOnNewFolder = false;
                SelectedFolder = folderService.Get((int)id).Result;
            }
        }

        public void UpdateFolders()
        {
            List<DpsFolder> dpsFolders = new();
            dpsFolders.AddRange(folderService.GetAll().Result);
            dpsFolders = dpsFolders.OrderBy(x => x.Name).ToList();
            Folders.Clear();
            foreach (DpsFolder folder in dpsFolders)
            {
                Folders.Add(folder);
            }
        }

        public void AddFolder()
        {
            if (WorkingOnNewFolder && folderService.CanCreate(SelectedFolder).Result.CanExist)
            {
                folderService.Create(SelectedFolder).Wait();
                UpdateFolders();
            }
        }

        public void UpdateFolder()
        {
            if (!WorkingOnNewFolder && folderService.CanUpdate(SelectedFolder).Result.CanExist)
            {
                folderService.Update(SelectedFolder).Wait();
                UpdateFolders();
            }
        }
    }
}
