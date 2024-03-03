using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmployeeTask.Server.Hubs
{
    [Authorize]
    public class SignalRHub : Hub
    {
        public class usermessage
        {
            public string username { get; set; }
            public List<string> message { get; set; } = new List<string>();
        }

        private static List<ConnectedUser> connectedUsers = new List<ConnectedUser>();
        private static List<string> lists = new List<string>();
        private static List<usermessage> Messages = new List<usermessage>();
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var user = connectedUsers.Where(cu => cu.UserIdentifier == Context.UserIdentifier).FirstOrDefault();

            var connection = user.Connections.Where(c => c.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var count = user.Connections.Count;

            if (count == 1) // A single connection: remove user
            {
                connectedUsers.Remove(user);

            }
            if (count > 1) // Multiple connection: Remove current connection
            {
                user.Connections.Remove(connection);
            }
        }


        public override async Task OnConnectedAsync()
        {
            var user = connectedUsers.Where(cu => cu.UserIdentifier == Context.UserIdentifier).FirstOrDefault();

            if (user == null) // User does not exist
            {
                ConnectedUser connectedUser = new ConnectedUser
                {
                    UserIdentifier = Context.UserIdentifier,
                    Name = Context.User.Identity.Name,
                    Connections = new List<Connection> { new Connection { ConnectionID = Context.ConnectionId } }
                };

                connectedUsers.Add(connectedUser);
            }
            else
            {
                user.Connections.Add(new Connection { ConnectionID = Context.ConnectionId });
            }


        }
        public void storependingrequest(string userId)
        {
            lists.Add(userId);
        }
        public async Task UserConnected()
        {
            var userID = Context.UserIdentifier;
            var userI2D = Context.UserIdentifier;
            if (userID == "manish@gmail.com")
            {
                userID = "Satish";
            }
            if (userID == "satish@gmail.com")
            {
                userID = "Manish";
            }
            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.Name == userID
                                  select _connectedUser.UserIdentifier).FirstOrDefault();
            if(userIdentifier != null)
            {
                await Clients.User(userIdentifier).SendAsync("UserConnectedd", userIdentifier);
            }
            else
            {
                storependingrequest("manish@gmail.com");
            }

            if (lists!=null&& lists.Any(x=>x.Equals(userI2D)))
            {
                await Clients.User(lists.FirstOrDefault()!).SendAsync("UserConnectedd", lists.FirstOrDefault()!);
            }
            if(Messages != null && Messages.Any(x=>x.username == userI2D))
            {
                await Clients.User(userI2D).SendAsync("RefreshMessagess",Messages.FirstOrDefault(x=>x.username.Equals(userI2D))?.message);
            }
        }
        public async Task UserDisConnected()
        {
            var userID = Context.UserIdentifier;
            if (userID == "manish@gmail.com")
            {
                userID = "Satish";
            }
            if (userID == "satish@gmail.com")
            {
                userID = "Manish";
            }
            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.Name == userID
                                  select _connectedUser.UserIdentifier).FirstOrDefault();

            await Clients.User(userIdentifier).SendAsync("UserDisConnectedd");
        }
        public async Task RefreshMessages(string message)
        {
            var userID = Context.UserIdentifier;
            if (userID == "manish@gmail.com")
            {
                userID = "Satish";
            }
            if (userID == "satish@gmail.com")
            {
                userID = "Manish";
            }
            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.Name == userID
                                  select _connectedUser.UserIdentifier).FirstOrDefault();
            if(userIdentifier !=null)
            {
                await Clients.User(userIdentifier).SendAsync("RefreshMessagess", new List<string> { message });
            }
            else
            {
                usermessage usermessage = new usermessage();
                usermessage.message.Add(message);
                usermessage.username = "manish@gmail.com";
                
                if (Messages.Any(x=>x.username == usermessage.username))
                {
                    int index = Messages.FindIndex(u => u.username == usermessage.username);
                    Messages[index].message.Add(message);
                }
                else
                {
                    Messages.Add(usermessage);
                }
            }

        }
        public async Task RefreshEmployees(string hello)
        {
            var userID = Context.UserIdentifier;
            //var conid = Context.ConnectionId;
            //List<string> employees = new List<string>();
            if(userID == "manish@gmail.com")
            {
                userID = "Satish";
            }
            if(userID == "satish@gmail.com")
            {
                userID = "Manish";
            }
            //if(user == "manfdj@gmail.com")
            //{
            //    employees = new List<string>() { "manfdjw@gmail.com", "aditi@gmail.com" };
            //}
            //if(user == "manfdjw@gmail.com")
            //{
            //    employees = new List<string>() { "manfdj@gmail.com", "aditi@gmail.com" };
            //}
            //if(employees !=null )
            //{
            //    await Clients.Clients(employees).SendAsync("RefreshEmployees", "Hello");
            //}

            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.Name == userID
                                  select _connectedUser.UserIdentifier).FirstOrDefault();

            await Clients.User(userIdentifier).SendAsync("RefreshEmployees");

        }
    }
    public class ConnectedUser
    {
        public string Name { get; set; }
        public string UserIdentifier { get; set; }

        public List<Connection> Connections { get; set; }
    }
    public class Connection
    {
        public string ConnectionID { get; set; }

    }
}
