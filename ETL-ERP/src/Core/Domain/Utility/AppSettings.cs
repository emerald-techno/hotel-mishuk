namespace Domain.Utility;

public class AppSettings
{
    public SmtpConfig SmtpConfig { get; set; } = new SmtpConfig();
    public ConnectionString ConnectionStrings { get; set; } = new ConnectionString();
    public AppAutoVoucherConfig AppAutoVoucherConfig { get; set; } = new AppAutoVoucherConfig();

    public string UploadingFolderPath { get; set; }
    public string UploadingFileFolderName { get; set; }
    public string ApplicationUrl { get; set; }
    public bool IsFileUploadToRootDir { get; set; }

    public string TokenSecretKey { get; set; }
    public int TokenExpiresHours { get; set; }
}


public class ConnectionString
{
    public string DefaultConnection { get; set; }
}

public class SmtpConfig
{
    public string SmtpEmailSenderName { get; set; }
    public string SmtpEmailAddress { get; set; }
    public string SmtpEmailPassword { get; set; }
    public string SmtpEmailHost { get; set; }
    public string SmtpEmailPort { get; set; }
    public bool SmtpUseSSL { get; set; }
}

public class AppAutoVoucherConfig
{
    public bool IsAutoVoucherFoodBill { get; set; }
    public bool IsAutoVoucherLoan { get; set; }
    public bool IsAutoVoucherSalaryPay { get; set; }
    public bool IsAutoVoucherAttDeduct { get; set; }
}