using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IFAFolderService
    {
        /// <summary>
        /// tests if an FAFolder can be created
        /// </summary>
        /// <param name="toCreate">the folder that's to be tested if it can be created</param>
        /// <returns>a task containing an FAFolderCanExistReport which contains .CanExist which tells if the folder can be created and data on why it can or can't</returns>
        public Task<FAFolderCanExistReport> CanCreate(FAFolder toCreate);

        /// <summary>
        /// tests if an FAFolder can be created
        /// </summary>
        /// <param name="toUpdate">the folder that's to be tested if it can be updated</param>
        /// <returns>a task containing an FAFolderCanExistReport which contains .CanExist which tells if the folder can be updated and data on why it can or can't</returns>
        public Task<FAFolderCanExistReport> CanUpdate(FAFolder toUpdate);

        /// <summary>
        /// creates an FAFolder to the repository
        /// </summary>
        /// <param name="toCreate">the folder to create</param>
        /// <returns>a task with a boolean which is true if the folder was added and false if it was not</returns>
        public Task<bool> Create(FAFolder toCreate);

        /// <summary>
        /// deletes an FAFolder in the repository with the same id. this only works if no folders have the folder that is to be deleted as their parent
        /// </summary>
        /// <param name="id">the id of the folder to delete</param>
        /// <returns>a task with a boolean that's true if the folder was deleted and false if it was not</returns>
        public Task<bool> Delete(int id);

        /// <summary>
        /// deletes an FAFolder in the repository with the same id, and any folders that have the folder as their parent.
        /// </summary>
        /// <param name="id">the id of the folder to delete</param>
        /// <returns>a task with a boolean that's true if the folder was deleted and false if it was not</returns>
        public Task<bool> DeleteWithChildren(int id);

        /// <summary>
        /// returns an FAFolder with the given id
        /// </summary>
        /// <param name="id">the id of the folder to get</param>
        /// <returns>a task with the folder or null if no folder matched the id</returns>
        public Task<FAFolder> Get(int id);

        /// <summary>
        /// returns each FAFolder in the repository
        /// </summary>
        /// <returns>a task with an IEnumerable of FAFolder with each folder</returns>
        public Task<IEnumerable<FAFolder>> GetAll();

        /// <summary>
        /// get all folders that have the folder of the given id as their parent folder
        /// </summary>
        /// <param name="parentFolderId">the id of the parent folder</param>
        /// <returns>a task with an IEnumerable of FAFolder with all the child folders</returns>
        public Task<IEnumerable<FAFolder>> GetChildren(int parentFolderId);

        /// <summary>
        /// get all folders that don't have parents
        /// </summary>
        /// <returns>a task with an IEnumerable of FAFolder with each of the parentless folder</returns>
        public Task<IEnumerable<FAFolder>> GetParentless();

        /// <summary>
        /// get all the folders that that have items but have no DpsNumber attached
        /// </summary>
        /// <returns>a task with an IEnumerable of FAFolder with all the relavent folders</returns>
        public Task<IEnumerable<FAFolder>> GetAllWithItemsAndNoNumber();

        /// <summary>
        /// get all the folders that that have items but have no DpsFolder attached
        /// </summary>
        /// <returns>a task with an IEnumerable of FAFolder with all the relavent folders</returns>
        public Task<IEnumerable<FAFolder>> GetAllWithItemsAndNoFolder();

        /// <summary>
        /// get all the folders that that have items but have no DpsSubfolder attached
        /// </summary>
        /// <returns>a task with an IEnumerable of FAFolder with all the relavent folders</returns>
        public Task<IEnumerable<FAFolder>> GetAllWithItemsAndNoSubfolder();

        /// <summary>
        /// finds out if there is a folder that has a reference to a specific entity of type T
        /// </summary>
        /// <typeparam name="T">the type of entity for which it'll look for a reference</typeparam>
        /// <param name="id">the id of the entity for which it'll look for a reference</param>
        /// <returns>a task containing a boolean that's true if there is a folder with a reference to en entity of type T with the given id, and false if there is not</returns>
        public Task<bool> HasFolderWithReferenceTo<T>(int id);

        /// <summary>
        /// updates all folders that have a reference to type T with the given id to null
        /// </summary>
        /// <typeparam name="T">the type to which references are to be set to null</typeparam>
        /// <param name="id">the id of the item to which items are to be set to null</param>
        /// <returns>a task containing a boolean thats true if all references are correctly set to null, and false if it failed to do that</returns>
        public Task<bool> SetReferenceNull<T>(int id);

        /// <summary>
        /// updates an FAFolder
        /// </summary>
        /// <param name="toUpdate">the folder to update</param>
        /// <returns>a task with a boolean that's true if the folder was updated and false if it wasn't</returns>
        public Task<bool> Update(FAFolder toUpdate);

        /// <summary>
        /// updates all folders in the repository. it adds new folders and updates existing folders that had changes.
        /// </summary>
        /// <param name="folderPath">the folderpath to the folder to update the repository with</param>
        /// <returns>a task containing an FAFolderUpdateReport containing information with how many and what folders were added, updated,  untouched, failed to add and failed to update, as well as what folders no longer exist in the file structure which maybe should be deleted.</returns>
        public Task<FAFolderUpdateReport> UpdateFolders(string folderPath)
        {
            return UpdateFolders(folderPath, null, null);
        }

        /// <summary>
        /// updates all folders in the repository. it adds new folders and updates existing folders that had changes.
        /// </summary>
        /// <param name="folderPath">the folderpath to the folder to update the repository with</param>
        /// <param name="progressPersentageDone">an IProgress\<double\> to keep track of how far the process is done. at 0 at the start, 0.5 at 50% and 1 at 100%</param>
        /// <returns>a task containing an FAFolderUpdateReport containing information with how many and what folders were added, updated,  untouched, failed to add and failed to update, as well as what folders no longer exist in the file structure which maybe should be deleted.</returns>
        public Task<FAFolderUpdateReport> UpdateFolders(string folderPath, IProgress<double> progressPersentageDone)
        {
            return UpdateFolders(folderPath, progressPersentageDone, null);
        }

        /// <summary>
        /// updates all folders in the repository. it adds new folders and updates existing folders that had changes.
        /// </summary>
        /// <param name="folderPath">the folderpath to the folder to update the repository with</param>
        /// <param name="progressReport">an IProgress\<FolderUpdateReport\> that gives an update whenever there's a change to the report if you want to keep a very close eye on the process.</FolderUpdateReport></param>
        /// <returns>a task containing an FAFolderUpdateReport containing information with how many and what folders were added, updated,  untouched, failed to add and failed to update, as well as what folders no longer exist in the file structure which maybe should be deleted.</returns>
        public Task<FAFolderUpdateReport> UpdateFolders(string folderPath, IProgress<FAFolderUpdateReport> progressReport)
        {
            return UpdateFolders(folderPath, null, progressReport);
        }

        /// <summary>
        /// updates all folders in the repository. it adds new folders and updates existing folders that had changes.
        /// </summary>
        /// <param name="folderPath">the folderpath to the folder to update the repository with</param>
        /// <param name="progressPersentageDone">an IProgress\<double\> to keep track of how far the process is done. at 0 at the start, 0.5 at 50% and 1 at 100%</param>
        /// <param name="progressReport">an IProgress\<FolderUpdateReport\> that gives an update whenever there's a change to the report if you want to keep a very close eye on the process.</FolderUpdateReport></param>
        /// <returns>a task containing an FAFolderUpdateReport containing information with how many and what folders were added, updated,  untouched, failed to add and failed to update, as well as what folders no longer exist in the file structure which maybe should be deleted.</returns>
        public Task<FAFolderUpdateReport> UpdateFolders(string folderPath, IProgress<double> progressPersentageDone, IProgress<FAFolderUpdateReport> progressReport);
    }
}
