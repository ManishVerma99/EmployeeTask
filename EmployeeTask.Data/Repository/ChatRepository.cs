using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTask.Data.Repository
{
    public class ChatRepository:IChatRepository
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatRepository(ApplicationContext applicationContext,IHttpContextAccessor httpContextAccessor)
        {
            _applicationContext = applicationContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task SaveUserChat(Chat chat)
        {
            await _applicationContext.Chats.AddAsync(chat);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<List<Chat>> GetConversationAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst("UserId")?.Value;
            var result = await _applicationContext.Chats.Where(x =>
                    (x.FromUserId == id && x.ToUserId == userId) || (x.FromUserId == userId && x.ToUserId == id)).OrderBy(x=>x.CreatedDate)
                .ToListAsync();
            return result ?? new List<Chat>();

        }
    }
}
