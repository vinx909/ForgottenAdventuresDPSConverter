using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IDpsSubfolderService
    {
        /// <summary>
        /// tests if an DpsSubfolder can be created
        /// </summary>
        /// <param name="toCreate">the folder that's to be tested if it can be created</param>
        /// <returns>a task containing an DpsSubfolderCanExistReport which contains .CanExist which tells if the folder can be created and data on why it can or can't</returns>
        public Task<DpsSubfolderCanExistReport> CanCreate(DpsSubfolder toCreate);

        /// <summary>
        /// tests if an DpsSubfolder can be created
        /// </summary>
        /// <param name="toUpdate">the subfolder that's to be tested if it can be updated</param>
        /// <returns>a task containing an DpsSubfolderCanExistReport which contains .CanExist which tells if the subfolder can be updated and data on why it can or can't</returns>
        public Task<DpsSubfolderCanExistReport> CanUpdate(DpsSubfolder toUpdate);

        /// <summary>
        /// returns an DpsSubfolder with the given id
        /// </summary>
        /// <param name="id">the id of the subfolder to get</param>
        /// <returns>a task with the subfolder or null if no subfolder matched the id</returns>
        public Task<DpsSubfolder> Get(int id);

        /// <summary>
        /// returns each DpsSubfolder in the repository
        /// </summary>
        /// <returns>a task with an IEnumerable of DpsSubfolder with each subfolder</returns>
        public Task<IEnumerable<DpsSubfolder>> GetAll();

        //todo: summary
        public Task<IEnumerable<DpsSubfolder>> GetAllWhereDescriptionContains(string descriptionPart);

        //todo: summary
        public Task<IEnumerable<DpsSubfolder>> GetAllWhereDescriptionContains(IEnumerable<string> descriptionParts);

        //todo: summary
        public Task<IEnumerable<DpsSubfolder>> GetAllWhereNameContains(string namePart);

        //todo: summary
        public Task<IEnumerable<DpsSubfolder>> GetAllWhereNameContains(IEnumerable<string> nameParts);

        /// <summary>
        /// creates an DpsSubfolder to the repository
        /// </summary>
        /// <param name="toCreate">the subfolder to create</param>
        /// <returns>a task with a boolean which is true if the subfolder was added and false if it was not</returns>
        public Task<bool> Create(DpsSubfolder toCreate);

        /// <summary>
        /// updates an DpsSubfolder
        /// </summary>
        /// <param name="toUpdate">the subfolder to update</param>
        /// <returns>a task with a boolean that's true if the subfolder was updated and false if it wasn't</returns>
        public Task<bool> Update(DpsSubfolder toUpdate);

        /// <summary>
        /// deletes an DpsSubfolder in the repository with the same id, this will only work if no items have a foreign key to the subfolder that is to be deleted.
        /// </summary>
        /// <param name="id">the id of the subfolder to delete</param>
        /// <returns>a task with a boolean that's true if the subfolder was deleted and false if it was not</returns>
        public Task<bool> Delete(int id);

        /// <summary>
        /// deletes an DpsSubfolder in the repository with the same id and set any references to the subfolder to null instead.
        /// </summary>
        /// <param name="id">the id of the subfolder to delete</param>
        /// <returns>a task with a boolean that's true if the subfolder was deleted and false if it was not</returns>
        public Task<bool> DeleteAndSetReferencesToNull(int id);
    }
}
