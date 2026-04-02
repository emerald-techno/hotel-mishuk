using ClosedXML.Excel;
using Domain.ConfigurationModel;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;

namespace Domain.Utility
{
    public static class Utility
    {
        private static AppSettings _appSettings;

        public static AppSettings AppSettings
        {
            get => _appSettings ?? GetAppSettingsData();
            set => _appSettings = value;
        }

        #region GetAppSettingsData

        private static AppSettings GetAppSettingsData()
        {
            _appSettings = new AppSettings();
            var configurationSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var appSettings = configurationSettings.GetSection("AppSettings");
            var smtpConfig = configurationSettings.GetSection("SmtpConfig");
            var conString = configurationSettings.GetSection("ConnectionString");
            var voucherConfig = configurationSettings.GetSection("AppAutoVoucherConfig");


            _appSettings.UploadingFolderPath = appSettings["UploadingFolderPath"];
            _appSettings.UploadingFileFolderName = appSettings["UploadingFileFolderName"];
            _appSettings.IsFileUploadToRootDir = (bool)appSettings["IsFileUploadToRootDir"]?.ToLower()?.Equals("true");
            _appSettings.ApplicationUrl = appSettings["ApplicationUrl"]?.TrimEnd('/');

            if (AppSettings?.SmtpConfig != null)
            {
                _appSettings.SmtpConfig.SmtpEmailSenderName = smtpConfig["SmtpEmailSenderName"];
                _appSettings.SmtpConfig.SmtpEmailAddress = smtpConfig["SmtpEmailAddress"];
                _appSettings.SmtpConfig.SmtpEmailPassword = smtpConfig["SmtpEmailPassword"];
                _appSettings.SmtpConfig.SmtpEmailHost = smtpConfig["SmtpEmailHost"];
                _appSettings.SmtpConfig.SmtpEmailPort = smtpConfig["SmtpEmailPort"];
                _appSettings.SmtpConfig.SmtpUseSSL = (bool)smtpConfig["SmtpUseSSL"]?.ToLower()?.Equals("true");
            }

            if (_appSettings?.ConnectionStrings != null)
            {
                _appSettings.ConnectionStrings.DefaultConnection = conString["DefaultConnection"];
            }

            if (_appSettings?.AppAutoVoucherConfig != null)
            {
                _appSettings.AppAutoVoucherConfig.IsAutoVoucherFoodBill = (bool)voucherConfig["IsAutoVoucherFoodBill"]?.ToLower()?.Equals("true");
                _appSettings.AppAutoVoucherConfig.IsAutoVoucherLoan = (bool)voucherConfig["IsAutoVoucherLoan"]?.ToLower()?.Equals("true");
                _appSettings.AppAutoVoucherConfig.IsAutoVoucherSalaryPay = (bool)voucherConfig["IsAutoVoucherSalaryPay"]?.ToLower()?.Equals("true");
                _appSettings.AppAutoVoucherConfig.IsAutoVoucherAttDeduct = (bool)voucherConfig["IsAutoVoucherAttDeduct"]?.ToLower()?.Equals("true");
            }

            return _appSettings;
        }
        #endregion

        #region File

        public static bool IsFileUploadToRootDir = AppSettings?.IsFileUploadToRootDir ?? false;
        public static string UploadingFileFolderName = AppSettings?.UploadingFileFolderName;
        public static string UploadingFolderPath = AppSettings?.UploadingFolderPath;
        public static string ProjectPhysicalPath = "";
        public static string ServerPath = "";

        public static bool IsNullOrEmpty(this List<IFormFile> file)
        {
            return file == null || file.Count <= 0 || file[0] == null;
        }

        public static bool IsNullOrEmpty(this IFormFile file)
        {
            return file == null || file.Length <= 0;
        }


        public static bool IsImageOrPdf(this List<AppFile> files)
        {
            if (files == null || files.Count <= 0) return false;
            return files.Select(GetFileExtension).Select(c => !c.IsPdf() && !c.IsImage()).FirstOrDefault();
        }

        public static bool IsImageOrPdf(this AppFile file)
        {
            if (file == null) return false;
            var extension = GetFileExtension(file);
            return extension.IsPdf() || extension.IsImage();
        }

        public static bool IsImageOrWord(this List<AppFile> files)
        {
            if (files == null || files.Count <= 0) return false;
            return files.Select(GetFileExtension).Select(c => !c.IsDocOrDocx() && !c.IsImage()).FirstOrDefault();
        }

