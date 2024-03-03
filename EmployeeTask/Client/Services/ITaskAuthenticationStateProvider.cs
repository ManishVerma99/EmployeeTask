namespace EmployeeTask.Client.Services
{
    public interface ITaskAuthenticationStateProvider
    {
        Task MarkUserAsAuthenticated(LogInUserDetailViewModel user);
        Task MarkUserAsLoggedOut();
    }
}
