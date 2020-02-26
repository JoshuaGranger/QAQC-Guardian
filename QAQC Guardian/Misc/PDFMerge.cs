using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QAQC_Guardian.Models;
using System.Windows;

namespace QAQC_Guardian.Misc
{
    static class PDFMerge
    {
        public static void MergePDFs(IEnumerable<Document> pdfs)
        {
            // Assemble output filename and path
            string targetFile = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Environment.UserName.ToString()}.pdf";
            string targetPath = Globals.PathFiles + targetFile;

            // Check that the folder exists
            if (!System.IO.Directory.Exists(Globals.PathFiles))
                System.IO.Directory.CreateDirectory(Globals.PathFiles);

            // Check if the file exists. If so, add (2) to the end
            if (System.IO.File.Exists(targetPath))
                targetPath.Replace(".pdf", "(2).pdf");

            // Assemble document
            using (PdfDocument targetDoc = new PdfDocument())
            {
                if (pdfs.Any())
                {
                    foreach (Document pdf in pdfs)
                    {
                        if (File.Exists(pdf.FullPath))
                            using (PdfDocument pdfDoc = PdfReader.Open(pdf.FullPath, PdfDocumentOpenMode.Import))
                                for (int i = 0; i < pdfDoc.PageCount; i++)
                                    targetDoc.AddPage(pdfDoc.Pages[i]);
                        else
                            Misc.Globals.ShowMsg($"A document that was selected could not be found. Please try reloading the application.\n\nDocument:\n" +
                                $"{pdf.FileName}\n\nPath:\n{pdf.FullPath}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    

                    if (targetDoc.Pages.Count > 0)
                    {
                        if (!File.Exists(targetPath) || !IsFileinUse(new FileInfo(targetPath)))
                        {
                            // Save the file
                            try
                            {
                                targetDoc.Save(targetPath);
                            }
                            catch (Exception e)
                            {
                                Misc.Globals.ShowMsg($"An unexpected error occurred while trying to save the file.\nPlease try again.\n\nMessage details:\n{e.Message}\n\nTarget filepath:\n{targetPath}",
                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                            // Open the file
                            try
                            {
                                System.Diagnostics.Process.Start(targetPath);
                            }
                            catch (Exception e)
                            {
                                Misc.Globals.ShowMsg($"An unexpected error occurred while trying to open the file.\nPlease try again.\n\nMessage details:\n{e.Message}\n\nTarget filepath:\n{targetPath}",
                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }

                        }
                    }
                }
            }
        }

        public static bool IsFileinUse(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
    }
}
