using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Web.Extensions
{
    internal static class QueryableExtensions
    {
        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable, params string[] includes) where T : class
        {
            if ((includes == null) || (includes.Length == 0))
                return queryable;

            return includes.Aggregate(queryable, (current, include) => current.Include(include));
        }
    }
}