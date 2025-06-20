using Bulk_Sign_Certificates.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Maui.Controls;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Bulk_Sign_Certificates.Components.Pages
{
    public partial class Login
    {
        LoginOutput loginDto = new();
        [Inject]
        protected NavigationManager Navigation { get; set; }

        void clicklogin(EditContext context)
        {
            if (context.Validate()) // <-- this checks if model is valid
            {
                var model = context.Model as LoginOutput;
                if (model != null)
                {
                    Console.WriteLine(model.email);
                    System.Diagnostics.Debug.WriteLine(model.email);
                    var client = new RestClient("http://localhost:7000");
                    var request = new RestRequest("/user/login",Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("User-Agent", "insomnia/11.0.0");
                    request.AddParameter("application/json", " {\n  \"email\": \""+model.email+"\",\n\"password\": \""+model.password+"\"\n}", ParameterType.RequestBody);
                    RestResponse response = client.Execute(request);
                    if (response.IsSuccessful)
                    {
                        Navigation.NavigateTo("/home");
                        System.Diagnostics.Debug.WriteLine(response.Content);
                    }
                    else
                    {
                        
                        System.Diagnostics.Debug.WriteLine(response.ErrorMessage);
                    }

                    // Perform your login logic here
                    // For example: await AuthService.LoginAsync(model);
                }
                else
                {
                    Console.WriteLine("Invalid model type.");
                }
            }
            else
            {
                Console.WriteLine("Model is invalid. Please correct the errors.");
            }
        }


    }
}
