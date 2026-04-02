using System.Collections.Generic;

namespace Domain.Utility.Common;

public class DataTablePagination<TSearchVm, TSearchResult> where TSearchVm : class where TSearchResult : class
{
    public DataTablePagination()
    {
        Search = new DataTableSearch();
        Column = new DataTableColumn();
    }
    public DataTablePagination(int take) : this()
    {
        Length = take;
    }

    //For Search
    public int? LineDraw { get; set; }
    public int Start { get; set; } = 0;
    public int Length { get; set; } = 25;
    public string Order { get; set; }
    public DataTableSearch Search { get; set; }
    public DataTableColumn Column { get; set; }
    public TSearchVm SearchModel { get; set; }



    //----------ForResult, must be camel case
    public int draw { get; set; }
    public int recordsFiltered { get; set; }
    public int recordsTotal { get; set; }
    public List<TSearchResult> data { get; set; } = new List<TSearchResult>();

    public class DataTableSearch
    {
        public string Value { get; set; }
        public string Regex { get; set; }
    }

    public class DataTableColumn
    {
        public int Order { get; set; }
        public string Regex { get; set; }
    }
}
