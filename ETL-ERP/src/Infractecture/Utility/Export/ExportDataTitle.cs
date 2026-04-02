namespace Utility.Export
{
    public class ExportDataTitle
    {
        public ExportDataTitle()
        {

        }
        public string Header { get; set; } = "";
        public string AddressOne { get; set; } = "";
        public string AddressTwo { get; set; } = "";
        public string ReportTitle { get; set; } = "";
        public List<string> SignatureList { get; set; } = new List<string>();
        public bool IsSignature { get; set; } = false;

        public bool IsSignTwoRow { get; set; } = false;
        public int FirstRowSignCount { get; set; } = 2;

        public string UniqueCode { get; set; } = "";
        public string NoteText { get; set; } = "";
        public bool IsHeaderLogo { get; set; } = false;
        public bool IsFooterChange { get; set; } = false;
    }
}
