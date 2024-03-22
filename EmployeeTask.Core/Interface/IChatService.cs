using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTask.Core.Interface
{
    public interface IChatService
    {
        Task SaveUserChat(ChatModel chat);
        Task<List<ChatModel>> GetConversationAsync(string id);
    }
}
