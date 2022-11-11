
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Forms.Fields;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Forms;

namespace XafPlayingWithPDF.Module.Helper
{
    public static class PDFHelper
    {
        //HACK: https://kb.itextpdf.com/home/it7kb/examples/adding-page-numbers-to-an-existing-pdf
        /// <summary>
        /// Merge a list of PDF's Files into a New One.
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="targetPdf"></param>
        /// <returns></returns>
        static public bool MergePDFs(List<string> fileNames, string targetPdf)
        {
            bool merged = true;

            try
            {
                File.Delete(targetPdf);

                PdfDocument document = new PdfDocument(new PdfWriter(targetPdf));
                PdfMerger merger = new PdfMerger(document);


                foreach (string file in fileNames)
                {
                    PdfDocument sourcePdf = new PdfDocument(new PdfReader(file));
                    merger.Merge(sourcePdf, 1, sourcePdf.GetNumberOfPages());
                }

                document.Close();
                merger.Close();
            }
            catch (Exception ex)
            {
                merged = false;
                Console.WriteLine("Error while trying to merge files: " + ex.Message);
            }


            return merged;
        }

        /// <summary>
        /// Add page number to a PDF document and Save as a New one
        /// </summary>
        /// <param name="pdfSourceName"></param>
        /// <param name="pdfTargetName"></param>
        /// <returns></returns>
        public static bool EnumeratePDF(string pdfSourceName, string pdfTargetName)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(pdfSourceName);
                pdfReader.SetUnethicalReading(true);

                PdfDocument pdfDoc = new PdfDocument(pdfReader, new PdfWriter(pdfTargetName));
                iText.Layout.Document doc = new iText.Layout.Document(pdfDoc);

                int numberOfPages = pdfDoc.GetNumberOfPages();
                for (int i = 1; i <= numberOfPages; i++)
                {
                    // Write aligned text to the specified by parameters point
                    doc.ShowTextAligned(
                        new Paragraph("page " + i + " of " + numberOfPages),
                        559,
                        806,
                        i,
                        TextAlignment.RIGHT,
                        VerticalAlignment.TOP,
                        0);
                }

                doc.Close();
                pdfDoc.Close();


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Fill PDF information base on a List of Fields and Values
        /// </summary>
        /// <param name="pdfSourceName"></param>
        /// <param name="pdfTargetName"></param>
        /// <param name="fieldsAndValues"></param>
        /// <returns></returns>
        public static bool FillPDFForm(
            string pdfSourceName,
            string pdfTargetName,
            List<KeyValuePair<string, string>> fieldsAndValues)
        {
            bool lConverted = true;
            try
            {
                PdfReader pdfReader = new PdfReader(pdfSourceName);
                pdfReader.SetUnethicalReading(true);

                PdfDocument pdfDoc = new PdfDocument(pdfReader, new PdfWriter(pdfTargetName));

                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

                form.RemoveXfaForm();

                var fields = form.GetFormFields();

                foreach (var field in fields)
                {
                    var fieldValue = form.GetField(field.Key).GetValue();
                    if (fieldValue != null)
                    {
                        if (!string.IsNullOrEmpty(fieldValue.ToString()))
                        {
                            if (fieldsAndValues.Where(f => f.Key == fieldValue.ToString()).Any())
                            {
                                string value = fieldsAndValues.Where(f => f.Key == fieldValue.ToString())
                                    .FirstOrDefault()
                                    .Value;
                                form.GetField(field.Key).SetValue(value);
                                Console.WriteLine(fieldValue);
                            }
                        }
                    }
                }

                pdfDoc.Close();
                pdfReader.Close();
            }
            catch (Exception ex)
            {
                lConverted = false;
                Console.WriteLine(ex.Message);
            }

            return lConverted;
        }

    }
}
