using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTask.Data.IRepository
{
    public interface IChatRepository
    {
         Task SaveUserChat(Chat chat);
         Task<List<Chat>> GetConversationAsync(string id);
    }
}
