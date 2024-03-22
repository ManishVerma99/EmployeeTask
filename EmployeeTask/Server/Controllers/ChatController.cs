using EmployeeTask.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTask.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        [Route("GetConversationAsync/{id}")]
        public async Task<IActionResult> GetConversationAsync(string id)
        {
            try
            {
                var chatList = await _chatService.GetConversationAsync(id);
                return Ok(chatList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("SaveMessageAsync")]
        public async Task<IActionResult> SaveMessageAsync(ChatModel chatModel)
        {
            try
            {
                await _chatService.SaveUserChat(chatModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }
    }
}
