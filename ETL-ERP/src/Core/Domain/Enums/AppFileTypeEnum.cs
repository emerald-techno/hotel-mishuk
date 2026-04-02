using System.ComponentModel;

namespace Domain.Enums;

public enum AppFileTypeEnum
{
    [Description("image/jpg")]
    Jpg = 1,

    [Description("image/jpeg")]
    Jpeg,

    [Description("image/gif")]
    GiF,

    [Description("image/png")]
    PnG,

    [Description("text/plain")]
    Text,

    [Description("application/pdf")]
    PdF,

    [Description("application/msword")]
    MsWord,

    [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    DocOrDocx,

    [Description("application/vnd.ms-excel")]
    MsExcel,

    [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    XlsOrXlsx,

    [Description("application/vnd.ms-powerpoint")]
    MsPowerPoint,

    [Description("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    PptOrPpTx,

    [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Xd,
}
