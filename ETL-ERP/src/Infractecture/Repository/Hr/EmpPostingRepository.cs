using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpPosting;
using Interface.Repository.Hr;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Hr
{
    public class EmpPostingRepository : BaseRepository<EmpPosting>, IEmpPostingRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmpPostingRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region GetEmployeePosting

        public async Task<List<EmpPostingVm>> GetPostingByEmpId(long empId)
        {
            if (empId <= 0) return new List<EmpPostingVm>();

            var data = await Context.EmpPostings
                .Include(dpt => dpt.Department)
                .Include(pdpt => pdpt.PreDepartment)
                .Include(des => des.Designation)
                .Include(pdes => pdes.PreDesignation)
                .Where(c => c.EmployeeId == empId && !c.IsDeleted)
                .ToListAsync();

            var dataList = _iMapper.Map<List<EmpPostingVm>>(data);

            foreach (var item in dataList)
            {
                var filterData = data.Where(c => c.Id == item.Id).FirstOrDefault();

                item.DesignationName = filterData?.Designation?.Name;
                item.DepartmentName = filterData?.Department?.Name;
                item.PreDepartmentName = filterData?.PreDepartment?.Name;
                item.PreDesignationName = filterData?.PreDesignation?.Name;
            }

            return dataList;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
