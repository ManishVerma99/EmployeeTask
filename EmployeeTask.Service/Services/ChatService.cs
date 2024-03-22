using EmployeeTask.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTask.Service.Services
{
    public class ChatService:IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }
        public async Task SaveUserChat(ChatModel chatModel)
        {
            Chat chat = new Chat()
            {
                Message = chatModel.Message,
                ToUserId = chatModel.ToUserId,
                FromUserId = chatModel.FromUserId,
                CreatedDate = chatModel.CreatedDate
            };
            await _chatRepository.SaveUserChat(chat);
        }

        public async Task<List<ChatModel>> GetConversationAsync(string id)
        {
            var chatList =  await _chatRepository.GetConversationAsync(id);
            List<ChatModel> chatModelList = chatList.Select(chat => new ChatModel
            {
                ToUserId = chat.ToUserId,
                FromUserId = chat.FromUserId,
                Message = chat.Message,
                CreatedDate = chat.CreatedDate,
                Id = chat.Id
            }).ToList();
            return chatModelList;
        }
    }
}