        public static bool IsImageOrWord(this AppFile file)
        {
            if (file == null) return false;
            var extension = GetFileExtension(file);
            return extension.IsDocOrDocx() || extension.IsImage();
        }


        public static bool IsImageOrPdfOrWord(this List<AppFile> files)
        {
            if (files == null || files.Count <= 0) return false;
            return files.Select(GetFileExtension).Select(c => !c.IsDocOrDocx() && !c.IsDocOrDocx() && !c.IsImage()).FirstOrDefault();
        }

        public static bool IsImageOrPdfOrWord(this AppFile file)
        {
            if (file == null) return false;
            var extension = GetFileExtension(file);
            return extension.IsDocOrDocx() || extension.IsDocOrDocx() || extension.IsImage();
        }









        //public static byte[] ConvertImageToBinary(FormFile uploadFile)
        //{
        //    if (uploadFile == null) return null;
        //    var convertedImage = new byte[uploadFile.ContentLength];
        //    uploadFile.InputStream.Read(convertedImage, 0, uploadFile.ContentLength);
        //    return convertedImage;

        //}

        public static bool IsOfficialFile(List<IFormFile> files)
        {
            var isValid = false;
            if (files == null || files.Count <= 0) return false;
            foreach (var file in files)
            {
                isValid = IsOfficialFile(file);
                if (!isValid)
                {
                    return false;
                }
            }

            return isValid;
        }

        public static bool IsOfficialFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return false;
            }

            if (file.Length > 100000000)
            {
                throw new Exception("File Size Must Be Less Then 100 MB");
            }

            var fType = file.ContentType;
            if (GetFileTypeId(fType) > 0 is false)
            {
                throw new Exception(
                    "File Type Allow Only .jpg, .jpeg, .gif, .png, .txt, .pdf, .xls, .xlsx, .doc, .docx and .xd");
            }

