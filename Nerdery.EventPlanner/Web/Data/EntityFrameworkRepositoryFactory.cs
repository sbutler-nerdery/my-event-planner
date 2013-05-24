using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web.Data
{
    public class EntityFrameworkRepositoryFactory : IRepositoryFactory
    {
        private readonly DbContext _context;

        public EntityFrameworkRepositoryFactory(DbContext context)
        {
            _context = context;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EntityFrameworkRepository<T>(_context);
        }
    }
}