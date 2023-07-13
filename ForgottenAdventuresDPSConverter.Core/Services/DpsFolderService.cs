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
    public class DpsFolderService : IDpsFolderService
    {
        private const int nameMaxLength = 64;
        private const int nameAbriviationMaxLength = 2;
        private const int maxLength = 4000;

        private readonly IRepository<DpsFolder> repository;
        private readonly IFAFolderService FAFolderService;

        public DpsFolderService(IRepository<DpsFolder> DpsFolderRepository, IFAFolderService FAFolderService)
        {
            repository = DpsFolderRepository ?? throw new ArgumentNullException(nameof(DpsFolderRepository));
            this.FAFolderService = FAFolderService ?? throw new ArgumentNullException(nameof(FAFolderService));
        }

        public async Task<DpsFolderCanExistReport> CanCreate(DpsFolder toCreate)
        {
            DpsFolderCanExistReport report = await CanExist(toCreate);

            Task<bool>? containsNameTask = null;
            if (report.NameNotNullOrWhiteSpace && report.NameNotTooLong)
            {
                containsNameTask = repository.Contains(f => f.Name.Equals(toCreate.Name));
            }
            else
            {
                report.NameUnique = true;
            }
            Task<bool>? containsNameAbriviationTask = null;
            if (report.NameAbriviationNotNullOrWhiteSpace && report.NameAbriviationNotTooLong)
            {
                containsNameAbriviationTask = repository.Contains(f => f.NameAbriviation.Equals(toCreate.NameAbriviation));
            }
            else
            {
                report.NameAbriviationUnique = true;
            }

            report.IdUnique = true;

            if (containsNameTask != null)
            {
                report.NameUnique = !await containsNameTask;
            }
            if (containsNameAbriviationTask != null)
            {
                report.NameAbriviationUnique = !await containsNameAbriviationTask;
            }

            report.Finalize();
            return report;
        }

        public async Task<DpsFolderCanExistReport> CanUpdate(DpsFolder toUpdate)
        {
            Task<DpsFolderCanExistReport> reportTask = CanExist(toUpdate);
            Task<bool> containsIdTask = repository.Contains(f => f.Id == toUpdate.Id);

            DpsFolderCanExistReport report = await reportTask;
            Task<bool>? containsNameTask = null;
            if(report.NameNotNullOrWhiteSpace && report.NameNotTooLong)
            {
                containsNameTask = repository.Contains(f => f.Name.Equals(toUpdate.Name) && f.Id != toUpdate.Id);
            }
            else
            {
                report.NameUnique = true;
            }
            Task<bool>? containsNameAbriviationTask = null;
            if(report.NameAbriviationNotNullOrWhiteSpace && report.NameAbriviationNotTooLong)
            {
                containsNameAbriviationTask = repository.Contains(f => f.NameAbriviation.Equals(toUpdate.NameAbriviation) && f.Id != toUpdate.Id);
            }
            else
            {
                report.NameAbriviationUnique = true;
            }

            report.IdUnique = await containsIdTask;

            if(containsNameTask != null)
            {
                report.NameUnique = !await containsNameTask;
            }
            if(containsNameAbriviationTask != null)
            {
                report.NameAbriviationUnique = !await containsNameAbriviationTask;
            }

            report.Finalize();
            return report;
        }

        public Task<bool> Create(DpsFolder toCreate)
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
            if(await FAFolderService.HasFolderWithReferenceTo<DpsFolder>(id))
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
            if (await FAFolderService.SetReferenceNull<DpsFolder>(id))
            {
                return await repository.Delete(id);
            }
            else
            {
                return false;
            }
        }

        public Task<DpsFolder> Get(int id)
        {
            return repository.Get(id);
        }

        public Task<IEnumerable<DpsFolder>> GetAll()
        {
            return repository.GetAll();
        }

        public Task<IEnumerable<DpsFolder>> GetAllWhereDescriptionContains(string descriptionPart)
        {
            //todo:
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DpsFolder>> GetAllWhereDescriptionContains(IEnumerable<string> descriptionParts)
        {
            //todo:
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DpsFolder>> GetAllWhereNameContains(string namePart)
        {
            //todo:
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DpsFolder>> GetAllWhereNameContains(IEnumerable<string> nameParts)
        {
            //todo:
            throw new NotImplementedException();
        }

        public async Task<bool> Update(DpsFolder toUpdate)
        {
            if (toUpdate == null)
            {
                return false;
            }
            else if (CanUpdate(toUpdate).Result.CanExist)
            {
                return await repository.Update(toUpdate);
            }
            else
            {
                return false;
            }
        }


        private Task<DpsFolderCanExistReport> CanExist(DpsFolder toExist)
        {
            DpsFolderCanExistReport report = new();

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
            if (string.IsNullOrWhiteSpace(toExist.NameAbriviation))
            {
                report.NameAbriviationNotNullOrWhiteSpace = false;
                report.NameAbriviationNotTooLong = true;
            }
            else
            {
                report.NameAbriviationNotNullOrWhiteSpace = true;
                report.NameAbriviationNotTooLong = toExist.NameAbriviation.Count() <= nameAbriviationMaxLength;
            }
            report.DescriptionNotNull = toExist.NameAbriviation != null;
            report.DescriptionNotTooLong = toExist.Description.Count() <= maxLength;//has no practical max length at present but may need to be changed in future

            return Task.FromResult(report);
        }
    }
}
