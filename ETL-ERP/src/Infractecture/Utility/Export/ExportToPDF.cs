using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using DU = Domain.Utility;

namespace Utility.Export
{
    public class ExportToPDF
    {

        IHttpContextAccessor _httpContextAccessor;
        //private readonly IWebHostEnvironment _iWebHostEnvironment;
        public ExportToPDF(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region PdfContentWithoutLogo
        public byte[] ExportContentToPdf(string reportContent, string reportName, int left = 6, int right = 6, int top = 65, int bottom = 15, bool isLandScape = false, ExportDataTitle reportTitle = null, bool withFooter = true, bool isLarge = false, int width = 1070, int height = 816)
        {
            MemoryStream _memoryStream = new MemoryStream();
            try
            {
                var pgSize = new iTextSharp.text.Rectangle(width, height);
                Document doc = new Document(PageSize.A4, left, right, top, bottom);
                if (isLandScape)
                {
                    if (isLarge)
                        doc.SetPageSize(pgSize);
                    else
                        doc.SetPageSize(PageSize.A4.Rotate());
                }

                reportName = reportName + ".pdf";


                string html = reportContent.ToString();
                html = Regex.Replace(html, "</?(a|A).*?>", "");
                html = html.Replace("&", "&amp;");

                PdfWriter writer = PdfWriter.GetInstance(doc, _memoryStream);

                if (withFooter)
                {
                    ITextEvent eventHead = new ITextEvent();
                    eventHead.FormatData = reportTitle;
                    writer.PageEvent = eventHead;
                }
                doc.Open();

                // CSS

                //var cssFilePath = DU.Utility.ServerPath + "\\print.css";

                var cssFilePath = DU.Utility.UploadingFolderPath + "print.css";
                var cssResolver = new StyleAttrCSSResolver();
                var cssFile = XMLWorkerHelper.GetCSS(new FileStream($@"{cssFilePath}", FileMode.Open));
                cssResolver.AddCss(cssFile);

                //string rootPath = Directory.GetCurrentDirectory();
                //string cssFilePath = Path.Combine(rootPath, "wwwroot", "print.css");

                //var cssResolver = new StyleAttrCSSResolver();
                //using (var fs = new FileStream(cssFilePath, FileMode.Open))
                //{
                //    var cssFile = XMLWorkerHelper.GetCSS(fs);
                //    cssResolver.AddCss(cssFile);
                //}

                CssAppliers ca = new CssAppliersImpl();
                HtmlPipelineContext hpc = new HtmlPipelineContext(ca);
                hpc.SetTagFactory(Tags.GetHtmlTagProcessorFactory());


                //// PIPELINES
                PdfWriterPipeline pdf = new PdfWriterPipeline(doc, writer);
                HtmlPipeline htmlPipe = new HtmlPipeline(hpc, pdf);
                CssResolverPipeline css = new CssResolverPipeline(cssResolver, htmlPipe);

                XMLWorker worker = new XMLWorker(css, true);
                XMLParser p = new XMLParser(worker, Encoding.UTF8);
                StringReader sr = new StringReader(html);
                p.Parse(sr);
                doc.Close();

                return _memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                //eallySimpleLog.WriteLog(ex);
            }
            return _memoryStream.ToArray();
        }
        #endregion

        #region PdfContentWithLogo
        public byte[] ExportContentToPdfWithLogo(string reportContent, string reportName, int left = 6, int right = 6, int top = 65, int bottom = 15, bool isLandScape = false, bool isLegalPage = false, ExportDataTitle reportTitle = null, bool withFooter = true, bool isLarge = false, int width = 1070, int height = 816, bool companyImage = true)
        {
            MemoryStream _memoryStream = new MemoryStream();
            try
            {
                //var pgSize = new iTextSharp.text.Rectangle(width, height);
                //Document doc = new Document(PageSize.A4, left, right, top, bottom);
                //if (isLandScape)
                //{
                //    if (isLarge)
                //        doc.SetPageSize(pgSize);
                //    else
                //        doc.SetPageSize(PageSize.A4.Rotate());
                //}

                var basePageSize = isLegalPage ? PageSize.LEGAL : PageSize.A4;
                var pgSize = new iTextSharp.text.Rectangle(width, height);

                Document doc = new Document(basePageSize, left, right, top, bottom);
                if (isLandScape)
                {
                    if (isLarge)
                        doc.SetPageSize(pgSize);
                    else
                        doc.SetPageSize(basePageSize.Rotate());
                }

                reportName = reportName + ".pdf";


                string html = reportContent.ToString();
                html = Regex.Replace(html, "</?(a|A).*?>", "");
                html = html.Replace("&", "&amp;");

                PdfWriter writer = PdfWriter.GetInstance(doc, _memoryStream);

                if (withFooter)
                {
                    ITextEventBill eventHead = new ITextEventBill();
                    eventHead.FormatData = reportTitle;
                    writer.PageEvent = eventHead;
                }
                doc.Open();

                //company image
                //if (companyImage)
                //{
                //    try
                //    {
                //        string imageURL = DU.Utility.ServerPath + "\\misuk_logo_sm.jpg";
                //        iTextSharp.text.Image compImg = iTextSharp.text.Image.GetInstance(imageURL);

                //        //Resize image depend upon your need
                //        compImg.ScaleToFit(80f, 80f);
                //        compImg.SpacingBefore = 10f;
                //        compImg.SpacingAfter = 1f;
                //        compImg.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);
                //        compImg.SetAbsolutePosition(doc.PageSize.Width / 2 - 35, doc.PageSize.Height - 70);
                //        doc.Add(compImg);
                //    }
                //    catch (Exception ex)
                //    {
                //        throw new Exception(ex.Message);
                //    }
                //}

                // CSS
                //var cssFilePath = DU.Utility.ServerPath + "\\print.css";

                var cssFilePath = DU.Utility.UploadingFolderPath + "print.css";
                var cssResolver = new StyleAttrCSSResolver();
                var cssFile = XMLWorkerHelper.GetCSS(new FileStream($@"{cssFilePath}", FileMode.Open));
                cssResolver.AddCss(cssFile);

                // wwwroot print CSS
                //string rootPath = Directory.GetCurrentDirectory();
                //string cssFilePath = Path.Combine(rootPath, "wwwroot", "print.css");

                //var cssResolver = new StyleAttrCSSResolver();
                //using (var fs = new FileStream(cssFilePath, FileMode.Open))
                //{
                //    var cssFile = XMLWorkerHelper.GetCSS(fs);
                //    cssResolver.AddCss(cssFile);
                //}

                CssAppliers ca = new CssAppliersImpl();
                HtmlPipelineContext hpc = new HtmlPipelineContext(ca);
                hpc.SetTagFactory(Tags.GetHtmlTagProcessorFactory());


                //// PIPELINES
                PdfWriterPipeline pdf = new PdfWriterPipeline(doc, writer);
                HtmlPipeline htmlPipe = new HtmlPipeline(hpc, pdf);
                CssResolverPipeline css = new CssResolverPipeline(cssResolver, htmlPipe);

                XMLWorker worker = new XMLWorker(css, true);
                XMLParser p = new XMLParser(worker, Encoding.UTF8);
                StringReader sr = new StringReader(html);
                p.Parse(sr);
                doc.Close();

                return _memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                //eallySimpleLog.WriteLog(ex);
            }
            return _memoryStream.ToArray();
        }
        #endregion

        #region EmployeeCVPdf
        public static void ExportEmployeeContentToPDF(string reportContent, string reportName, string candidatePhoto, int left = 6, int right = 6, int top = 65, int bottom = 15, bool isLandScape = false, ExportDataTitle reportTitle = null)
        {
            try
            {
                candidatePhoto = (candidatePhoto == null) ? "" : candidatePhoto;
                var candidatePhotoPath = DU.Utility.UploadingFolderPath;
                if (!candidatePhoto.Contains(candidatePhotoPath))
                    candidatePhotoPath = candidatePhotoPath + candidatePhoto;

                Document doc = new Document(PageSize.A4, left, right, top, bottom);
                if (isLandScape)
                    doc.SetPageSize(PageSize.A4.Rotate());

                reportName = reportName + ".pdf";

                string html = reportContent;
                html = Regex.Replace(html, "</?(a|A).*?>", "");

                MemoryStream _memoryStream = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(doc, _memoryStream);
                ITextEvent eventHead = new ITextEvent();
                eventHead.FormatData = reportTitle;
                writer.PageEvent = eventHead;
                doc.Open();


                //candidate image
                if (candidatePhoto != null && !string.IsNullOrEmpty(candidatePhoto))
                {
                    try
                    {
                        iTextSharp.text.Image candidateImg = iTextSharp.text.Image.GetInstance(candidatePhotoPath);

                        //Resize image depend upon your need
                        candidateImg.ScaleToFit(140f, 100f);
                        candidateImg.SpacingBefore = 10f;
                        candidateImg.SpacingAfter = 1f;
                        candidateImg.BorderWidthLeft = 5f;
                        candidateImg.BorderWidthTop = 5f;
                        candidateImg.BorderWidthRight = 5f;
                        candidateImg.BorderWidthBottom = 5f;
                        candidateImg.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);
                        candidateImg.SetAbsolutePosition(440, 620);
                        doc.Add(candidateImg);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }


                // CSS
                var cssFilePath = DU.Utility.UploadingFolderPath + "print.css";
                var cssResolver = new StyleAttrCSSResolver();
                var cssFile = XMLWorkerHelper.GetCSS(new FileStream($@"{cssFilePath}", FileMode.Open));
                cssResolver.AddCss(cssFile);

                // HTML
                CssAppliers ca = new CssAppliersImpl();
                HtmlPipelineContext hpc = new HtmlPipelineContext(ca);
                hpc.SetTagFactory(Tags.GetHtmlTagProcessorFactory());

                // PIPELINES
                PdfWriterPipeline pdf = new PdfWriterPipeline(doc, writer);
                HtmlPipeline htmlPipe = new HtmlPipeline(hpc, pdf);
                CssResolverPipeline css = new CssResolverPipeline(cssResolver, htmlPipe);

                XMLWorker worker = new XMLWorker(css, true);
                XMLParser p = new XMLParser(worker);
                StringReader sr = new StringReader(html);
                p.Parse(sr);

                doc.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion

        #region BillPdf

        public byte[] ExportBillContentToPdfWithLogo(string reportContent, string reportName, bool companyImage, int left = 6, int right = 6, int top = 65, int bottom = 15, bool isLandScape = false, ExportDataTitle reportTitle = null, bool withFooter = true)
        {
            MemoryStream _memoryStream = new MemoryStream();
            try
            {
                Document doc = new Document(PageSize.A4, left, right, top, bottom);
                if (isLandScape)
                    doc.SetPageSize(PageSize.A4.Rotate());

                reportName = reportName + ".pdf";

                string html = reportContent.ToString();
                html = Regex.Replace(html, "</?(a|A).*?>", "");

                PdfWriter writer = PdfWriter.GetInstance(doc, _memoryStream);
                //writer.CloseStream = false;
                //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(reportName, FileMode.Create));
                if (withFooter)
                {
                    ITextEventBill eventHead = new ITextEventBill();
                    eventHead.FormatData = reportTitle;
                    writer.PageEvent = eventHead;
                }
                doc.Open();

                // CSS
                var cssFilePath = DU.Utility.ServerPath + "\\css\\print.css";
                var cssResolver = new StyleAttrCSSResolver();
                var cssFile = XMLWorkerHelper.GetCSS(new FileStream($@"{cssFilePath}", FileMode.Open));
                cssResolver.AddCss(cssFile);

                XMLWorkerFontProvider fontProvider = new XMLWorkerFontProvider();
                fontProvider.Register(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.TTF"), "TimeNewRoman");


                CssAppliers ca = new CssAppliersImpl(fontProvider);
                HtmlPipelineContext hpc = new HtmlPipelineContext(ca);
                hpc.SetTagFactory(Tags.GetHtmlTagProcessorFactory());


                //// PIPELINES
                PdfWriterPipeline pdf = new PdfWriterPipeline(doc, writer);
                HtmlPipeline htmlPipe = new HtmlPipeline(hpc, pdf);
                CssResolverPipeline css = new CssResolverPipeline(cssResolver, htmlPipe);

                XMLWorker worker = new XMLWorker(css, true);
                XMLParser p = new XMLParser(worker, Encoding.UTF8);
                StringReader sr = new StringReader(html);
                p.Parse(sr);
                doc.Close();

                //_memoryStream.Dispose();

                return _memoryStream.ToArray();



            }
            catch (Exception ex)
            {
                //eallySimpleLog.WriteLog(ex);
            }
            return _memoryStream.ToArray();
        }

        #endregion

        #region PosPrinterBillWithLogo

        public byte[] ExportPosBillContentLogoToPdf(string reportContent, string reportName, bool companyImage, int left = 6, int right = 6, int top = 65, int bottom = 15, bool isLandScape = false, ExportDataTitle reportTitle = null, bool withFooter = true, float pgHeight = 80)
        {
            MemoryStream _memoryStream = new MemoryStream();
            try
            {
                var widthPoint = Utilities.MillimetersToPoints(75);
                var heightPoint = Utilities.MillimetersToPoints(pgHeight);

                var pgSize = new Rectangle(widthPoint, heightPoint);

                Document doc = new Document(pgSize, left, right, top, bottom);
                if (isLandScape)
                    doc.SetPageSize(PageSize.A4.Rotate());

                reportName = reportName + ".pdf";

                string html = reportContent.ToString();
                html = Regex.Replace(html, "</?(a|A).*?>", "");

                PdfWriter writer = PdfWriter.GetInstance(doc, _memoryStream);

                if (withFooter)
                {
                    ITextEvent eventHead = new ITextEvent();
                    eventHead.FormatData = reportTitle;
                    writer.PageEvent = eventHead;
                }
                doc.Open();

                //company image
                if (companyImage)
                {
                    try
                    {
                        string imageURL = DU.Utility.ServerPath + "\\misuk_logo_sm.jpg";
                        iTextSharp.text.Image logoImg = iTextSharp.text.Image.GetInstance(imageURL);

                        //Resize image depend upon your need
                        logoImg.ScaleToFit(150f, 150f);
                        logoImg.SpacingBefore = 1f;
                        logoImg.SpacingAfter = 1f;
                        logoImg.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);

                        // Set the image to top center
                        float docWidth = doc.PageSize.Width;
                        float imageWidth = logoImg.ScaledWidth;
                        float centerX = (docWidth - imageWidth) / 2;
                        float topY = doc.PageSize.Height - logoImg.ScaledHeight - 5; // Adjust 20 for top margin if needed

                        logoImg.SetAbsolutePosition(centerX, topY);

                        doc.Add(logoImg);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

                // CSS

                var cssFilePath = DU.Utility.UploadingFolderPath + "print.css";
                var cssResolver = new StyleAttrCSSResolver();
                var cssFile = XMLWorkerHelper.GetCSS(new FileStream($@"{cssFilePath}", FileMode.Open));
                cssResolver.AddCss(cssFile);

                CssAppliers ca = new CssAppliersImpl();
                HtmlPipelineContext hpc = new HtmlPipelineContext(ca);
                hpc.SetTagFactory(Tags.GetHtmlTagProcessorFactory());


                //// PIPELINES
                PdfWriterPipeline pdf = new PdfWriterPipeline(doc, writer);
                HtmlPipeline htmlPipe = new HtmlPipeline(hpc, pdf);
                CssResolverPipeline css = new CssResolverPipeline(cssResolver, htmlPipe);

                XMLWorker worker = new XMLWorker(css, true);
                XMLParser p = new XMLParser(worker, Encoding.UTF8);
                StringReader sr = new StringReader(html);
                p.Parse(sr);
                doc.Close();

                return _memoryStream.ToArray();

            }
            catch (Exception ex)
            {
            }

            return _memoryStream.ToArray();
        }

        #endregion
    }
}
