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
    public class DpsSubfolderService : IDpsSubfolderService
    {
        private const int nameMaxLength = 64;
        private const int maxLength = 4000;

        private readonly IRepository<DpsSubfolder> repository;
        private readonly IFAFolderService FAFolderService;

        public DpsSubfolderService(IRepository<DpsSubfolder> dpSubfolderRepository, IFAFolderService FAFolderService)
        {
            this.repository = dpSubfolderRepository ?? throw new ArgumentNullException(nameof(dpSubfolderRepository));
            this.FAFolderService = FAFolderService ?? throw new ArgumentNullException(nameof(FAFolderService));
        }

        public async Task<DpsSubfolderCanExistReport> CanCreate(DpsSubfolder toCreate)
        {
            DpsSubfolderCanExistReport report = await CanExist(toCreate);

            Task<bool>? containsNameTask = null;
            if(report.NameNotNullOrWhiteSpace && report.NameNotTooLong)
            {
                containsNameTask = repository.Contains(s => s.Name.Equals(toCreate.Name));
            }

            report.IdUnique = true;

            if(containsNameTask != null)
            {
                report.NameUnique = !await containsNameTask;
            }

            report.Finalize();
            return report;
        }

        public async Task<DpsSubfolderCanExistReport> CanUpdate(DpsSubfolder toUpdate)
        {
            Task<DpsSubfolderCanExistReport> reportTask = CanExist(toUpdate);
            Task<bool> containsIdTask = repository.Contains(s => s.Id == toUpdate.Id);

            DpsSubfolderCanExistReport report = await reportTask;

            Task<bool>? containsNameTask = null;
            if(report.NameNotNullOrWhiteSpace && report.NameNotTooLong)
            {
                containsNameTask = repository.Contains(s => s.Name.Equals(toUpdate.Name) && s.Id != toUpdate.Id);
            }
            else
            {
                report.NameUnique = true;
            }

            report.IdUnique = await containsIdTask;
            if(containsNameTask != null)
            {
                report.NameUnique = !await containsNameTask;
            }

            report.Finalize();
            return report;
        }

        public Task<bool> Create(DpsSubfolder toCreate)
        {
            if(toCreate == null)
            {
                return Task.FromResult(false);
            }
            else if(CanCreate(toCreate).Result.CanExist)
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
            if (await FAFolderService.HasFolderWithReferenceTo<DpsSubfolder>(id))
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
            if (await FAFolderService.SetReferenceNull<DpsSubfolder>(id))
            {
                return await repository.Delete(id);
            }
            else
            {
                return false;
            }
        }

        public Task<DpsSubfolder> Get(int id)
        {
            return repository.Get(id);
        }

        public Task<IEnumerable<DpsSubfolder>> GetAll()
        {
            return repository.GetAll();
        }

        public Task<IEnumerable<DpsSubfolder>> GetAllWhereDescriptionContains(string descriptionPart)
        {
            //todo
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DpsSubfolder>> GetAllWhereDescriptionContains(IEnumerable<string> descriptionParts)
        {
            //todo
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DpsSubfolder>> GetAllWhereNameContains(string namePart)
        {
            //todo
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DpsSubfolder>> GetAllWhereNameContains(IEnumerable<string> nameParts)
        {
            //todo
            throw new NotImplementedException();
        }

        public Task<bool> Update(DpsSubfolder toUpdate)
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


        private Task<DpsSubfolderCanExistReport> CanExist(DpsSubfolder toExist)
        {
            DpsSubfolderCanExistReport report = new();

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
            if(toExist.Description == null)
            {
                report.DescriptionNotNull = false;
                report.DescriptionNotTooLong = true;
            }
            else
            {
                report.DescriptionNotNull = true;
                report.DescriptionNotTooLong = toExist.Name.Count() <= maxLength;//has no practical max length at present but may need to be changed in future
            }

            return Task.FromResult(report);
        }
    }
}