            return true;
        }



        public static List<AppFile> ConvertFormFileToAppFile(AppFileUploadHelper fileUploadHelper)
        {
            if (fileUploadHelper == null) return null;
            var model = new List<AppFile>();
            if (!fileUploadHelper.FormFileList.IsNullOrEmpty())
            {
                foreach (var (v, i) in fileUploadHelper.FormFileList.GetItemWithIndex())
                {
                    if (v == null || v.Length <= 0) continue;
                    var singleFileUploadModel = new AppFileUploadHelper()
                    {
                        FormFile = v,
                        FolderPath = fileUploadHelper.FolderPath,
                        FormFileList = fileUploadHelper.FormFileList,
                        FileCount = i,
                        IsFileUploadToRootDirectory = fileUploadHelper.IsFileUploadToRootDirectory,
                        IsOfficialFile = fileUploadHelper.IsOfficialFile,
                        OperationId = fileUploadHelper.OperationId,
                        OperationTypeId = fileUploadHelper.OperationTypeId,
                    };

                    model.Add(ConvertSingleFormFileToSingleAppFile(singleFileUploadModel));
                }
                if (model.Count > 0) return model;
            }
            else if (!fileUploadHelper.FormFile.IsNullOrEmpty())
            {
                model.Add(ConvertSingleFormFileToSingleAppFile(fileUploadHelper));
                return model;
            }

            //toDo:SRABAN vaiia Changed  for Library SRM Dto
            return new List<AppFile>();
        }


        //Don't Change this to public
        private static AppFile ConvertSingleFormFileToSingleAppFile(AppFileUploadHelper fileUploadHelper)
        {
            if (fileUploadHelper == null || fileUploadHelper.FormFile == null || fileUploadHelper.FormFile.Length <= 0) return null;

            if (fileUploadHelper.IsOfficialFile)
            {
                IsOfficialFile(fileUploadHelper.FormFile);
            }


            var mapPath = "";
            if (IsFileUploadToRootDir)
            {
                mapPath = ServerPath;
                //mapPath = HttpContext.Current.Server.MapPath("~/App_Data/" + UploadingFileFolderName + fileUploadHelper.FolderPath);
            }
            else
            {
                mapPath = UploadingFolderPath + "\\" + UploadingFileFolderName + "\\" + fileUploadHelper.FolderPath;
            }

            var exists = Directory.Exists(mapPath);
            if (!exists)
            {
                Directory.CreateDirectory(mapPath);
            }


            var fnWoEx = Path.GetFileNameWithoutExtension(fileUploadHelper.FormFile.FileName);
            var fEx = Path.GetExtension(fileUploadHelper.FormFile.FileName);

            var rename = fnWoEx?.Length > 3 ? fnWoEx.Substring(0, 3) : fnWoEx?.Length > 2 ? fnWoEx.Substring(0, 2) : fnWoEx?.Length > 0 ? fnWoEx.Substring(0, 1) : "S" + DateTime.Now.Millisecond + "S";
            var appFileModel = new AppFile
            {
                FormFile = fileUploadHelper.FormFile,
                FileName = $"{fileUploadHelper.FileNamePrefix}_{rename}_{DateTime.Now:dMyhmss}{DateTime.Now.Millisecond}{fileUploadHelper.FileCount}{fEx}",
            };


            appFileModel.FileUrl = Path.Combine(mapPath, appFileModel.FileName.ToSafeFileName());
            appFileModel.FileTypeId = GetFileTypeId(fileUploadHelper.FormFile.ContentType);
            appFileModel.OperationId = fileUploadHelper.OperationId ?? 0;
            appFileModel.OperationTypeId = fileUploadHelper.OperationTypeId ?? 0;
            //appFileModel.FileUrl = GetFileSmallPath(appFileModel.FileUrl);
            return appFileModel;


        }




        public static List<AppFile> ConvertFormFileToAppFile(List<AppFileUploadHelper> filesUpload)
        {
            if (filesUpload == null) return null;
            var model = new List<AppFile>();
            if (filesUpload.Count <= 0) return null;
            foreach (var files in filesUpload.Select(ConvertFormFileToAppFile))
            {
                if (files != null) model.AddRange(files);
            }

            return model;
        }


        public static int GetFileTypeId(string contentTypeName)
        {
            var fileTypeId = 0;
            if (!string.IsNullOrEmpty(contentTypeName))
            {
                if (contentTypeName == AppFileTypeEnum.DocOrDocx.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.DocOrDocx;
                }
                else if (contentTypeName == AppFileTypeEnum.GiF.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.GiF;
                }
                else if (contentTypeName == AppFileTypeEnum.Jpeg.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.Jpeg;
                }
                else if (contentTypeName == AppFileTypeEnum.Jpg.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.Jpg;
                }
                else if (contentTypeName == AppFileTypeEnum.MsExcel.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.MsExcel;
                }
                else if (contentTypeName == AppFileTypeEnum.MsWord.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.MsWord;
                }
                else if (contentTypeName == AppFileTypeEnum.PdF.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.PdF;
                }
                else if (contentTypeName == AppFileTypeEnum.PnG.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.PnG;
                }
                else if (contentTypeName == AppFileTypeEnum.Text.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.Text;
                }
                else if (contentTypeName == AppFileTypeEnum.XlsOrXlsx.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.XlsOrXlsx;
                }
                else if (contentTypeName == AppFileTypeEnum.MsPowerPoint.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.MsPowerPoint;
                }
                else if (contentTypeName == AppFileTypeEnum.PptOrPpTx.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.PptOrPpTx;
                }
                else if (contentTypeName == AppFileTypeEnum.Xd.GetDescription())
                {
                    fileTypeId = (int)AppFileTypeEnum.Xd;
                }
            }

            if (!(fileTypeId > 0))
            {
                throw new Exception("Sorry! File Is Not Valid Format");
            }

            return fileTypeId;
        }

        //public static byte[] ConvertImageToBytes(FormFile file)
        //{
        //    if (file == null) return null;
        //    byte[] imageBytes = null;
        //    var reader = new BinaryReader(file.InputStream);
        //    imageBytes = reader.ReadBytes(file.ContentLength);
        //    return imageBytes;

        //}


        public static string RemoveLastCommaFromString(string result)
        {
            result = result.TrimEnd(',');
            return result;
        }


        public static string ConvertByteToBase64String(byte[] file)
        {
            if (file.Length <= 0) return null;
            var base64 = Convert.ToBase64String(file);
            var result = $"data:image/gif;base64,{base64}";
            return result;

        }



        //public static Image GetImageFromPath(string path)
        //{
        //    if (HttpContext.Current == null)
        //    {
        //        throw new ApplicationException("Can't use HttpContext.Current in non-ASP.NET context");
        //    }
        //    return Image.FromFile(HttpContext.Current.Server.MapPath(path) ?? throw new Exception("Not Found"));
        //}

        public static string GetBase64ImageStringFromPath(string path, bool isPhysicalPath = true)
        {
            if (string.IsNullOrEmpty(path)) return null;
            try
            {
                var combinePath = ProjectPhysicalPath + path;
                var file = File.OpenRead(combinePath);
                var imageArray = File.ReadAllBytes(combinePath ?? throw new Exception("Sorry! Content Not Found !"));

                //var imageArray = !isPhysicalPath ? File.ReadAllBytes(Path.Combine(ProjectPhysicalPath, $"{path}") ?? throw new Exception("Sorry! Content Not Found !")) : File.ReadAllBytes(path ?? throw new Exception("Sorry! Content Not Found !"));
                var base64Image = Convert.ToBase64String(imageArray);
                var imageString = $"data:jpg/jpeg/image/gif/png/bmp;base64,{base64Image}";
                return imageString;
            }
            catch
            {
                return null;
            }

        }

        public static string[] GetFileFromFolder(string fileUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    return Directory.GetFiles(fileUrl);
                }
            }
            catch
            {
                throw new Exception("Sorry! File Not Found");
            }

            return null;
        }

        public static bool IsImage(this string value)
        {
            value = value.ToLower();
            var result = value.EndsWith(".png") || value.EndsWith(".jpg") || value.EndsWith(".jpeg") || value.EndsWith(".gif") || value.EndsWith(".bmp") || value.EndsWith("/png") || value.EndsWith("/jpg") || value.EndsWith("/jpeg") || value.EndsWith("/gif") || value.EndsWith("/bmp");
            return result;
        }



        public static bool IsPdf(this string value)
        {
            return value.ToLower().EndsWith(".pdf");
        }

        public static bool IsDocOrDocx(this string value)
        {
            return value.ToLower().EndsWith(".doc") || value.EndsWith(".docx");
        }

        public static bool IsExcel(this string value)
        {
            return value.ToLower().EndsWith(".xls") || value.EndsWith(".xlsx");
        }

        public static bool IsCsV(this string value)
        {
            return value.ToLower().EndsWith(".csv");
        }

        public static bool IsPpT(this string value)
        {
            return value.ToLower().EndsWith(".ppt") || value.EndsWith(".pptx");
        }

        public static string GetFileExtension(AppFile file)
        {
            if (file == null) return "N/ A";
            var extension = file.FileName.Split('.')?.LastOrDefault() ?? "N/ A";
            return extension;
        }

        public static string GetFileExtension(string fileName)
        {
            if (fileName == null) return "N/ A";
            var extension = fileName.Split('.')?.LastOrDefault() ?? "N/ A";
            return extension;
        }


        public static string GetFileSmallPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var folderFullPath = UploadingFolderPath;
            CreateDirectory(folderFullPath);
            if (folderFullPath.Length < path.Length && path.Contains(folderFullPath))
            {
                path = path.Substring(folderFullPath.Length, path.Length - folderFullPath.Length);
            }

            return path;
        }


        public static void CreateDirectory(string mapPath)
        {
            var exists = Directory.Exists(mapPath);
            if (exists) return;
            try
            {
                Directory.CreateDirectory(mapPath);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Could not find a part of the path"))
                {
                    throw new Exception($"Sorry! System Couldn't Found This [{mapPath[0]}] Drive, Please Check Again or Contact with Vendor");
                }
            }
        }

        public static async Task<bool> UploadFileToFolderAsync(List<AppFile> files)
        {
            var isUploaded = false;
            if (files == null || files.Count <= 0) return false;
            foreach (var file in files)
            {
                isUploaded = await UploadFileToFolderAsync(file);
            }

            return isUploaded;
        }

        public static bool UploadFileToFolder(List<AppFile> files)
        {
            var isUploaded = false;
            if (files == null || files.Count <= 0) return false;
            foreach (var file in files)
            {
                isUploaded = UploadFileToFolder(file);
            }

            return isUploaded;
        }

        public static async Task<bool> UploadFileToFolderAsync(AppFile file)
        {
            if (file?.FormFile == null || file.FormFile.Length <= 0) return false;

            await using var stream = new FileStream(file.FileUrl, FileMode.Create);
            await file.FormFile.CopyToAsync(stream);
            return true;
        }

        public static bool UploadFileToFolder(AppFile file)
        {
            if (file?.FormFile == null || file.FormFile.Length <= 0) return false;
            using var stream = new FileStream(file.FileUrl, FileMode.Create);
            file.FormFile.CopyTo(stream);
            return true;
        }

        public static bool DeleteFileFromFolder(List<AppFile> files)
        {
            if (files == null || files.Count <= 0) return false;
            foreach (var fileBase in files)
            {
                DeleteFileFromFolder(fileBase.FileUrl);
            }

            return false;
        }


        public static bool DeleteFileFromFolder(AppFile file)
        {
            return DeleteFileFromFolder(file?.FileUrl);
        }

        public static bool DeleteFileFromFolder(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            if (!File.Exists(url)) return false;
            File.Delete(url);
            return true;

        }

        public static void SetFileAccess264CharLong()
        {
            var parentKey = Registry.LocalMachine;
            var sk = parentKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\FileSystem", true);
            sk?.SetValue("LongPathsEnabled", 1);
        }


        public static string ToSafeFileName(this string value)
        {
            return value.Replace("\\", "").Replace("/", "").Replace("\"", "").Replace("*", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
        }







        #endregion

        #region Enum


        public static string GetEnumNameByValue<T>(int value, bool isDescription = true) where T : Enum
        {
            //var name = Enum.GetName(typeof(T), value);
            if (!typeof(T).IsEnum) throw new Exception("Sorry! Given Type is Not Enum");
            var data = Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(c => Convert.ToInt32(c) == value);
            var result = isDescription ? data.GetDescription() : data?.ToString();
            result = string.IsNullOrEmpty(result) ? "--" : result;
            return result;

        }


        public static string GetDescription(this Enum enumName)
        {
            var genericEnumType = enumName.GetType();
            var data = genericEnumType.GetMember(enumName.ToString());
            if (!(data?.Length > 0)) return enumName.ToString();
            var attributes = data[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes?.ToList().Count > 0 ? ((DescriptionAttribute)attributes.ElementAt(0)).Description : enumName.ToString();
        }



        public static long ToInt64(this Enum enumName)
        {
            try { return Convert.ToInt64(enumName); } catch { return 0; }
        }



        public static short ToInt16(this Enum enumName)
        {
            try { return Convert.ToInt16(enumName); } catch { return 0; }
        }

        public static string ToCharToString(this Enum enumName)
        {
            try { return enumName.GetDescription()[0].ToString(); } catch { return ""; }
        }

        public static string ToInt32ToString(this Enum enumName)
        {
            try { return Convert.ToInt32(enumName).ToString(); } catch { return ""; }
        }


        #endregion

        #region Extension Methods

        public static IEnumerable<(T item, int index)> GetItemWithIndex<T>(this IEnumerable<T> self) => self?.Select((item, index) => (item, index)) ?? new List<(T, int)>();

        public static int ForEach<T>(this IEnumerable<T> list, Action<T, int> action) { if (action == null) throw new ArgumentNullException(nameof(action)); var i = 0; foreach (var elem in list) action(elem, i++); return i; }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action) { if (action == null) throw new ArgumentNullException(nameof(action)); foreach (var elem in list) action(elem); }

        //public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    var seenKeys = new HashSet<TKey>();
        //    foreach (var element in source) { if (seenKeys.Add(keySelector(element))) { yield return element; } }
        //}

        #endregion

        #region Convert Excel To DataTable

        public static DataTable ConvertCsVtoDataTable(IFormFile file)
        {
            if (file == null || file.Length <= 0 || !file.FileName.IsCsV()) return null;
            var dt = new DataTable();
            var sr = new StreamReader(file.OpenReadStream());
            var headers = sr.ReadLine()?.Split(',');
            if (headers == null) return dt;
            headers.ForEach(c => dt.Columns.Add(c.ToString().Trim()));
            while (!sr.EndOfStream)
            {
                var rows = sr.ReadLine()?.Split(',').ToList();
                if (!(rows?.Count > 1)) continue;
                var dr = dt.NewRow();

                rows.ForEach((c, i) => dr[i] = c.Trim());
                dt.Rows.Add(dr);
            }

            return dt;
        }


        public static DataTable ConvertExcelToDataTable(IFormFile file)
        {
            if (file == null || file.Length <= 0 || !file.FileName.IsExcel()) return null;
            var workBook = new XLWorkbook(file.OpenReadStream());
            var workSheet = workBook.Worksheet(1);
            var dt = new DataTable();
            var firstRow = true;

            foreach (var row in workSheet.Rows())
            {
                if (firstRow)
                {
                    if (row.Cells().ToList().Count > 0) row.Cells().ForEach(c => dt.Columns.Add(c.Value?.ToString()?.Trim()));
                    firstRow = false;
                    continue;
                }
                dt.Rows.Add();
                if (row.Cells().ToList().Count > 0) row.Cells().ForEach((c, i) => dt.Rows[dt.Rows.Count - 1][i] = c.Value?.ToString());
            }

            return dt;
        }

        public static DataTable ConvertExcelOrCsVToDataTable(IFormFile file)
        {
            if (file == null || file.Length <= 0 || !(file.FileName.IsCsV() || file.FileName.IsExcel())) return null;
            return file.FileName.IsCsV() ? ConvertCsVtoDataTable(file) : file.FileName.IsExcel() ? ConvertExcelToDataTable(file) : null;
        }

        #endregion

        #region DateTime

        public static DateTime GetBdDateTimeNow()
        {
            var utcTime = DateTime.UtcNow;
            var bdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, bdZone);
            return localDateTime;
        }

        public static DateTime GetBdDateTimeNow(DateTime dateTime)
        {
            var bdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            var utc = TimeZoneInfo.ConvertTimeToUtc(dateTime, bdZone);
            var bdTime = TimeZoneInfo.ConvertTimeFromUtc(utc, bdZone);
            return bdTime;
        }

        public static DateTime GetDate(string date)
        {
            if (string.IsNullOrEmpty(date)) return DateTime.Now;
            IFormatProvider culture = new CultureInfo("bn-BD", true);
            var convertedDate = DateTime.Parse(date, culture, DateTimeStyles.AssumeLocal);
            return convertedDate;
        }


        public static DateTime? ConvertStrToDate(string date)
        {
            if (string.IsNullOrEmpty(date)) return null;
            return GetDate(date);
        }



        public static string GetDate(DateTime date)
        {
            var data = date.ToString("dd/MM/yyyy");
            return data;
        }

        public static string GetTime(DateTime date)
        {
            var data = date.ToString("HH:mm:tt");
            return data;
        }

        public static string ConvertDateToStr(DateTime? date)
        {
            return date == null ? string.Empty : GetDate((DateTime)date);
        }

        public static string ConvertStrDateToStrTime(string date)
        {
            if (string.IsNullOrEmpty(date)) return "N/A";
            var data = GetDate(date);
            var time = GetTime(data);
            return time;
        }

        public static string ConvertStrDateToStrDateOnly(string date)
        {
            if (string.IsNullOrEmpty(date)) return "N/A";
            var data = GetDate(date);
            var time = GetDate(data);
            return time;
        }

        public static string ConvertTimeToStr(DateTime? time)
        {
            return time == null ? string.Empty : GetTime((DateTime)time);
        }


        public static TimeSpan GetStrTime(string time)
        {
            if (string.IsNullOrEmpty(time))
            {
                return new TimeSpan();
            }

            var res = DateTime.TryParse(time, out var dateTime);
            if (!res)
            {
                throw new Exception("Sorry! Time is Not Correct Format !");
            }

            var convertedTime = dateTime.TimeOfDay;
            return convertedTime;

        }



        public static string GetStrTime(this TimeSpan time)
        {

            var dateTime = DateTime.Today.Add(time);
            var displayTime = dateTime.ToString("hh:mm tt");
            return displayTime;
        }




        /// <summary>
        /// Convert DateTime to String DateTime
        /// </summary>
        /// <param name="date">C# Date</param>
        /// <param name="format">1: dd-MMM-yyyy hh:mm tt, 2: dd/MM/yyyy hh:mm tt, Default: 1</param>
        /// <param name="is24Time">Is 24 Hour Format? Default: false</param>
        /// <returns></returns>
        public static string ConvertDateTimeToStr(this DateTime date, int? format = null, bool is24Time = false)
        {
            format ??= 1;
            if (format == 1 && is24Time) return date.ToString("dd-MMM-yyyy HH:MM tt");
            if (format == 1 && !is24Time) return date.ToString("dd-MMM-yyyy hh:mm");

            if (format == 2 && is24Time) return date.ToString("dd/MM/yyyy HH:MM tt");
            if (format == 2 && !is24Time) return date.ToString("dd/MM/yyyy hh:mm");
            return "";
        }

        /// <summary>
        /// Convert DateTime to String DateTime
        /// </summary>
        /// <param name="date">C# Date</param>
        /// <param name="format">1: dd-MMM-yyyy hh:mm tt, 2: dd/MM/yyyy hh:mm tt, Default: 1</param>
        /// <returns></returns>
        public static string ConvertDateToStr(this DateTime date, int? format = null)
        {
            format ??= 1;
            if (format == 1) return date.ToString("dd-MMM-yyyy");
            if (format == 2) return date.ToString("dd/MM/yyyy");
            return "";
        }



        #endregion

        #region AutoCode

        public static string GetAutoGenNo(string prefix, int? howManyDigit = 10)
        {
            while (true)
            {
                var date = DateTime.Now;
                var result = $"{prefix}-{date.Year}{date.Month}{date.Day}{date.Hour}{date.Minute}{date.Second}{date.Millisecond:D}";
                result = result.Substring(0, howManyDigit ?? 10);
                return result;
            }
        }

        public static string GetAutoCodeWithAlphaNum(int? digit = 6)
        {
            if (digit == null) digit = 6;
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, (int)digit).Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }

        #endregion

        #region List-StringConversion
        public static string ConvertToCommaSeparatedString(List<long> list)
        {
            StringBuilder result = new StringBuilder();
            foreach (var val in list)
            {
                var lastIndex = list.Last();

                if (lastIndex == val)
                {
                    result.Append(val);
                }
                else
                {
                    result.Append(val + ", ");
                }
            }
            //for (int i = 0; i < list.Count; i++)
            //{
            //    result.Append(list[i]+", ");
            //}
            return result.ToString();
        }

        public static List<long> ConvertStringToList(string ids)
        {
            var result = ids.Split(",");

            return result.Select(long.Parse).ToList();
        }

        #endregion

        public static string GetMonthName(int monthIndex)
        {
            if (monthIndex < 1 || monthIndex > 12) return string.Empty;
            var month = DateTimeFormatInfo.CurrentInfo.GetMonthName(monthIndex);
            return month;
        }

        public static double PercentCalculation(double percentValue, double totalNumber)
        {
            if (totalNumber <= 0) return 0;
            var result = Math.Ceiling((percentValue * totalNumber) / 100);
            return result;
        }

        public static string ConvertToWordInt(double dblNumber, string taka = "", string paisa = "")//international
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 1000000);
            dblNumber = dblNumber % 1000000;

            if (dblResult > 0)
            {
                if (dblResult.ToString().Length > 2)
                    strInWord = strInWord + ConvertToWord(dblResult) + " Million";
                else
                    strInWord = strInWord + ConverTwoDigit(dblResult) + " Million ";
                //strInWord = strInWord + ConverTwoDigit(dblResult) + " Million ";
            }

            //dblResult = Math.Floor(dblNumber / 100000);
            //dblNumber = dblNumber % 100000;
            //if (dblResult > 0)
            //{
            //    strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            //}

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                if (dblResult.ToString().Length > 2)
                    strInWord = strInWord + ConvertToWord(dblResult) + " Thousand";
                else
                    strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                strInWord = strInWord + " and " + paisa + ConverTwoDigit(dblPrecision % 100);
            }

            tempConvertToWord = " " + taka + " " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);// + " Only";
            if (taka != "") { tempConvertToWord += " Only"; }

            return tempConvertToWord;
        }

        public static string ConvertToWord(double dblNumber, string taka = "", string paisa = "")
        {
            string tempConvertToWord = null;
            string strInWord = null;
            double dblResult = 0;
            double dblPrecision = 0;
            int intPosition = 0;
            string strNumber = Convert.ToString(dblNumber);
            strNumber = String.Format("{0:F2}", Convert.ToDouble(strNumber));

            intPosition = Convert.ToInt32(strNumber.Substring(strNumber.IndexOf(".") + 1, 2));
            dblPrecision = Convert.ToDouble(intPosition);

            //dblPrecision = (dblNumber - (int)Math.Floor(dblNumber)) * 100;
            //dblNumber = (dblNumber - dblPrecision / 100.0);
            strInWord = " ";

            if (dblNumber > 999999999.99)
            {
                return "It is too big to convert in word by this class. Try with smaller one. ";
            }

            if (dblNumber == 0)
            {
                return "Zero Only.";
            }

            if (dblNumber < 0)
            {
                return "Negative number cannot be converted.";
            }

            dblResult = Math.Floor(dblNumber / 10000000);
            dblNumber = dblNumber % 10000000;

            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Crore ";
            }

            dblResult = Math.Floor(dblNumber / 100000);
            dblNumber = dblNumber % 100000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Lac ";
            }

            dblResult = Math.Floor(dblNumber / 1000);
            dblNumber = dblNumber % 1000;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Thousand ";
            }

            dblResult = Math.Floor(dblNumber / 100);
            dblNumber = dblNumber % 100;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult) + " Hundred ";
            }

            dblResult = Math.Floor(dblNumber / 1);
            dblNumber = dblNumber % 1;
            if (dblResult > 0)
            {
                strInWord = strInWord + ConverTwoDigit(dblResult);
            }

            if (dblPrecision > 0)
            {
                strInWord = strInWord + " and " + paisa + ConverTwoDigit(dblPrecision % 100);
            }

            tempConvertToWord = " " + taka + " " + strInWord.Substring(2, 1).ToUpper() + strInWord.Substring(3);// + " Only";
            if (taka != "") { tempConvertToWord += " Only"; }

            return tempConvertToWord;
        }
        private static int FiveToRound(double dblDigit)
        {
            int intFloor = 0;
            double dblDigit2 = 0;

            intFloor = Convert.ToInt32(Math.Floor(dblDigit));
            dblDigit2 = Convert.ToDouble(intFloor);

            if (dblDigit - dblDigit2 > 0.5)
            {
                return intFloor + 1;
            }
            else
            {
                return intFloor;
            }
        }
        private static string ConverTwoDigit(double dblTwoDigit)
        {
            double dblFirstDigit = 0;
            double intSecondDigit = 0;
            string[] strArrayFirst = new string[10];
            string[] strArraySecond = new string[10];
            string[] strArrayThird = new string[10];

            strArrayFirst[1] = " One";
            strArrayFirst[2] = " Two";
            strArrayFirst[3] = " Three";
            strArrayFirst[4] = " Four";
            strArrayFirst[5] = " Five";
            strArrayFirst[6] = " Six";
            strArrayFirst[7] = " Seven";
            strArrayFirst[8] = " Eight";
            strArrayFirst[9] = " Nine";

            strArraySecond[1] = " Ten";
            strArraySecond[2] = " Twenty";
            strArraySecond[3] = " Thirty";
            strArraySecond[4] = " Forty";
            strArraySecond[5] = " Fifty";
            strArraySecond[6] = " Sixty";
            strArraySecond[7] = " Seventy";
            strArraySecond[8] = " Eighty";
            strArraySecond[9] = " Ninety";

            strArrayThird[1] = " Eleven";
            strArrayThird[2] = " Twelve";
            strArrayThird[3] = " Thirteen";
            strArrayThird[4] = " Fourteen";
            strArrayThird[5] = " Fifteen";
            strArrayThird[6] = " Sixteen";
            strArrayThird[7] = " Seventeen";
            strArrayThird[8] = " Eighteen";
            strArrayThird[9] = " Nineteen";

            dblFirstDigit = Math.Floor(dblTwoDigit / 10);
            //intSecondDigit = Math.Floor(dblTwoDigit % 10);
            intSecondDigit = FiveToRound(dblTwoDigit % 10);

            if (dblFirstDigit > 0 && intSecondDigit == 0)
            {
                return strArraySecond[Convert.ToInt32(dblFirstDigit)];
            }

            if (dblFirstDigit == 1 && intSecondDigit > 0)
            {
                return strArrayThird[Convert.ToInt32(intSecondDigit)];
            }

            if (dblFirstDigit == 0 && intSecondDigit > 0)
            {
                return strArrayFirst[Convert.ToInt32(intSecondDigit)]; //sad
            }

            if (dblFirstDigit > 0 & intSecondDigit > 0)
            {
                return strArraySecond[Convert.ToInt32(dblFirstDigit)] + strArrayFirst[Convert.ToInt32(intSecondDigit)];
            }

            return " ";
        }

        #region ReportDate

        public static DateTime GenerateReportDate(DateTime date)
        {
            TimeSpan midnight = new TimeSpan(0, 0, 0);
            TimeSpan elevenAM = new TimeSpan(11, 0, 0);

            if (date.TimeOfDay < elevenAM && date.TimeOfDay <= midnight)
            {
                return date.AddDays(-1);
            }
            else
            {
                return date;
            }
        }

        #endregion
    }
}
