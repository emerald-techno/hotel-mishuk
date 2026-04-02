using iTextSharp.text;
using iTextSharp.text.pdf;
using DU = Domain.Utility;

namespace Utility.Export
{
    public class ITextEventBill : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private ExportDataTitle _formatData;
        #endregion

        #region Properties
        public ExportDataTitle FormatData
        {
            get { return _formatData; }
            set { _formatData = value; }
        }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);
            Font baseFontSmall = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL, BaseColor.BLACK);
            Font baseFontNormal = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL, BaseColor.BLACK);
            Font baseFontBig = new Font(Font.FontFamily.TIMES_ROMAN, 13f, Font.BOLD, BaseColor.BLACK);
            //Font baseFontBig = new Font(Font.FontFamily.TIMES_ROMAN, 17f, Font.BOLD, BaseColor.BLACK);
            Font baseFontMidBold = new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK);
            Font baseFontNormalBold = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);

            //string imageURL = DU.Utility.ServerPath + "\\misuk_logo.jpg";
            //Image logo = Image.GetInstance(imageURL);
            //logo.ScalePercent(9);
            //logo.SetAbsolutePosition(document.PageSize.Width/2-60, document.PageSize.Height - 110);
            //cb.AddImage(logo);

            string imageURL = DU.Utility.ServerPath + "\\misuk_logo_sm.jpg";
            iTextSharp.text.Image candidateImg = iTextSharp.text.Image.GetInstance(imageURL);
            //Resize image depend upon your need
            candidateImg.ScaleToFit(80f, 80f);
            candidateImg.SpacingBefore = 10f;
            candidateImg.SpacingAfter = 1f;
            candidateImg.BorderColor = new iTextSharp.text.BaseColor(System.Drawing.Color.Gray);
            candidateImg.SetAbsolutePosition(document.PageSize.Width / 2 - 35, document.PageSize.Height - 70);
            cb.AddImage(candidateImg);


            Phrase compName = new Phrase(FormatData.Header, baseFontBig);//header text


            //Create PdfTable object
            PdfPTable pdfTab = new PdfPTable(FormatData.SignatureList.Count * 2 - 1);
            PdfPTable pdfTabHeader = new PdfPTable(3);
            PdfPTable pdfTranInfoOne = new PdfPTable(6);
            PdfPTable pdfTranInfoTwo = new PdfPTable(6);
            PdfPTable pdfTranInfoThree = new PdfPTable(6);
            PdfPTable pdfNoteTable = new PdfPTable(1);
            PdfPTable pdfCustomerSign = new PdfPTable(1);
            //pdfTabHeader.DefaultCell.FixedHeight = 100f;


            //We will have to create separate cells to include image logo and 2 separate strings
            //Row 1

            PdfPCell pdfCelHed1 = new PdfPCell();
            PdfPCell pdfCelHed2 = new PdfPCell(compName);
            PdfPCell pdfCelHed3 = new PdfPCell();
            PdfPCell pdfCelHed4 = new PdfPCell(new Phrase(FormatData.AddressOne, baseFontSmall));
            PdfPCell pdfCelHed5 = new PdfPCell(new Phrase(FormatData.AddressTwo, baseFontSmall));
            PdfPCell pdfCelHed6 = new PdfPCell(new Phrase(FormatData.ReportTitle, baseFontMidBold));

            //tranation header information

            if (FormatData.SignatureList.Count > 0)
            {
                foreach (string signature in FormatData.SignatureList)
                {
                    PdfPCell cellSign = new PdfPCell(new Phrase(signature, baseFontNormalBold));
                    PdfPCell signGap = new PdfPCell();
                    cellSign.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellSign.VerticalAlignment = Element.ALIGN_TOP;
                    cellSign.Border = 0;
                    signGap.Border = 0;
                    cellSign.BorderWidthTop = 1;

                    pdfTab.AddCell(cellSign);
                    pdfTab.AddCell(signGap);
                }
            }



            String text = "Page " + writer.PageNumber + " of ";

            if (FormatData.IsFooterChange)
            {
                //print date
                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.SetTextMatrix(38, document.PageSize.GetBottom(20));
                cb.ShowText($"{FormatData.AddressOne} | {FormatData.AddressTwo}");
                cb.EndText();
            }
            else
            {

                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.SetTextMatrix(48, document.PageSize.GetBottom(20));
                cb.ShowText("Developed By : Emerald Techno Ltd.");
                cb.EndText();

                //print date
                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.SetTextMatrix(250, document.PageSize.GetBottom(20));
                cb.ShowText("Print date : " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt"));
                cb.EndText();


                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.SetTextMatrix(document.PageSize.GetRight(80), document.PageSize.GetBottom(20));
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 6);
                cb.AddTemplate(footerTemplate, document.PageSize.GetRight(80) + len, document.PageSize.GetBottom(20));
            }

            pdfCelHed1.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCelHed2.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCelHed3.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCelHed4.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCelHed5.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCelHed6.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCelHed6.VerticalAlignment = Element.ALIGN_MIDDLE;

            pdfCelHed1.Border = 0;
            pdfCelHed2.Border = 0;
            pdfCelHed3.Border = 0;
            pdfCelHed4.Border = 0;
            pdfCelHed5.Border = 0;
            pdfCelHed6.Border = 0;


            pdfCelHed4.Colspan = 3;
            pdfCelHed5.Colspan = 3;
            pdfCelHed6.Colspan = 3;
            pdfCelHed6.PaddingTop = 10;//report title
                                       //pdfCelHed6.BackgroundColor = BaseColor.LIGHT_GRAY;


            pdfTabHeader.AddCell(pdfCelHed1);
            pdfTabHeader.AddCell(pdfCelHed2);
            pdfTabHeader.AddCell(pdfCelHed3);
            if (FormatData.AddressOne.ToString().Trim().Length > 0)
                pdfTabHeader.AddCell(pdfCelHed4);
            if (FormatData.AddressTwo.ToString().Trim().Length > 0)
                pdfTabHeader.AddCell(pdfCelHed5);
            if (FormatData.ReportTitle.ToString().Trim().Length > 0)
            {
                //pdfCelHed6.PaddingTop = 10;
                pdfTabHeader.AddCell(pdfCelHed6);
            }


            //header possition
            float headerPositionTop = (FormatData.IsHeaderLogo) ? document.PageSize.Height - 65 : document.PageSize.Height - 18;
            float[] widthsHead = new float[] { 10f, document.PageSize.Width - 50f, 10f };
            pdfTabHeader.SetWidths(widthsHead);
            pdfTabHeader.TotalWidth = document.PageSize.Width - 60f;
            pdfTabHeader.WidthPercentage = 98;

            pdfTabHeader.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfTabHeader.WriteSelectedRows(0, -1, 40, headerPositionTop, writer.DirectContent);

            //tansation heder Info 
            float[] widthsTranOne = new float[] { 12f, 27f, 12f, 27f, 12f, 27f };
            pdfTranInfoOne.SetWidths(widthsTranOne);
            pdfTranInfoOne.TotalWidth = document.PageSize.Width - 5f;
            pdfTranInfoOne.WidthPercentage = 98;
            pdfTranInfoOne.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfTranInfoOne.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 100, writer.DirectContent);

            //tansation heder Info Two 
            float[] widthsTranTwo = new float[] { 12f, document.PageSize.Width - 10f };
            pdfTranInfoTwo.SetWidths(widthsTranOne);
            pdfTranInfoTwo.TotalWidth = document.PageSize.Width - 5f;
            pdfTranInfoTwo.WidthPercentage = 98;
            pdfTranInfoTwo.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfTranInfoTwo.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 115, writer.DirectContent);

            //tansation heder Info Three 
            pdfTranInfoThree.SetWidths(widthsTranOne);
            pdfTranInfoThree.TotalWidth = document.PageSize.Width - 5f;
            pdfTranInfoThree.WidthPercentage = 98;
            pdfTranInfoThree.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfTranInfoThree.WriteSelectedRows(0, -1, 20, document.PageSize.Height - 130, writer.DirectContent);


            //using (var srHtml = new StringReader(FormatData.NoteText))
            //{
            //    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, cb.PdfDocument, srHtml);

            //}


            if (FormatData.IsSignature && FormatData.SignatureList.Count == 1)
            {
                float[] widths = new float[] { 45f };
                pdfTab.SetWidths(widths);
                pdfTab.TotalWidth = 100f;
                pdfTab.WidthPercentage = 50;
                pdfTab.WriteSelectedRows(0, -1, 50, document.PageSize.GetBottom(96) - 15, writer.DirectContent);
            }

            if (FormatData.IsSignature && FormatData.SignatureList.Count > 1)
            {
                //signature possition
                float[] widths = new float[] { 16f, 2f, 15f, 2f, 16f, 2f, 16f, 2f, 16f };
                switch (FormatData.SignatureList.Count)
                {
                    case 1:
                        widths = new float[] { 25f };
                        break;
                    case 2:
                        widths = new float[] { 25f, 20f, 25f };
                        break;
                    case 3:
                        widths = new float[] { 25f, 2f, 25f, 2f, 25f };
                        break;
                    case 4:
                        widths = new float[] { 20f, 2f, 20f, 2f, 20f, 2f, 20f };
                        break;
                    case 5:
                        widths = new float[] { 16f, 2f, 15f, 2f, 16f, 2f, 16f, 2f, 16f };
                        break;
                    default:
                        widths = new float[] { 16f, 2f, 15f, 2f, 16f, 2f, 16f, 2f, 16f };
                        break;
                }
                pdfTab.SetWidths(widths);
                pdfTab.TotalWidth = document.PageSize.Width - 60f;
                pdfTab.WidthPercentage = 98;
                pdfTab.WriteSelectedRows(0, -1, 20, document.PageSize.GetBottom(96) - 15, writer.DirectContent);
            }

            

            /*QR Code */
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode(FormatData.UniqueCode, QRCodeGenerator.ECCLevel.Q);
            //Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            //string qrCodeImageAsBase64 = qrCode.GetGraphic(3);
            //iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Convert.FromBase64String(qrCodeImageAsBase64));
            //img.SetAbsolutePosition(500, 740);
            //cb.PdfDocument.Add(img);


            //Move the pointer and draw line to separate footer section from rest of page
            cb.MoveTo(40, document.PageSize.GetBottom(40));
            cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(40));
            cb.Stroke();
        }


        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bf, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 6);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();


        }

    }
}