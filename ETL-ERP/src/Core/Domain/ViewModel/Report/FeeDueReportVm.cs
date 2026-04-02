using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Report
{
    public class FeeDueReportVm
    {
        public long FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public long BatchId { get; set; }
        public string BatchName { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public double FeeAmount { get; set; }
        public double PaidAmount { get; set; }
        public double DueAmount { get; set; }
        public IEnumerable<SelectListItem> FeeTypeLookup { get; set; }
        public IEnumerable<SelectListItem> CourseLookup { get; set; }
        public IEnumerable<SelectListItem> BatchLookup { get; set; }
        public IEnumerable<SelectListItem> StudentLookup { get; set; }
    }
}
