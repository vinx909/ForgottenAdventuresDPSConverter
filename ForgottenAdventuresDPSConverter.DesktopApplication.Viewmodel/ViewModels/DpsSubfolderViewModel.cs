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
    public class DpsSubfolderViewModel : BaseViewModel
    {
        private readonly IDpsSubfolderService subfolderService;

        private bool workingOnNewSubfolder;
        private DpsSubfolder selectedSubfolder;
        private readonly ObservableCollection<DpsSubfolder> subfolders;

        public bool WorkingOnNewSubfolder
        {
            get => workingOnNewSubfolder;
            set
            {
                if (workingOnNewSubfolder != value)
                {
                    workingOnNewSubfolder = value;
                    RaiseProppertyChanged();
                }
            }
        }
        public DpsSubfolder SelectedSubfolder
        {
            get => selectedSubfolder;
            set
            {
                if (selectedSubfolder != value)
                {
                    selectedSubfolder = value;
                    RaiseProppertyChanged();
                }
            }
        }
        public ObservableCollection<DpsSubfolder> Subfolders
        {
            get => subfolders;
        }

        public DpsSubfolderViewModel()
        {
            selectedSubfolder = new();
            WorkingOnNewSubfolder = false;
            subfolders = new();
        }

        public DpsSubfolderViewModel(IDpsSubfolderService subfolderService) : this()
        {
            this.subfolderService = subfolderService;
        }

        public void SelectNewSubfolder(int? id)
        {
            if (id == null)
            {
                WorkingOnNewSubfolder = true;
                SelectedSubfolder = new();
            }
            else
            {
                WorkingOnNewSubfolder = false;
                SelectedSubfolder = subfolderService.Get((int)id).Result;
            }
        }

        public void UpdateSubfolders()
        {
            List<DpsSubfolder> dpsSubfolders = new();
            dpsSubfolders.AddRange(subfolderService.GetAll().Result);
            dpsSubfolders = dpsSubfolders.OrderBy(x => x.Name).ToList();
            Subfolders.Clear();
            foreach (DpsSubfolder subfolder in dpsSubfolders)
            {
                Subfolders.Add(subfolder);
            }
        }

        public void AddSubfolder()
        {
            if (WorkingOnNewSubfolder && subfolderService.CanCreate(SelectedSubfolder).Result.CanExist)
            {
                subfolderService.Create(SelectedSubfolder).Wait();
                UpdateSubfolders();
            }
        }

        public void UpdateSubfolder()
        {
            if (!WorkingOnNewSubfolder && subfolderService.CanUpdate(SelectedSubfolder).Result.CanExist)
            {
                subfolderService.Update(SelectedSubfolder).Wait();
                UpdateSubfolders();
            }
        }
    }
}
