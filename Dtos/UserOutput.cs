using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Sign_Certificates.Dtos
{
    public class UserOutput
    {
        public Result result { get; set; }
        public bool error { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }
    public class Result
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool isActive { get; set; }
        public string status { get; set; }
    }
}
