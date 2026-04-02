using AutoMapper;
using Interface.Repository.Common;
using Persistence.DapperModel;

namespace Repository
{
    public class AutoCodeRepository : IAutoCodeRepository
    {
        #region Config

        private readonly IMapper _iMapper;
        private readonly IApplicationReadDbConnection _iReadDbConnection;

        public AutoCodeRepository(IMapper iMapper, IApplicationReadDbConnection iReadDbConnection)
        {
            _iMapper = iMapper;
            _iReadDbConnection = iReadDbConnection;
        }

        #endregion

        #region AccountingModule

        public async Task<string> GetAccHeadCodeAsync(long groupId, long? parentId)
        {
            var query = String.Empty;

            if (parentId == 0)
            {
                query = $@"
                    select CONCAT(CONVERT(INT,ag.GroupCode),'.',(select isnull(max(HeadGroupCode),0)+1 from AccHeads where ParentHeadId = 0 and GroupId = {groupId})) code from AccGroups ag 
                    left join AccHeads ah on ah.GroupId = ag.Id where ag.Id = {groupId}";
            }
            else
            {
                query = $@"select CONCAT(HeadCode,'.',(select isnull(max(HeadGroupCode),0)+1 from AccHeads where ParentHeadId = {parentId})) code
                            from AccHeads
                            where Id = {parentId}";
            }


            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            return data;
        }

        public async Task<long> GetAccHeadGroupCode(long groupId, long? parentId)
        {
            var query = $"select isnull(max(HeadGroupCode),0)+1 code from AccHeads where ParentHeadId = {parentId} and GroupId = {groupId}";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);

            if (!string.IsNullOrEmpty(data))
            {
                return Convert.ToInt64(data);
            }
            return 0;
        }

        public async Task<string> GetAccLedgerCode(long headId)
        {
            var query = $@"select CONCAT('L',ah.HeadCode,'.',(select isnull(max(LedgerGroupCode),0)+1 from AccLedgers where HeadId = {headId})) CODE
                            from AccHeads ah
                            left join AccLedgers al on al.HeadId = ah.Id
                            where ah.Id = {headId}";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            return data;
        }

        public async Task<long> GetAccLedgerGroupCode(long? headId)
        {
            var query = $"select isnull(max(LedgerGroupCode),0)+1 code from AccLedgers where HeadId = {headId}";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);

