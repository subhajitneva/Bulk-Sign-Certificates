using Bulk_Sign_Certificates.Dtos;
using Microsoft.AspNetCore.Components;
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
        string userName;
        string password;
        HttpClient client = new HttpClient();
        [Inject]
        protected NavigationManager Navigation { get; set; }
        protected void RedirectToHome()
        {
            
            Navigation.NavigateTo("/home");
        }
        public async void LoginUser()
        {

            LoginOutput loginOutput = new LoginOutput();
            loginOutput.email = userName;
            loginOutput.password = password;
            string jsonString = JsonSerializer.Serialize(loginOutput, new JsonSerializerOptions { WriteIndented = true });
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://localhost:7000/user/login"),
                Headers =
                {
                    { "User-Agent", "insomnia/11.0.0" },
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
                var jsonData = JsonSerializer.Deserialize<UserOutput>(body);
                var token = jsonData.token;
                Preferences.Set("token", token);
                Console.WriteLine(body);
                Navigation.NavigateTo("/home");
            }
            // Constructor logic can be added here if needed
        }
    }
}
