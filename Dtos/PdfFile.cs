using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Sign_Certificates.Dtos
{
    public class PdfFile
    {
        public bool isSelected { get; set; } = false;
        public string fileName { get; set; }
        public string contentType { get; set; }
        public string base64Content { get; set; }
    }
}
