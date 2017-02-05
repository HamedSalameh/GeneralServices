using GeneralServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace GeneralServices.Components
{
    public class GenericRepositoryBase<T, DbCtx> : IGenericRepositoryBase<T> where T : class where DbCtx : DbContext, new()
    {
        public virtual IList<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;

            using (var context = new DbCtx())
            {
                try
                {
                    IQueryable<T> query = context.Set<T>();

                    foreach (Expression<Func<T, object>> navigationParam in navigationProperties)
                    {
                        query = query.Include<T, object>(navigationParam);
                    }

                    list = query.AsNoTracking().ToList<T>();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }

            return list;
        }

        public virtual IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list = null;

            using (var context = new DbCtx())
            {
                try
                {
                    IQueryable<T> query = context.Set<T>();

                    foreach (Expression<Func<T, object>> navigationParam in navigationProperties)
                    {
                        query = query.Include<T, object>(navigationParam);
                    }

                    list = query.AsNoTracking().Where(where).ToList<T>();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }

            return list;
        }

        public virtual T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T result;

            using (var context = new DbCtx())
            {
                try
                {
                    IQueryable<T> query = context.Set<T>();

                    foreach (Expression<Func<T, object>> navigationParam in navigationProperties)
                    {
                        query = query.Include<T, object>(navigationParam);
                    }

                    result = query.AsNoTracking().FirstOrDefault(where);
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }

            return result;
        }

        public virtual void Add(params T[] items)
        {
            Update(items);
        }

        public virtual void Remove(params T[] items)
        {
            Update(items);
        }

        public virtual void Update(params T[] items)
        {
            using (var context = new DbCtx())
            {
                try
                {
                    DbSet<T> set = context.Set<T>();

                    foreach (var item in items)
                    {
                        set.Add(item);
                        foreach (DbEntityEntry<IEntity> entry in context.ChangeTracker.Entries<IEntity>())
                        {
                            IEntity entity = entry.Entity;
                            entry.State = GetEntityState(entity.EntityState);
                        }
                    }

                    context.SaveChanges();
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
            }
        }

        protected virtual System.Data.Entity.EntityState GetEntityState(Interfaces.EntityState entityState)
        {
            switch (entityState)
            {
                case Interfaces.EntityState.Unchanged:
                    return System.Data.Entity.EntityState.Unchanged;
                case Interfaces.EntityState.Added:
                    return System.Data.Entity.EntityState.Added;
                case Interfaces.EntityState.Modified:
                    return System.Data.Entity.EntityState.Modified;
                case Interfaces.EntityState.Deleted:
                    return System.Data.Entity.EntityState.Deleted;
                default:
                    return System.Data.Entity.EntityState.Detached;
            }
        }
    }
}
