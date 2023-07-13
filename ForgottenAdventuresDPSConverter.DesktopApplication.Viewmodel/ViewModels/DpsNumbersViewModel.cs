using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels
{
    public class DpsNumbersViewModel : BaseViewModel
    {
        private readonly IDpsNumberService numberService;

        private bool workingOnNewNumber;
        private DpsNumberEntityViewModel selectedNumber;
        private readonly ObservableCollection<DpsNumberEntityViewModel> numbers;

        public bool WorkingOnNewNumber
        {
            get => workingOnNewNumber;
            set
            {
                if (workingOnNewNumber != value)
                {
                    workingOnNewNumber = value;
                    RaiseProppertyChanged();
                }
            }
        }
        public DpsNumberEntityViewModel SelectedNumber
        {
            get => selectedNumber;
            set
            {
                if(selectedNumber != value)
                {
                    selectedNumber = value;
                    RaiseProppertyChanged();
                }
            }
        }
        public ObservableCollection<DpsNumberEntityViewModel> Numbers
        {
            get => numbers;
        }

        public DpsNumbersViewModel()
        {
            selectedNumber = new();
            WorkingOnNewNumber = false;
            numbers = new();
        }

        public DpsNumbersViewModel(IDpsNumberService numberService) : this()
        {
            this.numberService = numberService;
        }

        public void SelectNewNumber(int? id)
        {
            if(id == null)
            {
                WorkingOnNewNumber = true;
                SelectedNumber = new();
            }
            else
            {
                WorkingOnNewNumber = false;
                SelectedNumber = new(numberService.Get((int)id).Result);
            }
        }

        public void UpdateNumbers()
        {
            List<DpsNumber> dpsNumbers = new();
            dpsNumbers.AddRange(numberService.GetAll().Result);
            dpsNumbers.OrderBy(x => x.Number).ToList();
            Numbers.Clear();
            foreach (DpsNumber number in dpsNumbers)
            {
                Numbers.Add(new(number));
            }
            SelectedNumber = new();
            WorkingOnNewNumber = false;
        }

        public void AddNumber()
        {
            if (WorkingOnNewNumber && numberService.CanCreate(SelectedNumber.ToDpsNumber(true)).Result.CanExist)
            {
                numberService.Create(SelectedNumber.ToDpsNumber(true)).Wait();
                SelectedNumber = new();
            }
        }

        public void UpdateNumber()
        {
            if (!WorkingOnNewNumber && numberService.CanUpdate(SelectedNumber.ToDpsNumber()).Result.CanExist)
            {
                numberService.Update(SelectedNumber.ToDpsNumber()).Wait();
                UpdateNumbers();
            }
        }
    }
}
