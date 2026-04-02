using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.ChartOfAcc
{
    public class AccTreeNode
    {
        public long HeadId { get; set; }
        public string HeadName { get; set; }
        public string HeadCode { get; set; }

        public long ParentHeadId { get; set; }
        public short LevelId { get; set; }
        public short GroupId { get; set; }
        public string GroupCode { get; set; }


    }
}
