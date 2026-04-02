using Interface.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Repository.Base
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected BaseRepository(DbContext context)
        {
            Db = context;
        }

        public DbSet<T> Table
        {
            get
            {
                var set = Db.Set<T>();
                return set;
            }
        }

        protected DbContext Db { get; set; }

        protected DbSet<T> Set => Db.Set<T>();

        public void Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException();
            Set.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException();
            await Set.AddAsync(entity);
        }

        public void AddOrUpdate(Expression<Func<T, object>> identifier, ICollection<T> entityCollections)
        {
            throw new NotImplementedException();
        }

        public void AddRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count <= 0) throw new ArgumentNullException();
            Set.AddRange(entities);
        }

        public async Task AddRangeAsync(ICollection<T> entities)
        {
            if (entities == null || entities.Count <= 0) throw new ArgumentNullException();
            await Set.AddRangeAsync(entities);
        }

        public virtual ICollection<T> Get(Expression<Func<T, bool>> predicate)
        {
            var result = Set.Where(predicate).ToList();
            return result;
        }


        public virtual async Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await Set.Where(predicate).ToListAsync();
            return result;
        }

        public IQueryable<T> GetQueryableAsync(Expression<Func<T, bool>> predicate)
        {
            var result = Set.Where(predicate);
            return result;
        }

        public virtual T GetSingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            var result = Set.SingleOrDefault(predicate);
            return result;
        }

        public virtual T GetFirstOrDefault()
        {
            var result = Set.FirstOrDefault();
            return result;
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var result = Set.FirstOrDefault(predicate);
            return result;
        }

        public virtual async Task<T> GetFirstOrDefaultAsync()
        {
            var result = await Set.FirstOrDefaultAsync();
            return result;
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any())
            {
                return Set.FirstOrDefault(predicate);
            }


            var result = includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include), c => c.FirstOrDefault(predicate));

            return result;
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any())
            {
                return await Set.FirstOrDefaultAsync(predicate);
            }


            var result = await includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include), c => c.FirstOrDefaultAsync(predicate));

            return result;
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = Table;

            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }


        public virtual T GetLastOrDefault(Expression<Func<T, bool>> predicate)
        {
            var result = Set.LastOrDefault(predicate);
            return result;
        }

        public virtual T GetLastOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any())
            {
                return Set.LastOrDefault(predicate);
            }


            var result = includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include), c => c.LastOrDefault(predicate));

            return result;
        }



        public virtual ICollection<T> Get(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any())
            {
                return Get(predicate);
            }


            var result = includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include), c => c.Where(predicate)).ToList();

            return result;
        }


        public virtual async Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any())
            {
                return Get(predicate);
            }


            var result = await includes.Aggregate(Table.AsQueryable(), (current, include) => current.Include(include), c => c.Where(predicate)).ToListAsync();

            return result;
        }



        public virtual void Detach(T entity)
        {
            //if (Db.Exists(entity))
            //{
            //    Db.Entry(entity).State = EntityState.Detached;
            //}
        }

        public virtual int GetMaxAutoCode(string tableName)
        {
            //var query = $"SELECT ISNULL(MAX(AutoGenNumber),0) FROM {tableName}";
            //var data = Db.Database.SqlQuery<string>(query).FirstOrDefault();
            //if (!string.IsNullOrEmpty(data))
            //{
            //    return Convert.ToInt32(data);
            //}
            return 0;
        }

        public int GetMaxAutoCode(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }

        public int GetMaxAutoCodeOracle(string tableName, string columnName)
        {
            //var query = $"SELECT NVL(MAX({columnName}),0) FROM {tableName}";
            //var data = Db.Database.SqlQuery<decimal>(query).FirstOrDefault();
            //return (data > 0) ? Convert.ToInt32(data) : 0;
            return 0;
        }

        public long GetNextSeqValueOracle(string sequenceName)
        {
            //var query = $@"SELECT {sequenceName}.nextval FROM DUAL";
            //var data = Db.Database.SqlQuery<decimal>(query).FirstOrDefault();
            //return (data > 0) ? Convert.ToInt64(data) : 0;
            return 0;
        }

        public long GetNextSeqValue(string sequenceName)
        {
            //var query = $@"SELECT NEXT VALUE FOR {sequenceName}";
            //var data = Db.Database.SqlQuery<long>(query).FirstOrDefault();
            //return (data > 0) ? Convert.ToInt64(data) : 0;
            return 0;
        }

        public virtual ICollection<T> GetDataListBySqlQuery(string sqlQuery)
        {
            //var data = Db.Database.SqlQuery<T>(sqlQuery).ToList();
            //return data;
            return new List<T>();
        }

        public virtual T GetDataBySqlQuery(string sqlQuery)
        {
            //var data = Db.Database.SqlQuery<T>(sqlQuery).FirstOrDefault();
            //return data;
            return null;
        }


        public virtual dynamic GetAnyDataBySqlQuery(string sqlQuery)
        {
            //var data = Db.Database.ExecuteSqlCommand(sqlQuery);
            //return data;
            return null;
        }


        public string GetMaxAutoCode(string tableName, string columnName = null, string prefix = null, int? howManyDigit = null, string suffix = null, bool? isPrefixFilter = false)
        {

            //var monthPrefix = DateTime.Now.ToString("MM") + DateTime.Now.ToString("yy");
            //prefix = string.IsNullOrEmpty(prefix) && isPrefixFilter == false ? "S" : prefix;

            //howManyDigit ??= 3;
            //var startPosition = prefix?.Length + 1 ?? 1;
            //var subStringLength = prefix?.Length ?? 0;
            //columnName = string.IsNullOrEmpty(columnName) ? "AutoGenCode" : columnName;

            //var query = $"SELECT ISNULL(MAX(CONVERT(BIGINT, SUBSTRING(CONVERT(varchar(100),{columnName}),{startPosition},LEN({columnName})-{subStringLength}))),0)+1 FROM {tableName} WHERE 1 = 1";
            //if (!string.IsNullOrEmpty(prefix) && isPrefixFilter == true) query += $" AND SUBSTRING({columnName},1,{subStringLength}) ='{prefix}'";
            //var data = Db.ExecSQL<long>(query).FirstOrDefault();
            //var maxNo = data > 0 ? data : 0;


            //var autoGenCode = (prefix ?? "") + string.Format("{0:D" + howManyDigit + "}", maxNo) + (suffix ?? ""); // Dont use resharper for this Line {}

            //return autoGenCode;

            throw new NotImplementedException();
        }

        public string GetStringDataBySqlQuery(string sqlQuery)
        {
            //string result;

            //using (var command = Db.Database.GetDbConnection().CreateCommand())
            //{
            //    command.CommandText = sqlQuery;
            //    Db.Database.OpenConnection();

            //    result = (string)command.ExecuteScalar();
            //}

            //return string.IsNullOrEmpty(result) ? "" : result;
            throw new NotImplementedException();
        }

        public void Remove(T entity)
        {
            if (entity == null) throw new ArgumentNullException();
            try
            {
                Set.Remove(entity);
            }
            catch (Exception e)
            {
                var msg = "not found in the ObjectStateManager";
                if (e.Message.ToLower().Contains(msg.ToLower()))
                {
                    Set.Attach(entity);
                    Set.Remove(entity);
                }
                var containMsg = "The DELETE statement conflicted with the REFERENCE constraint";
                if (e.InnerException != null && (e.InnerException.InnerException != null && (e.InnerException != null && e.InnerException.InnerException.Message.Contains(containMsg))))
                {
                    throw new Exception("Sorry! This Item Used With Another Items, For that Delete Can't Possible!");
                }

            }
        }

        public Task RemoveAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count <= 0) throw new ArgumentNullException();

            foreach (var data in entities)
            {
                Remove(data);
            }
        }

        public Task RemoveRangeAsync(ICollection<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException();
            try
            {
                Set.Attach(entity);
                Db.Entry(entity).State = EntityState.Modified;
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.ToLower().Contains("attach") || e.Message.ToLower().Contains("multiple instances"))
                {
                    Db.Entry(entity).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException();
            try
            {
                Set.Attach(entity);
                Db.Entry(entity).State = EntityState.Modified;
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.ToLower().Contains("attach") || e.Message.ToLower().Contains("multiple instances"))
                {
                    Db.Entry(entity).State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateRange(ICollection<T> entities)
        {
            if (entities == null || entities.Count <= 0) throw new ArgumentNullException();

            if (entities.Count > 0)
            {
                foreach (var entity in entities)
                {
                    Update(entity);
                }
            }
        }

        public async Task UpdateRangeAsync(ICollection<T> entities)
        {
            if (entities == null || entities.Count <= 0) throw new ArgumentNullException();

            if (entities.Count > 0)
            {
                foreach (var entity in entities)
                {
                    await UpdateAsync(entity);
                }
            }
        }

        public ICollection<T> GetAll()
        {
            return Set.ToList();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await Set.ToListAsync();
        }

        public T GetById(long id)
        {
            var data = Set.Find(id) as T;
            return data;
        }

        public async Task<T> GetByIdAsync(long id)
        {
            var data = await Set.FindAsync(id) as T;
            return data;
        }
    }
}
