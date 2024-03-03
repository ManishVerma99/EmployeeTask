namespace EmployeeTask.Client.Pages
{
    public partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await _authProvider.GetAuthenticationStateAsync();
            var role = authenticationState.User.FindFirst(ClaimTypes.Role).Value;
            var id = authenticationState.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var route = role == "Admin" ? "/EmployeeGrid" : $"/TaskGrid/{id}";
            Navigation.NavigateTo(route);
        }
    }
}
