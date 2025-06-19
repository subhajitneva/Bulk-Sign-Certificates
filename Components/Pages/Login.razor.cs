using Bulk_Sign_Certificates.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Bulk_Sign_Certificates.Components.Pages
{
    public partial class Login
    {
        LoginOutput loginDto = new();
        LoginOutput output = new ();
        void clicklogin(EditContext context)
        {
            
            var model = context.Model as LoginOutput;
            if (model != null)
            {
                output = model;
                Console.WriteLine(model.email);
            }
            else
            {
                Console.WriteLine("Invalid model type.");
            }

        }
        
    }
}
