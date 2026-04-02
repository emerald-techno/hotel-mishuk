using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpPosting;
using Interface.Repository.Hr;
using Interface.Services.Admin;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Hr
{
    public class EmpPostingService : BaseService<EmpPosting>, IEmpPostingService
    {
        #region Config

        private IEmpPostingRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        private IDepartmentService _iDepartmentService;
        private IDesignationService _iDesignationService;

        public EmpPostingService(IEmpPostingRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork,
            IDepartmentService iDepartmentService, IDesignationService iDesignationService) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iDepartmentService = iDepartmentService;
            _iDesignationService = iDesignationService;
        }

        #endregion

        #region GetPostingByEmpId

        public async Task<List<EmpPostingVm>> GetPostingByEmpId(long empId)
        {
            var DepartmentList = _iDepartmentService.Get(c => !c.IsDeleted).ToList();
            var DesignationList = _iDesignationService.Get(c => !c.IsDeleted).ToList();
            if (empId <= 0) return new List<EmpPostingVm>();
            var data = await Repository.GetAsync(c => c.EmployeeId == empId && !c.IsDeleted);
            var dataList = _iMapper.Map<List<EmpPostingVm>>(data);

            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    var model = new EmpPostingVm();
                    model.DepartmentName = DepartmentList.FirstOrDefault(c => c.Id == item.DepartmentId).Name;
                    model.PreDepartmentName = DepartmentList.FirstOrDefault(c => c.Id == item.PreDepartmentId).Name;
                    model.DesignationName = DepartmentList.FirstOrDefault(c => c.Id == item.DesignationId).Name;
                    model.PreDepartmentName = DepartmentList.FirstOrDefault(c => c.Id == item.PreDesignationId).Name;
                }
            }

            return dataList;
        }

        #endregion
    }
}
