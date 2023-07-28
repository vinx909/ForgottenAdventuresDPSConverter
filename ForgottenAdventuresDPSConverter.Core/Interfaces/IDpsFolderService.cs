using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IDpsFolderService
    {
        /// <summary>
        /// tests if an DpsFolder can be created
        /// </summary>
        /// <param name="toCreate">the folder that's to be tested if it can be created</param>
        /// <returns>a task containing an DpsFolderCanExistReport which contains .CanExist which tells if the folder can be created and data on why it can or can't</returns>
        public Task<DpsFolderCanExistReport> CanCreate(DpsFolder toCreate);

        /// <summary>
        /// tests if an DpsFolder can be created
        /// </summary>
        /// <param name="toUpdate">the folder that's to be tested if it can be updated</param>
        /// <returns>a task containing an DpsFolderCanExistReport which contains .CanExist which tells if the folder can be updated and data on why it can or can't</returns>
        public Task<DpsFolderCanExistReport> CanUpdate(DpsFolder toUpdate);

        /// <summary>
        /// returns an DpsFolder with the given id
        /// </summary>
        /// <param name="id">the id of the folder to get</param>
        /// <returns>a task with the folder or null if no folder matched the id</returns>
        public Task<DpsFolder> Get(int id);

        /// <summary>
        /// returns each DpsFolder in the repository
        /// </summary>
        /// <returns>a task with an IEnumerable of DpsFolder with each folder</returns>
        public Task<IEnumerable<DpsFolder>> GetAll();

        /// <summary>
        /// returns each DPSFolder where the Discription contains the given String
        /// </summary>
        /// <param name="descriptionPart">the string the will be searched for in the destription</param>
        /// <returns>a task with an IEnumerable of DpsFolder with each folder with a match</returns>
        public Task<IEnumerable<DpsFolder>> GetAllWhereDescriptionContains(string descriptionPart);

        /// <summary>
        /// returns each DPSFolder where the Discription contains any of the given Strings
        /// </summary>
        /// <param name="descriptionParts">the strings the will be searched for in the destription</param>
        /// <returns>a task with an IEnumerable of DpsFolder with each folder with a match</returns>
        public Task<IEnumerable<DpsFolder>> GetAllWhereDescriptionContains(IEnumerable<string> descriptionParts);

        /// <summary>
        /// returns each DPSFolder where the Name contains the given String
        /// </summary>
        /// <param name="namePart">the string the will be searched for in the Names</param>
        /// <returns>a task with an IEnumerable of DpsFolder with each folder with a match</returns>
        public Task<IEnumerable<DpsFolder>> GetAllWhereNameContains(string namePart);

        /// <summary>
        /// returns each DPSFolder where the Name contains any of the given Strings
        /// </summary>
        /// <param name="nameParts">the strings the will be searched for in the name</param>
        /// <returns>a task with an IEnumerable of DpsFolder with each folder with a match</returns>
        public Task<IEnumerable<DpsFolder>> GetAllWhereNameContains(IEnumerable<string> nameParts);

        /// <summary>
        /// creates an DpsFolder to the repository
        /// </summary>
        /// <param name="toCreate">the folder to create</param>
        /// <returns>a task with a boolean which is true if the folder was added and false if it was not</returns>
        public Task<bool> Create(DpsFolder toCreate);

        /// <summary>
        /// updates an DpsFolder
        /// </summary>
        /// <param name="toUpdate">the folder to update</param>
        /// <returns>a task with a boolean that's true if the folder was updated and false if it wasn't</returns>
        public Task<bool> Update(DpsFolder toUpdate);

        /// <summary>
        /// deletes an DpsFolder in the repository with the same id, this will only work if no items have a foreign key to the folder that is to be deleted.
        /// </summary>
        /// <param name="id">the id of the folder to delete</param>
        /// <returns>a task with a boolean that's true if the folder was deleted and false if it was not</returns>
        public Task<bool> Delete(int id);

        /// <summary>
        /// deletes an DpsFolder in the repository with the same id and set any references to the folder to null instead.
        /// </summary>
        /// <param name="id">the id of the folder to delete</param>
        /// <returns>a task with a boolean that's true if the folder was deleted and false if it was not</returns>
        public Task<bool> DeleteAndSetReferencesToNull(int id);
    }
}
