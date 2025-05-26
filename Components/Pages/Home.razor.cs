using Bulk_Sign_Certificates.Dtos;
using Bulk_Sign_Certificates.Services;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace Bulk_Sign_Certificates.Components.Pages
{
    public partial class Home
    {
        HttpClient client = new HttpClient();
        public string message = "";
        public string errorMessage = "";
        string localFolderPdf;
        string signedFolderPdf;
        public List<X509Certificate2> cert = new();
        public X509Certificate2? SelectedCertificate { get; set; }
        public List<PdfFile> pdfFiles = new();
        FileService fileService;
        string tokenValue;
        public Home(FileService _fileService)
        {
            // Constructor logic can be added here if needed
            this.fileService = _fileService;
            tokenValue = Preferences.Get("token", "");
        }
        protected override void OnInitialized()
        {
            localFolderPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pdf_files");
            signedFolderPdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Signed_pdf_files");
            Directory.CreateDirectory(localFolderPdf);
            Directory.CreateDirectory(signedFolderPdf);
            cert = GetCertificates();
        }

        protected void RefreshCertificates()
        {
            cert = GetCertificates();
            SelectedCertificate = null;
        }

        protected void OnCertificateSelected(ChangeEventArgs e)
        {
            var thumbprint = e.Value?.ToString();
            SelectedCertificate = cert.FirstOrDefault(c => c.Thumbprint == thumbprint);
        }

        protected void loadCert()
        {
            cert = GetCertificates();
        }

        public List<X509Certificate2> GetCertificates()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates
                .Find(X509FindType.FindByTimeValid, DateTime.Now, false)
                .Cast<X509Certificate2>()
                .Where(cert => cert.HasPrivateKey)
                .ToList();

            store.Close();
            return certificates;
        }

        protected async Task getFilesFromWebAsync()
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://localhost:5008/api/Files/download-all"),
                    Headers = { { "User-Agent", "insomnia/11.1.0" } },
                };

                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var body = await response.Content.ReadAsStringAsync();
                var files = JsonSerializer.Deserialize<List<PdfFile>>(body);

                if (files != null)
                {
                    pdfFiles.Clear();
                    foreach (var file in files)
                    {
                        pdfFiles.Add(new PdfFile
                        {
                            fileName = file.fileName,
                            contentType = file.contentType,
                            base64Content = file.base64Content,
                            isSelected = false
                        });

                        var filePath = Path.Combine(localFolderPdf, file.fileName);
                        var fileBytes = Convert.FromBase64String(file.base64Content);
                        File.WriteAllBytes(filePath, fileBytes);
                    }
                    message = "Files loaded and saved successfully.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error: {ex.Message}";
            }
        }
        protected void signSelectedButton_Click()
        {
            if (SelectedCertificate == null)
            {
                errorMessage = "Please select a certificate.";
                return;
            }
            if (pdfFiles.Count == 0)
            {
                errorMessage = "No PDF files to sign.";
                return;
            }
            Console.WriteLine("selected cert are ");

            foreach (PdfFile file in pdfFiles)
            {

                if (file != null && file.isSelected)
                {
                    string inputFilePath = Path.Combine(localFolderPdf, file.fileName);
                    string outputFilePath = Path.Combine(signedFolderPdf, "signed_" + file.fileName);

                    SignPdf(inputFilePath, outputFilePath, SelectedCertificate);
                }
            }
            var fileList = fileService.getAllSignedPdfsAsBase64();
            fileService.deleteAllFilesFromFolder("Pdf_files");
            fileService.deleteAllFilesFromFolder("Signed_pdf_files");
            sendSignedFilesToServer(fileList);
            message = "Selected PDFs have been signed and sent to the server.";
        }
        public string sortText(string input, int length)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (input.Length< length) return input;                  
            return input.Substring(0, length) + "........";
        }
        public void SignPdf(string inputPdfPath, string outputPdfPath, X509Certificate2 cert)
        {
            var reader = new PdfReader(inputPdfPath);
            using var outputStream = new FileStream(outputPdfPath, FileMode.Create);

            var stamper = PdfStamper.CreateSignature(reader, outputStream, '\0');
            var appearance = stamper.SignatureAppearance;
            appearance.Reason = "Document signed";
            appearance.Location = "Digital Cert Desktop";
            appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), 1, null);

            var chain = new[] { DotNetUtilities.FromX509Certificate(cert) };

            // Try wrapping with the RSACng algorithm
            var rsa = cert.GetRSAPrivateKey(); // RSACng
            var signature = new CustomRSACngSignature(rsa, cert, "SHA256");

            MakeSignature.SignDetached(appearance, signature, chain, null, null, null, 0, CryptoStandard.CADES);
        }
        public async void sendSignedFilesToServer(List<PdfFile> pdfFiles)
        {
            string jsonString = JsonSerializer.Serialize(pdfFiles, new JsonSerializerOptions { WriteIndented = true });
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:5008/api/Files/loadPDFlist"),
                Headers =
                {
                    { "User-Agent", "insomnia/11.1.0" },
                    { "Authorization", "Bearer "+ tokenValue },
                },

                Content = new StringContent(jsonString)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);
            }
        }
        protected void ToggleAll(MouseEventArgs e)
        {
            bool check = !pdfFiles.All(f => f.isSelected);
            foreach (var file in pdfFiles)
            {
                file.isSelected = check;
            }
        }
        protected void emptyMessage(bool err = false)
        {
            if (err)
            {
                message = string.Empty;
            }
            else
            {
                errorMessage = string.Empty;
            }
        }
    }

}
