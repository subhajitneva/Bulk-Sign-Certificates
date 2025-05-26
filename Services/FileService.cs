using Bulk_Sign_Certificates.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Sign_Certificates.Services
{
    public class FileService
    {
        public void deleteAllFilesFromFolder(string folderName)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);

            if (!Directory.Exists(folderPath))
                return;

            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    // Optionally log or handle the error
                    Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                }
            }
        }
        public List<PdfFile> getAllSignedPdfsAsBase64()
        {
            var result = new List<PdfFile>();

            string signedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Signed_pdf_files");
            if (!Directory.Exists(signedFolder))
                return result;

            var files = Directory.GetFiles(signedFolder, "*.pdf");

            foreach (var filePath in files)
            {
                var fileName = Path.GetFileName(filePath);
                var fileBytes = File.ReadAllBytes(filePath);
                var base64 = Convert.ToBase64String(fileBytes);

                result.Add(new PdfFile
                {
                    fileName = fileName,
                    base64Content = base64,
                    contentType = "application/pdf"
                });
            }

            return result;
        }
        
    }
}
