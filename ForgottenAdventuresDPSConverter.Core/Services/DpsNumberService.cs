using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Services
{
    public class DpsNumberService : IDpsNumberService
    {
        private const int nameMaxLength = 64;
        private const int maxLength = 4000;

        private readonly IRepository<DpsNumber> repository;
        private readonly IFAFolderService FAFolderService;

        public DpsNumberService(IRepository<DpsNumber> numberRepository, IFAFolderService FAFolderService)
        {
            this.repository = numberRepository ?? throw new ArgumentNullException(nameof(numberRepository));
            this.FAFolderService = FAFolderService ?? throw new ArgumentNullException(nameof(FAFolderService));
        }

        public async Task<DpsNumberCanExistReport> CanCreate(DpsNumber toCreate)
        {
            Task<DpsNumberCanExistReport> reportTask = CanExist(toCreate);
            Task<bool> containsNumberTask = repository.Contains(n => n.Number == toCreate.Number);

            DpsNumberCanExistReport report = await reportTask;
            Task<bool>? containsNameTask = null;
            if(report.NameNotNullOrWhiteSpace && report.NameNotTooLong)
            {
                containsNameTask = repository.Contains(n => n.Name.Equals(toCreate.Name));
            }
            else
            {
                report.NameUnique = true;
            }

            report.IdAvailable = true;
            
            report.NumberUnique = !await containsNumberTask;
            if(containsNameTask != null)
            {
                report.NameUnique = !await containsNameTask;
            }

            return report;
        }

        public async Task<DpsNumberCanExistReport> CanUpdate(DpsNumber toUpdate)
        {
            DpsNumberCanExistReport report = await CanExist(toUpdate);
            Task<bool> containsIdTask = repository.Contains(n => n.Id == toUpdate.Id);
            Task<bool> numberNotInUseWithOtherIdTask = repository.Contains(n => n.Number == toUpdate.Number && n.Id != toUpdate.Id);

            Task<bool> containsNameTask = null;
            if(report.NameNotNullOrWhiteSpace && report.NameNotTooLong)
            {
                containsNameTask = repository.Contains(n => n.Name.Equals(toUpdate.Name) && n.Id != toUpdate.Id);
            }
            else
            {
                report.NameUnique = true;
            }

            report.IdAvailable = await containsIdTask;
            report.NumberUnique = !await numberNotInUseWithOtherIdTask;

            if(containsNameTask != null)
            {
                report.NameUnique = !await containsNameTask;
            }

            return report;
        }

        public Task<bool> Create(DpsNumber toCreate)
        {
            if (toCreate == null)
            {
                return Task.FromResult(false);
            }
            else if (CanCreate(toCreate).Result.CanExist)
            {
                return Task.FromResult(repository.Create(toCreate).Result.Item1);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public async Task<bool> Delete(int id)
        {
            if (await FAFolderService.HasFolderWithReferenceTo<DpsNumber>(id))
            {
                return false;
            }
            else
            {
                return await repository.Delete(id);
            }
        }

        public async Task<bool> DeleteAndSetReferencesToNull(int id)
        {
            if (await FAFolderService.SetReferenceNull<DpsNumber>(id))
            {
                return await repository.Delete(id);
            }
            else
            {
                return false;
            }
        }

        public Task<DpsNumber> Get(int id)
        {
            return repository.Get(id);
        }

        public Task<IEnumerable<DpsNumber>> GetAll()
        {
            return repository.GetAll();
        }

        public Task<bool> Update(DpsNumber toUpdate)
        {
            if (toUpdate == null)
            {
                return Task.FromResult(false);
            }
            else if (CanUpdate(toUpdate).Result.CanExist)
            {
                return repository.Update(toUpdate);
            }
            else
            {
                return Task.FromResult(false);
            }
        }


        private Task<DpsNumberCanExistReport> CanExist(DpsNumber toExist)
        {
            DpsNumberCanExistReport report = new();

            if (string.IsNullOrWhiteSpace(toExist.Name))
            {
                report.NameNotNullOrWhiteSpace = false;
                report.NameNotTooLong = true;
            }
            else
            {
                report.NameNotNullOrWhiteSpace = true;
                report.NameNotTooLong = toExist.Name.Count() <= nameMaxLength;
            }
            if (toExist.Description == null)
            {
                report.DescriptionNotNull = false;
                report.DescriptionNotTooLong = true;
            }
            else
            {
                report.DescriptionNotNull = true;
                report.DescriptionNotTooLong = toExist.Description.Count() <= maxLength;//has no practical max length at present but may need to be changed in future
            }


            return Task.FromResult(report);
        }
    }
}
