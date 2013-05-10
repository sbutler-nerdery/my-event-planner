using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Web.Data
{
    /// <summary>
    /// A contract for interacting with the underlying database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get all objects from the repository
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();
        /// <summary>
        /// Get all objects from the repository, appending the specified includes.
        /// </summary>
        /// <param name="includes">The array of child entities to include in the query</param>
        /// <returns></returns>
        IQueryable<T> GetAll(params string[] includes);
        /// <summary>
        /// Inset an entity into the repository
        /// </summary>
        /// <param name="entity">The entity to insert</param>
        void Insert(T entity);
        /// <summary>
        /// Delete an entity from the repository
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(T entity);
        /// <summary>
        /// Save changes to the repository
        /// </summary>
        void SubmitChanges();
    }
}
