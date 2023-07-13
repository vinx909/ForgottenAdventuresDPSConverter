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
    public class FAFolderService : IFAFolderService
    {
        private const int maxLength = 4000;

        private readonly IRepository<FAFolder> repository;
        private readonly IRepository<DpsFolder> folderRepository;
        private readonly IRepository<DpsNumber> numberRepository;
        private readonly IRepository<DpsSubfolder> subfolderRepository;


        public FAFolderService(IRepository<FAFolder> FAFolderRepository, IRepository<DpsFolder> folderRepository, IRepository<DpsNumber> numberRepository, IRepository<DpsSubfolder> subfolderRepository)
        {
            this.repository = FAFolderRepository ?? throw new ArgumentNullException(nameof(FAFolderRepository));
            this.folderRepository = folderRepository ?? throw new ArgumentNullException(nameof(folderRepository));
            this.numberRepository = numberRepository ?? throw new ArgumentNullException(nameof(numberRepository));
            this.subfolderRepository = subfolderRepository ?? throw new ArgumentNullException(nameof(subfolderRepository));
        }


        public async Task<FAFolderCanExistReport> CanCreate(FAFolder toCreate)
        {
            Task<FAFolderCanExistReport> reportTask = CanExist(toCreate);

            FAFolderCanExistReport report = await reportTask;

            Task<bool>? containsRelativePathTask = null;
            if (report.RelativePathNotNullOrWhiteSpace && report.RelativePathNotTooLong)
            {
                containsRelativePathTask = repository.Contains(f => f.RelativePath.Equals(toCreate.RelativePath));
            }
            else
            {
                report.RelativePathUnique = true;
            }

            report.IdUnique = true;

            if (containsRelativePathTask != null)
            {
                report.RelativePathUnique = !await containsRelativePathTask;
            }

            report.Finalize();
            return report;
        }

        public async Task<FAFolderCanExistReport> CanUpdate(FAFolder toUpdate)
        {
            Task<FAFolderCanExistReport> reportTask = CanExist(toUpdate);
            Task<bool> containsIdTask = repository.Contains(f => f.Id == toUpdate.Id);

            FAFolderCanExistReport report = await reportTask;

            Task<bool>? containsRelativePathTask = null;
            if (report.RelativePathNotNullOrWhiteSpace && report.RelativePathNotTooLong)
            {
                containsRelativePathTask = repository.Contains(f => f.RelativePath.Equals(toUpdate.RelativePath) && f.Id != toUpdate.Id);
            }
            else
            {
                report.RelativePathUnique = true;
            }

            report.IdUnique = await containsIdTask;

            if (containsRelativePathTask != null)
            {
                report.RelativePathUnique = !await containsRelativePathTask;
            }

            report.Finalize();
            return report;
        }

        public Task<bool> Create(FAFolder toCreate)
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
            if(!await repository.Contains(f => f.ParentId == id))
            {
                return await repository.Delete(id);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteWithChildren(int id)
        {
            List<Task<bool>> childDeleteTasks = new();

            foreach (FAFolder child in await GetChildren(id))
            {
                childDeleteTasks.Add(DeleteWithChildren(child.Id));
            }

            bool childrenSuccessfullyDeleted = true;
            while(childDeleteTasks.Count > 0)
            {
                childrenSuccessfullyDeleted = childrenSuccessfullyDeleted && await childDeleteTasks[0];
                childDeleteTasks.RemoveAt(0);
            }

            if (childrenSuccessfullyDeleted)
            {
                return await Delete(id);
            }
            else
            {
                return false;
            }
        }

        public Task<FAFolder> Get(int id)
        {
            return repository.Get(id);
        }

        public Task<IEnumerable<FAFolder>> GetAll()
        {
            return repository.GetAll();
        }

        public Task<IEnumerable<FAFolder>> GetAllWithItemsAndNoFolder()
        {
            return repository.GetAll(f => f.FolderId == null);
        }

        public Task<IEnumerable<FAFolder>> GetAllWithItemsAndNoNumber()
        {
            return repository.GetAll(f => f.NumberId == null);
        }

        public Task<IEnumerable<FAFolder>> GetAllWithItemsAndNoSubfolder()
        {
            return repository.GetAll(f => f.SubfolderId == null);
        }

        public Task<IEnumerable<FAFolder>> GetChildren(int parentFolderId)
        {
            return repository.GetAll(f => f.ParentId == parentFolderId);
        }

        public Task<IEnumerable<FAFolder>> GetParentless()
        {
            return repository.GetAll(f => f.ParentId == null);
        }

        public Task<bool> HasFolderWithReferenceTo<T>(int id)
        {
            if(typeof(T) == typeof(FAFolder))
            {
                return repository.Contains(f => f.ParentId == id);
            }
            else if (typeof(T) == typeof(DpsFolder))
            {
                return repository.Contains(f => f.FolderId == id);
            }
            else if (typeof(T) == typeof(DpsNumber))
            {
                return repository.Contains(f => f.NumberId == id);
            }
            else if (typeof(T) == typeof(DpsSubfolder))
            {
                return repository.Contains(f => f.SubfolderId == id);
            }
            else
            {
                throw new NotImplementedException("HasFolderWithReferenceTo in FAFolderService was called with type T as " + typeof(T).Name + " which no implementation has been given for.");
            }
        }

        public async Task<bool> SetReferenceNull<T>(int id)
        {
            Task<IEnumerable<FAFolder>> toUpdateFoldersTask;
            if(typeof(T) == typeof(DpsFolder))
            {
                toUpdateFoldersTask = repository.GetAll(f => f.FolderId == id);
            }
            else if(typeof(T) == typeof(DpsNumber))
            {
                toUpdateFoldersTask = repository.GetAll(f => f.NumberId == id);
            }
            else if(typeof (T) == typeof(DpsSubfolder))
            {
                toUpdateFoldersTask = repository.GetAll(f => f.SubfolderId == id);
            }
            else
            {
                throw new NotImplementedException("SetReferenceNull in FAFolderService was called with type T as " + typeof(T).Name + " which no implementation has been given for. this was thrown at the getting folders setp (first step)");
            }

            List<Task<bool>> updateTasks = new();
            foreach (FAFolder folder in await toUpdateFoldersTask)
            {
                if (typeof(T) == typeof(DpsFolder))
                {
                    folder.FolderId = null;
                }
                else if (typeof(T) == typeof(DpsNumber))
                {
                    folder.NumberId = null;
                }
                else if (typeof(T) == typeof(DpsSubfolder))
                {
                    folder.SubfolderId = null;
                }
                else
                {
                    throw new NotImplementedException("SetReferenceNull in FAFolderService was called with type T as " + typeof(T).Name + " which no implementation has been given for. this was thrown at the setting reference to null step (second step)");
                }

                updateTasks.Add(repository.Update(folder));
            }

            bool successfullyUpdated = true;
            while(updateTasks.Count > 0)
            {
                successfullyUpdated = successfullyUpdated && await updateTasks[0];
                updateTasks.RemoveAt(0);
            }

            return successfullyUpdated;
        }

        public Task<bool> Update(FAFolder toUpdate)
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

        public async Task<FAFolderUpdateReport> UpdateFolders(string folderPath, IProgress<double> progressPersentageDone, IProgress<FAFolderUpdateReport> progressReport)
        {
            const int progressTimerPeriod = 100;

            const string directoryDoesNotExistMessage = "the given directory does not exist";
            const string updateStillRunningMessage = "updating still running";
            const string successfullyFinishedMessage = "updating ran to completion";

            FAFolderUpdateReport report = new();

            if (Directory.Exists(folderPath) == false)
            {
                report.Message = directoryDoesNotExistMessage;
                return report;
            }
            else
            {
                report.Message = updateStillRunningMessage;
            }

            int folderPathStartLength = folderPath.Length; //saves the folderPath length to remove to split the relative folder path off.

            Dictionary<string, int> existingPathsAndIds = await GetDictionaryWithExistingPathsAndIds();

            List<Task> updateTasks = new();

            updateTasks.Add(Task.Run(async () => await updateSubfolder(folderPath, null)));

            #region set times for the IProgresses
            Timer? persentageTimer = null;
            if (progressPersentageDone != null)
            {
                progressPersentageDone.Report(0);//initial progress report
                persentageTimer = new(updateProgressPersentageDone, null, 0, progressTimerPeriod);//set a timer to periodically update the progressPersentageDone
            }

            Timer? reportTimer = null;
            bool reportUpdated = false;
            if (progressReport != null)
            {
                progressReport.Report(report);//initial report
                persentageTimer = new(updateProgressReport, null, 0, progressTimerPeriod);//set a timer to periodically update the progressReport
            }
            #endregion


            for (int i = 0; i < updateTasks.Count; i++)
            {
                await updateTasks[i];
            }


            if(existingPathsAndIds.Count > 0)
            {
                foreach (int id in existingPathsAndIds.Values)
                {
                    report.NoLongerExisting.Add(id);
                }
            }

            persentageTimer?.Dispose();
            progressPersentageDone?.Report(1);

            report.Message = successfullyFinishedMessage;
            return report;

            
            async Task updateSubfolder(string folderPath, int? parentId)
            {
                int pathLength = folderPath.Length + 1;

                foreach (string subfolderPath in Directory.GetDirectories(folderPath))
                {
                    string relativePath = subfolderPath.Remove(0, folderPathStartLength);

                    if (existingPathsAndIds.ContainsKey(relativePath))//if the relative path already exists check if it needs updating and if so update it. then remove that relative path from the dictionary so in the end it's left with only the directories that no longer exist.
                    {
                        FAFolder existingFolder = await repository.Get(existingPathsAndIds[relativePath]);
                        existingPathsAndIds.Remove(relativePath);
                        bool needsUpdating = false;


                        //check if any part of the folder needs updating
                        string name = subfolderPath.Remove(0, pathLength);
                        if (existingFolder.Name != name)
                        {
                            existingFolder.Name = name;
                            needsUpdating = true;
                        }

                        int? parentID = parentId;
                        if (existingFolder.ParentId != parentID)
                        {
                            existingFolder.ParentId = parentID;
                            needsUpdating = true;
                        }

                        bool hasItems = Directory.GetFiles(subfolderPath).Length > 0;
                        if(existingFolder.HasItems != hasItems)
                        {
                            existingFolder.HasItems = hasItems;
                            needsUpdating = true;
                        }


                        if(needsUpdating == true)//if the folder needs updating
                        {
                            FAFolderCanExistReport canExistReport = await CanUpdate(existingFolder);
                            if (canExistReport.CanExist)//if the folder can be updated
                            {
                                if (await repository.Update(existingFolder))//try to update
                                {
                                    report.Updated.Add(existingFolder.Id);//if updating succeeded add it to the updated list

                                    reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null
                                }
                                else
                                {
                                    report.FailedToUpdate.Add(existingFolder.Id);//if updating failed add the it ot the failed to update list

                                    reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null
                                }
                            }
                            else//if the folder can't be updated
                            {
                                report.FailedToUpdate.Add(existingFolder.Id);//if updating failed add the it ot the failed to update list

                                reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null
                            }
                        }
                        else
                        {
                            report.Unaltered.Add(existingFolder.Id);//if the folder didn't need updating instead add it to unaltered list

                            reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null
                        }
                        updateTasks.Add(Task.Run(() => updateSubfolder(subfolderPath, existingFolder.Id)));//continue updating all the folders subfolders
                    }
                    else//if the relative path doesn't already exists add it instead.
                    {
                        FAFolder newFolder = new()
                        {
                            Name = subfolderPath.Remove(0, pathLength),
                            RelativePath = relativePath,
                            ParentId = parentId,
                            HasItems = Directory.GetFiles(subfolderPath).Length > 0
                        };

                        FAFolderCanExistReport canExistReport = await CanCreate(newFolder);
                        if (canExistReport.CanExist)//if the item can be added try to add it
                        {
                            (bool, int) createResult = await repository.Create(newFolder);

                            if (createResult.Item1 == true) //if the item was successfully added
                            {
                                report.Added.Add(newFolder.Id);//add the folder to the added list

                                reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null

                                updateTasks.Add(Task.Run(() => updateSubfolder(subfolderPath, createResult.Item2)));//continue updating all the folders subfolders
                            }
                            else //if the item wasn't successfully added
                            {
                                report.FailedToAdd.Add(newFolder.RelativePath);//add the folder to the failed to add list
                                                                               //since continueing from this point with subfolders would at least result in folders with an incorrectly set parent no further updates are run.

                                reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null
                            }
                        }
                        else //if the item can't be added
                        {
                            report.FailedToAdd.Add(newFolder.RelativePath);//add the folder to the failed to add list
                                                                           //since continueing from this point with subfolders would at least result in folders with an incorrectly set parent no further updates are run.

                            reportUpdated = true;//set report updated to true so progressReport will report a change if it's not null
                        }

                    }
                }
            }

            #region the methods that update the IProgresses
            void updateProgressPersentageDone(object? args)
            {
                int done = 0;
                int todo = updateTasks.Count;
                for (int i = 0; i < todo; i++)
                {
                    if (updateTasks[i].IsCompleted)
                    {
                        done++;
                    }
                }
                progressPersentageDone.Report(1.0 * done / todo);
            }
            void updateProgressReport(object? args)
            {
                if (reportUpdated)
                {
                    reportUpdated = false;
                    progressReport.Report(report);
                }
            }
            #endregion
        }


        private async Task<FAFolderCanExistReport> CanExist(FAFolder toExist)
        {
            FAFolderCanExistReport report = new();

            Task<bool> parentIdExistsTask = null;
            if (toExist.ParentId == null)
            {
                report.ParentIdNull = true;
                report.ParentIdIsNotId = true;
                report.ParentIdExists = false;
            }
            else
            {
                report.ParentIdNull = false;
                if(toExist.Id == toExist.ParentId)
                {
                    report.ParentIdIsNotId = false;
                    report.ParentIdExists = true;
                }
                else
                {
                    report.ParentIdIsNotId = true;
                    parentIdExistsTask = repository.Contains(f => f.Id == toExist.ParentId);
                }
            }

            Task<bool> numberIdExistsTask = null;
            if(toExist.NumberId == null)
            {
                report.NumberIdNull = true;
                report.NumberIdExists = false;
            }
            else
            {
                report.NumberIdNull = false;
                numberIdExistsTask = numberRepository.Contains(f => f.Id == toExist.NumberId);
            }

            Task<bool> folderIdExistsTask = null;
            if(toExist.FolderId == null)
            {
                report.FolderIdNull = true;
                report.FolderIdExists = false;
            }
            else
            {
                report.FolderIdNull = false;
                folderIdExistsTask = folderRepository.Contains(f => f.Id == toExist.FolderId);
            }

            Task<bool> subfolderIdExistsTask = null;
            if(toExist.SubfolderId == null)
            {
                report.SubfolderIdNull = true;
                report.SubFolderIdExists = false;
            }
            else
            {
                report.SubfolderIdNull = false;
                subfolderIdExistsTask = subfolderRepository.Contains(f => f.Id == toExist.SubfolderId);
            }

            report.NameNotNullOrWhiteSpace = !string.IsNullOrWhiteSpace(toExist.Name);
            report.NameNotTooLong = toExist.Name.Count() <= maxLength; //has no practical max length at present but may need to be changed in future
            report.RelativePathNotNullOrWhiteSpace = !string.IsNullOrWhiteSpace(toExist.RelativePath);
            report.RelativePathNotTooLong = toExist.RelativePath.Count() <= maxLength; //has no practical max length at present but may need to be changed in future

            report.CommandsNotNull = toExist.Commands != null;
            if(toExist.Commands == null)
            {
                report.CommandsNotNull = false;
                report.CommandsAreValid = true; //todo: this will probably have to be handled by the command handler, probably in the form of a static method
                report.CommandsNotTooLong = true;
            }
            else
            {
                report.CommandsNotNull = true;
                report.CommandsAreValid = true; //todo: this will probably have to be handled by the command handler, probably in the form of a static method
                report.CommandsNotTooLong = toExist.Commands.Count() <= maxLength; //has no practical max length at present but may need to be changed in future
            }
            
            if(toExist.Notes == null)
            {
                report.NotesNotNull = false;
                report.NotesNotTooLong = true;
            }
            else
            {
                report.NotesNotNull = true;
                report.NotesNotTooLong = toExist.Notes.Count() <= maxLength; //has no practical max length at present but may need to be changed in future
            }

            if(parentIdExistsTask != null)
            {
                report.ParentIdExists = await parentIdExistsTask;
            }
            if(numberIdExistsTask != null)
            {
                report.NumberIdExists = await numberIdExistsTask;
            }
            if(folderIdExistsTask != null)
            {
                report.FolderIdExists = await folderIdExistsTask;
            }
            if(subfolderIdExistsTask != null)
            {
                report.SubFolderIdExists = await subfolderIdExistsTask;
            }

            return report;
        }

        /// <summary>
        /// returns a dictionary with all the relative paths as key and ids as value of existing FAFolders.
        /// </summary>
        /// <returns>the dictionary</returns>
        private async Task<Dictionary<string, int>> GetDictionaryWithExistingPathsAndIds()
        {
            Dictionary<string, int> existingPathsAndIds = new(); //a dictionary with every relative folder path and id so not every subfolder needs to make a call if it's not needed.
            foreach (FAFolder folder in await repository.GetAll())
            {
                existingPathsAndIds.Add(folder.RelativePath, folder.Id);
            }
            return existingPathsAndIds;
        }
    }
}
