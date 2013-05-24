using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Data
{
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Return a repository of the specified type.
        /// </summary>
        /// <typeparam name="T">The desired repository type</typeparam>
        /// <returns></returns>
        IRepository<T> GetRepository<T>() where T : class;
    }
}
