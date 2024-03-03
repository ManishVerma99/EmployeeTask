namespace EmployeeTask.Client.Pages
{
    public partial class Login
    {
        #region Private Fields
        private LoginModel loginModel = new();
        #endregion

        #region Services
        [Inject] public ILocalStorageService LocalStorage { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        #endregion

        private async Task OnLogin()
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApplicationRoutes.Url}Authenticate/Login", loginModel);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<LogInUserDetailViewModel>();
                if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                {
                    await ((ITaskAuthenticationStateProvider)_authProvider).MarkUserAsAuthenticated(tokenResponse);
                }
                if (tokenResponse.RoleNames == "Admin")
                {
                    var user = await _authProvider.GetAuthenticationStateAsync();
                    var createdBy = user.User.FindFirst(ClaimTypes.Name).Value;
                    await LocalStorage.SetItemAsync("CreatedBy", createdBy);
                    NavigationManager.NavigateTo("/EmployeeGrid");
                }
                else
                {
                    NavigationManager.NavigateTo($"/TaskGrid/{tokenResponse.UserId}");
                }
            }
            else
            {
                // Handle login failure
            }
        }
    }
}
