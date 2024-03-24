using EmployeeTask.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EmployeeTask.Server.Hubs
{
    [Authorize]
    public class SignalRHub : Hub
    {
        private static List<ConnectedUser> connectedUsers = new List<ConnectedUser>();
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var user = connectedUsers.Where(cu => cu.UserIdentifier == Context.UserIdentifier).FirstOrDefault();
            connectedUsers.Remove(user);
            await Clients.Users(connectedUsers.Select(x=>x.UserIdentifier).ToList()).SendAsync("NotifyUser");
            //var connection = user.Connections.Where(c => c.ConnectionID == Context.ConnectionId).FirstOrDefault();
            //var count = user.Connections.Count;

            //if (count == 1)
            //{
            //    connectedUsers.Remove(user);

            //}
            //if (count > 1)
            //{
            //    user.Connections.Remove(connection);
            //}
        }

        public List<ConnectedUser> GetConnectedUsers()
        {
            return connectedUsers;
        }

        public async Task SendMessage(ChatModel chat,string toUserEmail)
        {
            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.UserIdentifier == toUserEmail
								  select _connectedUser.UserIdentifier).FirstOrDefault();
            if (userIdentifier != null)
            {
                await Clients.User(userIdentifier).SendAsync("RecieveMessage", chat);
            }
        }

        public async Task MessageInput(string toUserEmail)
        {
            var userIdentifier = (from _connectedUser in connectedUsers
                                  where _connectedUser.UserIdentifier == toUserEmail
                                  select _connectedUser.UserIdentifier).FirstOrDefault();
            if (userIdentifier != null)
            {
                await Clients.User(userIdentifier).SendAsync("OnMessageInput", toUserEmail);
            }
        }


        public override async Task OnConnectedAsync()
        {
            var user = connectedUsers.Where(cu => cu.UserIdentifier == Context.UserIdentifier).FirstOrDefault();
            if (user == null)
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

            var list = connectedUsers.Where(x=>x.UserIdentifier!=Context.UserIdentifier).Select(x => x.UserIdentifier).ToList();
            if (list != null && list.Count > 0)
            {
                await Clients.Users(list).SendAsync("NotifyUser");

            }
        }
    }
   
}
