using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.Core.Interfaces
{
    public interface IRepository<T>
    {
        /// <summary>
        /// chechs if the repository contains an item that match the query
        /// </summary>
        /// <param name="query">the query that items will be checked against</param>
        /// <returns>a task with a boolean that's true if a matching item exists and false if it does not</returns>
        Task<bool> Contains(Expression<Func<T, bool>> query);
        /// <summary>
        /// counts the number of items in the repository that match the query
        /// </summary>
        /// <param name="query">the query that items will be checked against</param>
        /// <returns>a task with an interger of the number of items that match the query</returns>
        Task<int> Count(Expression<Func<T, bool>> query);
        /// <summary>
        /// creates an item in the repository from toCreate T
        /// </summary>
        /// <param name="toCreate">the object to create an item from</param>
        /// <returns>a task with a boolean and an int. the boolean is true if the item was added and false if it was not, the int is the id of the added item</returns>
        Task<(bool, int)> Create(T toCreate);
        /// <summary>
        /// deletes an item in the repository with the same id
        /// </summary>
        /// <param name="id">the id of the item to delete</param>
        /// <returns>a task with a boolean that's true if an item was deleted and false if it was not</returns>
        Task<bool> Delete(int id);
        /// <summary>
        /// returns an object of each item in the repository
        /// </summary>
        /// <returns>a task with an IEnumerable of T with an object of each item</returns>
        Task<IEnumerable<T>> GetAll();
        /// <summary>
        /// return an object of each item in the repository that matches the query
        /// </summary>
        /// <param name="query">the query that items will be checked against</param>
        /// <returns>a task with an IEnumerable of T with an object of each item that matches the query</returns>
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> query);
        /// <summary>
        /// returns an object of the item with the given id
        /// </summary>
        /// <param name="id">the id of the item to make an object of</param>
        /// <returns>a task with the object based of the item or null if no object matched with an item</returns>
        Task<T> Get(int id);
        /// <summary>
        /// changes an item to the properties of an object matching on id
        /// </summary>
        /// <param name="toUpdate">the object that's the bases of the item that matches the id</param>
        /// <returns>a task with a boolean that's true if an item was updated and false if no item was found with the same id or failed to update</returns>
        Task<bool> Update(T toUpdate);
    }
}
