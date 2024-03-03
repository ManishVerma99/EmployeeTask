namespace EmployeeTask.Client.Pages
{
    public partial class TaskGrid
    {
        #region Private Fields
        private MudTable<AssignedTaskModel> table;
        private bool IsAdmin { get; set; }
        #endregion

        #region Parameters
        [Parameter] public string Id { get; set; }
        #endregion

        #region Services
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthProvider.GetAuthenticationStateAsync();
            IsAdmin = authenticationState.User.IsInRole("Admin");
        }

        private async Task<TableData<AssignedTaskModel>> GetTasks(TableState tableState)
        {
            var authenticationState = await AuthProvider.GetAuthenticationStateAsync();
            var role = authenticationState.User.FindFirst(ClaimTypes.Role).Value;
            var result = await _httpClient.GetFromJsonAsync<List<AssignedTaskModel>>($"{ApplicationRoutes.Url}Task/{(role == "Admin" ? "GetTasksForAdminEmployee" : "GetTasksByEmployeeId")}/{Id}");
            return new TableData<AssignedTaskModel> { Items = result };
        }

        private async Task OpenEditTaskDialog(AssignedTaskModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("AssignedTask", model);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<CreateTask>("Modal", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await table.ReloadServerData();
            }
        }

        private async Task DeleteTask(int? id)
        {
            var result = await _httpClient.DeleteAsync($"{ApplicationRoutes.Url}Task/DeleteTask/{id}");
            var response = await result.Content.ReadFromJsonAsync<Response>();
            if (response != null && response.Status == "Success")
            {
                await table.ReloadServerData();
            }
        }
    }
}
