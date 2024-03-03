using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmployeeTask.Client.Services
{
    public class UserProviderService : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var test =  connection.User?.FindFirst(ClaimTypes.Email)?.Value!;
            return test;
        }
    }
}
