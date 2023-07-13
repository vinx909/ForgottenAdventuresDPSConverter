using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IDpsNumberService
    {
        /// <summary>
        /// tests if an DpsNumber can be created
        /// </summary>
        /// <param name="toCreate">the folder that's to be tested if it can be created</param>
        /// <returns>a task containing an DpsNumberCanExistReport which contains .CanExist which tells if the folder can be created and data on why it can or can't</returns>
        public Task<DpsNumberCanExistReport> CanCreate(DpsNumber toCreate);

        /// <summary>
        /// tests if an DpsNumber can be created
        /// </summary>
        /// <param name="toUpdate">the number that's to be tested if it can be updated</param>
        /// <returns>a task containing an DpsNumberCanExistReport which contains .CanExist which tells if the number can be updated and data on why it can or can't</returns>
        public Task<DpsNumberCanExistReport> CanUpdate(DpsNumber toUpdate);

        /// <summary>
        /// returns an DpsNumber with the given id
        /// </summary>
        /// <param name="id">the id of the number to get</param>
        /// <returns>a task with the number or null if no number matched the id</returns>
        public Task<DpsNumber> Get(int id);

        /// <summary>
        /// returns each DpsNumber in the repository
        /// </summary>
        /// <returns>a task with an IEnumerable of DpsNumber with each number</returns>
        public Task<IEnumerable<DpsNumber>> GetAll();

        /// <summary>
        /// creates an DpsNumber to the repository
        /// </summary>
        /// <param name="toCreate">the number to create</param>
        /// <returns>a task with a boolean which is true if the number was added and false if it was not</returns>
        public Task<bool> Create(DpsNumber toCreate);

        /// <summary>
        /// updates an DpsNumber
        /// </summary>
        /// <param name="toUpdate">the number to update</param>
        /// <returns>a task with a boolean that's true if the number was updated and false if it wasn't</returns>
        public Task<bool> Update(DpsNumber toUpdate);

        /// <summary>
        /// deletes an DpsNumber in the repository with the same id, this will only work if no items have a foreign key to the number that is to be deleted.
        /// </summary>
        /// <param name="id">the id of the number to delete</param>
        /// <returns>a task with a boolean that's true if the number was deleted and false if it was not</returns>
        public Task<bool> Delete(int id);

        /// <summary>
        /// deletes an DpsNumber in the repository with the same id and set any references to the number to null instead.
        /// </summary>
        /// <param name="id">the id of the number to delete</param>
        /// <returns>a task with a boolean that's true if the number was deleted and false if it was not</returns>
        public Task<bool> DeleteAndSetReferencesToNull(int id);
    }
}
