using System.Linq.Expressions;

namespace Interface.Base
{
    public interface IService<T> where T : class
    {
        long CurrentUserId { get; set; }
        string BaseUrl { get; set; }

        ICollection<T> GetAll();

        T GetById(long id);

        TR GetById<TR>(long id) where TR : class;
        Task<T> GetByIdAsync(long id, bool isTracking = true);
        Task<TR> GetByIdAsync<TR>(long id) where TR : class;

        bool Add(T entity);
        Task<bool> AddAsync(T entity);

        void AddRange(ICollection<T> entities);
        Task<bool> AddRangeAsync(ICollection<T> entities);


        bool Update(T entity);
        Task<bool> UpdateAsync(T entity);

        void UpdateRange(ICollection<T> entities);
        Task<bool> UpdateRangeAsync(ICollection<T> entities);

        void AddOrUpdate(Expression<Func<T, object>> identifier, ICollection<T> entityCollections);

        void Remove(T entity);
        Task RemoveAsync(T entity);
        void RemoveRange(ICollection<T> entities);
        Task RemoveRangeAsync(ICollection<T> entities);

        ICollection<T> Get(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetQueryableAsync(Expression<Func<T, bool>> predicate);
        ICollection<T> Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        T GetSingleOrDefault(Expression<Func<T, bool>> predicate);
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate);
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> GetFirstOrDefaultAsync();
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);


        T GetLastOrDefault(Expression<Func<T, bool>> predicate);
        T GetLastOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);


        ICollection<T> GetDataListBySqlQuery(string sqlQuery);
        T GetDataBySqlQuery(string sqlQuery);

        int GetMaxAutoCode(string tableName);
        int GetMaxAutoCode(string tableName, string columnName);
        int GetMaxAutoCode(string tableName, string columnName, bool isOracle);
        int GetMaxAutoCodeOracle(string tableName, string columnName);
        long GetNextSeqValueOracle(string sequenceName);
        long GetNextSeqValue(string sequenceName);

        dynamic GetAnyDataBySqlQuery(string sqlQuery);

        string GetDefaultAutoCode(string prefix);
        string GetMaxAutoCode(string tableName, string columnName = null, string prefix = null, int? howManyDigit = null, string suffix = null, bool? isPrefixFilter = false);

        //TransactionScope TsOpen(bool transactionScopeAsyncFlowOption = true);
        //bool TsComplete(TransactionScope ts);
        string GetStringDataBySqlQuery(string sqlQuery);
    }
}