            if (!string.IsNullOrEmpty(data))
            {
                return Convert.ToInt64(data);
            }
            return 0;
        }

        public async Task<string> GetVoucherAutoNo(string prefix = null, DateTime? vcDate = null)
        {
            var dateStr = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            if (vcDate != null)
            {
                dateStr = vcDate?.ToString("MM/dd/yyyy HH:mm:ss");
            }
            var query = $@"SELECT '{prefix}/'+RIGHT(REPLICATE('0', 6) + CONVERT(VARCHAR,ISNULL(MAX(CONVERT(BIGINT,SUBSTRING(VcNo,4,6))),0)+1), 6)+'/'+FORMAT(GETDATE(),'yy') FROM AccTranMsts where SUBSTRING(VcNo,1,2) = '{prefix}' AND YEAR(VcDate) = year('{dateStr}')";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            return data;
        }

        public async Task<string> GetLastVoucherNo(string prefix = null, DateTime? vcDate = null)
        {
            var dateStr = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            if (vcDate != null)
            {
                dateStr = vcDate?.ToString("MM/dd/yyyy HH:mm:ss");
            }
            var query = $@"SELECT RIGHT(REPLICATE('0', 6) + CONVERT(VARCHAR,ISNULL(MAX(CONVERT(BIGINT,SUBSTRING(VcNo,4,6))),0)+1), 6) FROM AccTranMsts where SUBSTRING(VcNo,1,2) = '{prefix}' AND YEAR(VcDate) = year('{dateStr}')";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            return data;
        }

        #endregion

        #region GetMaxAutoCode

        public async Task<int> GetMaxAutoCode(string tableName)
        {
            var query = $"SELECT ISNULL(MAX(AutoGenNumber),0) FROM {tableName}";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            if (!string.IsNullOrEmpty(data))
            {
                return Convert.ToInt32(data);
            }
            return 0;
        }

        #endregion

        #region GetNextSequentialValue

        public async Task<long> GetNextSeqValue(string sequenceName)
        {
            var query = $@"SELECT NEXT VALUE FOR {sequenceName}";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<long>(query);
            return (data > 0) ? Convert.ToInt64(data) : 0;
        }

        #endregion

        #region GetMaxAutoCodeOverloading

        public async Task<string> GetMaxAutoCode(string tableName, string columnName = null, string prefix = null, int? howManyDigit = null, string suffix = null, bool? isPrefixFilter = false)
        {

            //var monthPrefix = DateTime.Now.ToString("MM") + DateTime.Now.ToString("yy");
            prefix = string.IsNullOrEmpty(prefix) && isPrefixFilter == false ? "S" : prefix;

            howManyDigit ??= 3;
            var startPosition = prefix?.Length + 1 ?? 1;
            var subStringLength = prefix?.Length ?? 0;
            columnName = string.IsNullOrEmpty(columnName) ? "AutoGenCode" : columnName;

            var query = $"SELECT ISNULL(MAX(CONVERT(BIGINT, SUBSTRING(CONVERT(varchar(100),{columnName}),{startPosition},LEN({columnName})-{subStringLength}))),0)+1 FROM {tableName} WHERE 1 = 1";
            if (!string.IsNullOrEmpty(prefix) && isPrefixFilter == true) query += $" AND SUBSTRING({columnName},1,{subStringLength}) ='{prefix}'";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<long>(query);
            var maxNo = data > 0 ? data : 0;


            var autoGenCode = (prefix ?? "") + string.Format("{0:D" + howManyDigit + "}", maxNo) + (suffix ?? ""); // Dont use resharper for this Line {}

            return autoGenCode;
        }

        #endregion

        #region TransactionAutoCode

        public async Task<string> GetTransactionCode(string codeType, string tranType)
        {
            //-- code type should be 3 char

            var query = $@"SELECT '{codeType}'+RIGHT('000000'+CONVERT(VARCHAR(6), ISNULL(MAX(CONVERT(NUMERIC,SUBSTRING(TranNo,4,6))),0)+1),6)+'/'+CONVERT(VARCHAR(2),(YEAR(GETDATE()) % 100)) 
            NextTranNo FROM TranMsts WHERE (YEAR(TranDate) % 100) = (YEAR(GETDATE()) % 100) AND TranType = '{tranType}'";

            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            return data;
        }

        #endregion

        #region GetHtPaymentsAutoCode

        public async Task<string> GetHtPaymentsAutoCode(string tableName, string columnName = null, string prefix = null, int? howManyDigit = null, string suffix = null, bool? isPrefixFilter = false)
        {

            //var monthPrefix = DateTime.Now.ToString("MM") + DateTime.Now.ToString("yy");
            prefix = string.IsNullOrEmpty(prefix) && isPrefixFilter == false ? "S" : prefix;

            howManyDigit ??= 3;
            var startPosition = prefix?.Length + 1 ?? 1;
            var subStringLength = prefix?.Length ?? 0;
            columnName = string.IsNullOrEmpty(columnName) ? "AutoGenCode" : columnName;

            var query = $"SELECT ISNULL(MAX(TRY_CONVERT(BIGINT, SUBSTRING({columnName},{startPosition},{howManyDigit}))), 1003) + 1 FROM {tableName} where id > 7320";
            if (!string.IsNullOrEmpty(prefix) && isPrefixFilter == true) query += $" AND SUBSTRING({columnName},1,{subStringLength}) ='{prefix}'";
            var data = await _iReadDbConnection.QueryFirstOrDefaultAsync<long>(query);
            var maxNo = data > 0 ? data : 0;


            var autoGenCode = (prefix ?? "") + string.Format("{0:D" + howManyDigit + "}", maxNo) + (suffix ?? ""); // Dont use resharper for this Line {}

            return autoGenCode;
        }

        #endregion
    }
}
