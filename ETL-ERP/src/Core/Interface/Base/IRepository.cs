using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Interface.Base
{
    public interface IRepository<T> where T : class
    {
        ICollection<T> GetAll();
        Task<ICollection<T>> GetAllAsync();

        T GetById(long id);
        Task<T> GetByIdAsync(long id);

        void Add(T entity);
        Task AddAsync(T entity);

        void AddRange(ICollection<T> entities);
        Task AddRangeAsync(ICollection<T> entities);


        void Update(T entity);
        Task UpdateAsync(T entity);

        void UpdateRange(ICollection<T> entities);
        Task UpdateRangeAsync(ICollection<T> entities);

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
        T GetFirstOrDefault();
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate);
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> GetFirstOrDefaultAsync();
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);


        T GetLastOrDefault(Expression<Func<T, bool>> predicate);
        T GetLastOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);


        int GetMaxAutoCode(string tableName);
        int GetMaxAutoCode(string tableName, string columnName);
        int GetMaxAutoCodeOracle(string tableName, string columnName);
        long GetNextSeqValueOracle(string sequenceName);
        long GetNextSeqValue(string sequenceName);

        ICollection<T> GetDataListBySqlQuery(string sqlQuery);
        T GetDataBySqlQuery(string sqlQuery);
        dynamic GetAnyDataBySqlQuery(string sqlQuery);

        string GetMaxAutoCode(string tableName, string columnName = null, string prefix = null, int? howManyDigit = null, string suffix = null, bool? isPrefixFilter = false);
        //TransactionScope TsOpen(bool transactionScopeAsyncFlowOption = true);
        //bool TsComplete(TransactionScope ts);
        string GetStringDataBySqlQuery(string sqlQuery);
    }
}
