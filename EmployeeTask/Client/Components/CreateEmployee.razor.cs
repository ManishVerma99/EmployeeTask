using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Headers;

namespace EmployeeTask.Client.Components
{
    public partial class CreateEmployee : IDisposable
    {
        #region Parameters
        public MainLayout MainLayout { get; set; }
        [Parameter]
        public RegisterModel Register { get; set; } = new RegisterModel();
        [Parameter]
        public HubConnection? HubConnection { get; set; }
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
       
        #endregion

        #region Services
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public ILocalStorageService _localStorageService { get; set; }
        #endregion

        private async Task CreateOrUpdateEmployee()
        {
            var user = await AuthProvider.GetAuthenticationStateAsync();
            Register.CreatedBy = user.User.FindFirst(ClaimTypes.Name).Value;
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            if (Register.Id == null)
            {
                var savedToken = await _localStorageService.GetItemAsync<string>("token");

                // Add the access token to the request headers
              //  _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                httpResponseMessage = await _httpClient.PostAsJsonAsync($"{ApplicationRoutes.Url}Authenticate/register", Register);
            }
            else
            {
                UpdateEmployee updateEmployee = new UpdateEmployee
                {
                    FirstName = Register.FirstName,
                    LastName = Register.LastName,
                    Email = Register.Email,
                    Username = Register.Username,
                    Id = Register.Id
                };
                httpResponseMessage = await _httpClient.PostAsJsonAsync($"{ApplicationRoutes.Url}Employee/UpdateEmployee", updateEmployee);
            }
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            if (httpResponseMessage != null && response.Status == "Success")
            {
                MudDialog.Close(DialogResult.Ok(true));
                HubConnection?.InvokeAsync("RefreshEmployees", "Hello");
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
