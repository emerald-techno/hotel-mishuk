using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.ChartOfAcc
{
    public class ChartOfAccVm
    {
        public string FincYearName { get; set; }
        public ICollection<ChartAccGroup> ChartAccGroups { get; set; }
    }

    public class ChartAccGroup
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string GroupCode { get; set; }
        public string GroupShortCode { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }
        public ICollection<ChartAccHead> ChartAccHeads { get; set; }
    }

    public class ChartAccHead
    {
        public long Id { get; set; }
        public long? ParentHeadId { get; set; }
        public int LevelId { get; set; }
        public string HeadName { get; set; }
        public string HeadCode { get; set; }
        public string Description { get; set; }
        public bool BudgetHead { get; set; }
        public bool AssetHead { get; set; }
        public bool PettyHead { get; set; }
        public string Remarks { get; set; }
        public long GroupId { get; set; }
        public ICollection<ChartAccHead> ChartAccSubHeads { get; set; }
        public ICollection<ChartAccLedger> ChartAccLedgers { get; set; }
    }

    public class ChartAccLedger
    {
        public long Id { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public long HeadId { get; set; }
        public long? StudentId { get; set; }
    }
}
