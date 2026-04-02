using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccGroup;
using Domain.ViewModel.Accounting.ChartOfAcc;
using Interface.Repository.Accounts;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Accounts
{
    public class AccGroupService : BaseService<AccGroup>, IAccGroupService
    {
        #region Config
        private IAccGroupRepository Repository;
        private readonly IAccHeadRepository _iHeadRepository;
        private readonly IAccLedgerRepository _iLedgerRepository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        public AccGroupService(IAccGroupRepository iRepository, IAccHeadRepository iHeadRepository, IAccLedgerRepository iLedgerRepository, IMapper iMapper, IUnitOfWork iUnitOfWork)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iHeadRepository = iHeadRepository;
            _iLedgerRepository = iLedgerRepository;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<AccGroupSearchVm, AccGroupSearchVm>> SearchAsync(DataTablePagination<AccGroupSearchVm, AccGroupSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region GetChartOfAccount

        public async Task<ChartOfAccVm> GetChartOfAcc()
        {
            var chartOfAccVm = new ChartOfAccVm();

            var groupList = await Repository.GetAsync(c => !c.IsDeleted);
            var headList = await _iHeadRepository.GetAsync(c => !c.IsDeleted);
            var ledgerList = await _iLedgerRepository.GetAsync(c => !c.IsDeleted);

            if (groupList.Count > 0)
            {
                var chartGroupList = _iMapper.Map<List<ChartAccGroup>>(groupList);

                foreach (var group in chartGroupList)
                {
                    var filterHeadList = headList.Where(c => c.GroupId == group.Id && c.ParentHeadId == 0).ToList();

                    if (filterHeadList.Count > 0)
                    {
                        group.ChartAccHeads = _iMapper.Map<List<ChartAccHead>>(filterHeadList);

                        if (group.ChartAccHeads.Count > 0)
                        {
                            foreach (var head in group.ChartAccHeads)
                            {
                                GetHeadOrLedgerInfoByHead(head, headList, ledgerList);
                            }
                        }
                    }
                }
                chartOfAccVm.ChartAccGroups = chartGroupList;
            }
            return chartOfAccVm;
        }

        //public async Task<ChartOfAccVm> GetChartOfAcc()
        //{
        //    var chartOfAccVm = new ChartOfAccVm();

        //    var groupList = await Repository.GetAsync(c=> !c.IsDeleted);
        //    var headList = await _iHeadRepository.GetAsync(c => !c.IsDeleted);
        //    var ledgerList = await _iLedgerRepository.GetAsync(c => !c.IsDeleted);

        //    if (groupList.Count > 0)
        //    {
        //        var chartGroupList = _iMapper.Map<List<ChartAccGroup>>(groupList);

        //        foreach (var group in chartGroupList)
        //        {
        //            var filterHeadList = headList.Where(c => c.GroupId == group.Id).ToList();

        //            if (filterHeadList.Count > 0)
        //            {
        //                group.ChartAccHeads = _iMapper.Map<List<ChartAccHead>>(filterHeadList);

        //                if(group.ChartAccHeads.Count > 0)
        //                {
        //                    foreach (var head in group.ChartAccHeads)
        //                    {
        //                        #region
        //                        //var subHeadList = headList.Where(c => c.ParentHeadId == head.Id).ToList();

        //                        //if (subHeadList.Count > 0)
        //                        //{
        //                        //    head.ChartAccSubHeads = _iMapper.Map<List<ChartAccHead>>(subHeadList);

        //                        //    foreach (var subHead in head.ChartAccSubHeads)
        //                        //    {
        //                        //        var subHeadLedgerList = ledgerList.Where(c => c.HeadId == subHead.Id).ToList();

        //                        //        subHead.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(subHeadLedgerList);
        //                        //    }
        //                        //}
        //                        //else
        //                        //{
        //                        //    var filterLedgerList = ledgerList.Where(c => c.HeadId == head.Id).ToList();
        //                        //    head.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(filterLedgerList);
        //                        //}

        //                        //if (head.ParentHeadId == 0)
        //                        //{
        //                        //    var subHeadList = headList.Where(c => c.ParentHeadId == head.Id).ToList();

        //                        //    head.ChartAccSubHeads = _iMapper.Map<List<ChartAccHead>>(subHeadList);

        //                        //    foreach (var subHead in head.ChartAccSubHeads)
        //                        //    {
        //                        //        var subHeadLedgerList = ledgerList.Where(c => c.HeadId == subHead.Id).ToList();

        //                        //        subHead.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(subHeadLedgerList);
        //                        //    }
        //                        //}
        //                        //else
        //                        //{
        //                        //    var filterLedgerList = ledgerList.Where(c => c.HeadId == head.Id).ToList();
        //                        //    head.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(filterLedgerList);
        //                        //}
        //                        #endregion
        //                        if (head.HeadCode == "2.2.3.1")
        //                        {
        //                            var dataR = head;
        //                        }

        //                        var filterLedgerList = ledgerList.Where(c => c.HeadId == head.Id).ToList();

        //                        if(filterLedgerList.Count > 0)
        //                        {
        //                            head.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(filterLedgerList);
        //                        }
        //                        else
        //                        {
        //                            var subHeadList = headList.Where(c => c.ParentHeadId == head.Id).ToList();

        //                            head.ChartAccSubHeads = _iMapper.Map<List<ChartAccHead>>(subHeadList);

        //                            foreach (var subHead in head.ChartAccSubHeads)
        //                            {
        //                                var subHeadLedgerList = ledgerList.Where(c => c.HeadId == subHead.Id).ToList();

        //                                subHead.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(subHeadLedgerList);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //        }

        //        chartOfAccVm.ChartAccGroups = chartGroupList;
        //    }



        //    return chartOfAccVm;
        //}

        #endregion

        #region GetHeadOrLedgerInfo

        private ChartAccHead GetHeadOrLedgerInfoByHead(ChartAccHead head, ICollection<AccHead> headList, ICollection<AccLedger> ledgerList)
        {
            var filterLedgerList = ledgerList.Where(c => c.HeadId == head.Id).ToList();
            if (filterLedgerList.Count > 0)
            {
                head.ChartAccLedgers = _iMapper.Map<List<ChartAccLedger>>(filterLedgerList);
            }
            else
            {
                var subHeadList = headList.Where(c => c.ParentHeadId == head.Id).ToList();
                if (subHeadList.Count > 0)
                {
                    head.ChartAccSubHeads = _iMapper.Map<List<ChartAccHead>>(subHeadList);
                    foreach (var subHead in head.ChartAccSubHeads)
                    {
                        GetHeadOrLedgerInfoByHead(subHead, headList, ledgerList);
                    }
                }
            }
            return head;
        }

        #endregion
    }
}
