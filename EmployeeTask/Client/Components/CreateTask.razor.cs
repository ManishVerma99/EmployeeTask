namespace EmployeeTask.Client.Components
{
    public partial class CreateTask : IDisposable
    {
        #region Services
        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public AssignedTaskModel AssignedTask { get; set; } = new AssignedTaskModel();
        [Parameter]
        public string Id { get; set; }
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; }
        #endregion

        #region Private Fields
        private bool IsAdmin { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            IsAdmin = authenticationState.User.IsInRole("Admin");
        }

        private async Task CreateOrUpdateTask()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            Response response = new Response();
            if(AssignedTask.Id == 0)
            {
                httpResponseMessage = await _httpClient.GetAsync($"{ApplicationRoutes.Url}Employee/GetEmployee/{Id}");
                var employee = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();
                if (employee == null)
                {
                    return;
                }
                AssignedTask.EmployeeId = employee.Id;
                httpResponseMessage = await _httpClient.PostAsJsonAsync($"{ApplicationRoutes.Url}Task/CreateTask", AssignedTask);
            }
            else
            {
                httpResponseMessage = await _httpClient.PostAsJsonAsync($"{ApplicationRoutes.Url}Task/UpdateTask", AssignedTask);
            }
            
            response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            if (response != null && response.Status == "Success")
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
    }
}
