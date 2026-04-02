using Interface.Base;
using Interface.UnitOfWork;
using System.Linq.Expressions;

namespace Services.Base
{
    public class BaseService<T> : IService<T> where T : class
    {
        protected IRepository<T> MapRepository;
        private readonly IUnitOfWork _iUnitOfWork;
        public long CurrentUserId { get; set; }
        public string BaseUrl { get; set; }

        public BaseService() { }

        public BaseService(IRepository<T> infoMapRepository, IUnitOfWork iUnitOfWork)
        {
            MapRepository = infoMapRepository;
            _iUnitOfWork = iUnitOfWork;
        }

        public bool Add(T entity)
        {
            MapRepository.Add(entity);
            var result = _iUnitOfWork.Complete();
            return result;
        }

        public async Task<bool> AddAsync(T entity)
        {
            await MapRepository.AddAsync(entity);
            var result = await _iUnitOfWork.CompleteAsync();
            return result;
        }

        public void AddOrUpdate(Expression<Func<T, object>> identifier, ICollection<T> entityCollections)
        {
            MapRepository.AddOrUpdate(identifier, entityCollections);
        }

        public void AddRange(ICollection<T> entities)
        {
            MapRepository.AddRange(entities);
        }

        public async Task<bool> AddRangeAsync(ICollection<T> entities)
        {
            await MapRepository.AddRangeAsync(entities);
            var result = await _iUnitOfWork.CompleteAsync();
            return result;
        }

        public virtual ICollection<T> GetAll()
        {
            return MapRepository.GetAll();
        }


        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            return await MapRepository.GetAllAsync();
        }


        public virtual T GetById(long id)
        {
            return MapRepository.GetById(id);
        }

        public virtual async Task<T> GetByIdAsync(long id, bool isTracking = true)
        {
            return await MapRepository.GetByIdAsync(id);
        }

        public virtual Task<TR> GetByIdAsync<TR>(long id) where TR : class => null;

        public virtual TR GetById<TR>(long id) where TR : class
        {
            return null;
        }

        public bool Update(T entity)
        {
            MapRepository.Update(entity);
            var result = _iUnitOfWork.Complete();
            return result;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            await MapRepository.UpdateAsync(entity);
            var result = await _iUnitOfWork.CompleteAsync();
            return result;
        }

        public async Task<bool> UpdateRangeAsync(ICollection<T> entities)
        {
            await MapRepository.UpdateRangeAsync(entities);
            var result = await _iUnitOfWork.CompleteAsync();
            return result;
        }

        public void Remove(T entity)
        {
            MapRepository.Remove(entity);
        }

        public virtual async Task RemoveAsync(T entity)
        {
            await MapRepository.RemoveAsync(entity);
        }


        public void UpdateRange(ICollection<T> entities)
        {
            MapRepository.UpdateRange(entities);
        }

        public virtual ICollection<T> Get(Expression<Func<T, bool>> predicate)
        {
            return MapRepository.Get(predicate);
        }

        public virtual Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return MapRepository.GetAsync(predicate);
        }

        public IQueryable<T> GetQueryableAsync(Expression<Func<T, bool>> predicate)
        {
            return MapRepository.GetQueryableAsync(predicate);
        }

        public virtual ICollection<T> Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return MapRepository.Get(predicate, includes);
        }


        public void RemoveRange(ICollection<T> deletableDetails)
        {
            MapRepository.RemoveRange(deletableDetails);
        }

        public virtual async Task RemoveRangeAsync(ICollection<T> entities)
        {
            await MapRepository.RemoveRangeAsync(entities);
        }

        public virtual T GetSingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return MapRepository.GetSingleOrDefault(predicate);
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return MapRepository.GetFirstOrDefault(predicate);
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return MapRepository.GetFirstOrDefault(predicate, includes);
        }

        public virtual async Task<T> GetFirstOrDefaultAsync()
        {
            return await MapRepository.GetFirstOrDefaultAsync();
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await MapRepository.GetFirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await MapRepository.GetFirstOrDefaultAsync(predicate, includes);
        }


        public virtual T GetLastOrDefault(Expression<Func<T, bool>> predicate)
        {
            return MapRepository.GetLastOrDefault(predicate);
        }

        public virtual T GetLastOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return MapRepository.GetLastOrDefault(predicate, includes);
        }


        public virtual async Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            return await MapRepository.GetAsync(predicate, includes);
        }


        public virtual int GetMaxAutoCode(string tableName)
        {
            var data = MapRepository.GetMaxAutoCode(tableName);
            return data;
        }


        public virtual int GetMaxAutoCodeOracle(string tableName, string columnName)
        {
            var data = MapRepository.GetMaxAutoCodeOracle(tableName, columnName);
            return data;
        }

        public virtual long GetNextSeqValueOracle(string sequenceName)
        {
            var data = MapRepository.GetNextSeqValueOracle(sequenceName);
            return data;
        }

        public long GetNextSeqValue(string sequenceName)
        {
            var data = MapRepository.GetNextSeqValue(sequenceName);
            return data;
        }


        public virtual int GetMaxAutoCode(string tableName, string columnName, bool isOracle)
        {
            if (isOracle && !string.IsNullOrEmpty(tableName))
            {
                var data = MapRepository.GetMaxAutoCodeOracle(tableName, columnName);
                return data;
            }
            if (!string.IsNullOrEmpty(columnName))
            {
                var data = MapRepository.GetMaxAutoCode(tableName, columnName);
                return data;
            }
            if (!string.IsNullOrEmpty(tableName))
            {
                var data = MapRepository.GetMaxAutoCode(tableName);
                return data;
            }

            return 0;
        }

        public virtual int GetMaxAutoCode(string tableName, string columnName)
        {
            var data = MapRepository.GetMaxAutoCode(tableName, columnName);
            return data;
        }

        public virtual ICollection<T> GetDataListBySqlQuery(string sqlQuery)
        {
            var dataList = MapRepository.GetDataListBySqlQuery(sqlQuery);
            return dataList;
        }

        public virtual T GetDataBySqlQuery(string sqlQuery)
        {
            var data = MapRepository.GetDataBySqlQuery(sqlQuery);
            return data;
        }

        public virtual dynamic GetAnyDataBySqlQuery(string sqlQuery)
        {
            var data = MapRepository.GetAnyDataBySqlQuery(sqlQuery);
            return data;
        }

        public string GetDefaultAutoCode(string prefix)
        {
            var data = $"{prefix}-{DateTime.Now:dMyhmss}";
            return data;
        }

        public string GetStringDataBySqlQuery(string sqlQuery)
        {
            var data = MapRepository.GetStringDataBySqlQuery(sqlQuery);
            return data;
        }

        public string GetMaxAutoCode(string tableName, string columnName = null, string prefix = null, int? howManyDigits = null, string suffix = null, bool? isPrefixFilter = false)
        {
            var data = MapRepository.GetMaxAutoCode(tableName, columnName, prefix, howManyDigits, suffix, isPrefixFilter);
            return data;
        }

    }
}
