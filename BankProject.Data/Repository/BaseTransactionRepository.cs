﻿
using BankProject.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BankProject.Data.Repository
{
    public class BaseTransactionRepository<T> : IRepository<T> where T : class
    {
        private readonly BaseContext _dbContext;
        private readonly IRepository<T> repository;
        private IsolationLevel isolationLevel = IsolationLevel.Serializable;

        public IsolationLevel IsolationLevel
        {
            get
            {
                return isolationLevel;
            }

            set
            {
                isolationLevel = value;
            }
        }

        public BaseTransactionRepository(BaseContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException($"{nameof(dbContext)} can not be null");

            _dbContext = dbContext;
            repository = new BaseRepository<T>(_dbContext);
        }

        public IQueryable<T> GetAll()
        {
            return RunWithTransaction<IQueryable<T>>(() => { return repository.GetAll(); });
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return RunWithTransaction<IQueryable<T>>(() => { return repository.GetAll(predicate); });

        }

        public T GetById(int id)
        {
            return RunWithTransaction<T>(() => { return repository.GetById(id); });
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return RunWithTransaction<T>(() => { return repository.Get(predicate); });
        }
        public T Max(Expression<Func<T, bool>> predicate)
        {
            return RunWithTransaction<T>(() => { return repository.GetAll(predicate).Max(); });
        }
        public void Add(T entity)
        {
            RunWithTransaction(() => { repository.Add(entity); });
        }

        public void Delete(int id)
        {
            RunWithTransaction(() => { repository.Delete(id); });
        }

        public void Delete(T entity)
        {
            RunWithTransaction(() => { repository.Delete(entity); });
        }

        public void Update(T entity)
        {
            RunWithTransaction(() => { repository.Update(entity); });
        }

        public K RunWithTransaction<K>(Func<K> func)
        {
            using (_dbContext.Database.BeginTransaction(IsolationLevel))
            {
                return func();
            }
        }

        public void RunWithTransaction(Action func)
        {
            using (_dbContext.Database.BeginTransaction(IsolationLevel))
            {
                func();
            }
        }

       
    }
}
