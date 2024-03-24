using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Microsoft.Win32;
using System.Net.Http;
using System.Net.Http.Json;

namespace EmployeeTask.Client.Components
{
    public class UserChatBase : ComponentBase
    {
        #region Services
        [Inject] public HttpClient _httpClient { get; set; }
        [Inject] public IJSRuntime _JsRuntime { get; set; }
        [Inject] public ApiAuthenticationStateProvider _authenticationStateProvider { get; set; }
        #endregion

        #region Parameters
        [Parameter] public HubConnection HubConnection { get; set; }
        #endregion

        #region List
        public List<ApplicationUser> applicationUsers { get; set; } = new();
        public List<ChatModel> ChatList { get; set; } = new();
        #endregion

        #region Public Fields
        public ApplicationUser SelectedUser { get; set; } = new();
        public string CurrentMessage { get; set; }
        public bool ShowTyping { get; set; }
        public string SearchUser { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentUserEmail { get; set; }
        public bool showTypingIndicator = false;
        public bool ShouldScroll { get; set; }
        public string ShowTypingIndicatorToUserId { get; set; }
        public Timer typingTimer;
        public List<ConnectedUser> ConnectedUsers { get; set; } = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            var user = await _authenticationStateProvider.GetAuthenticationStateAsync();
            CurrentUserId = user.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            CurrentUserEmail = user.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value!;
            var result = await _httpClient.GetFromJsonAsync<List<ApplicationUser>>($"{ApplicationRoutes.Url}Employee/GetUsers");
            applicationUsers = result ?? new List<ApplicationUser>();
            ConnectedUsers = await HubConnection.InvokeAsync<List<ConnectedUser>>("GetConnectedUsers");
            HubConnection.On<ChatModel>("RecieveMessage", (message) =>
            {
                if ((SelectedUser.Id == message.ToUserId && CurrentUserId == message.FromUserId) || (SelectedUser.Id == message.FromUserId && CurrentUserId == message.ToUserId))
                {
                    if (SelectedUser.Id == message.ToUserId && CurrentUserId == message.FromUserId)
                    {
                        ChatList.Add(new ChatModel { Message = message.Message, CreatedDate = message.CreatedDate, FromUserId = CurrentUserId, ToUserId = SelectedUser.Id });
                    }
                    else if ((SelectedUser.Id == message.FromUserId && CurrentUserId == message.ToUserId))
                    {
                        ChatList.Add(new ChatModel { Message = message.Message, CreatedDate = message.CreatedDate, FromUserId = SelectedUser.Id });
                    }
                    ShouldScroll = true;
                    StateHasChanged();
                }
            });
            HubConnection.On<string>("OnMessageInput", (showTypingIndicatorToUser) =>
            {
                if (!showTypingIndicator)
                {
                    showTypingIndicator = true;
                    ShowTypingIndicatorToUserId = showTypingIndicatorToUser;
                    StateHasChanged();
                }

                ResetTypingTimer();
            });

            HubConnection.On("NotifyUser",  async () =>
            {
                ConnectedUsers = await HubConnection.InvokeAsync<List<ConnectedUser>>("GetConnectedUsers");
                StateHasChanged();
            });

        }

        public async void ScrollToBottom()
        {
            await _JsRuntime.InvokeVoidAsync("scrollToBottom");
            ShouldScroll = false;
        }

        public async Task LoadUserChat()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChatModel>>($"{ApplicationRoutes.Url}Chat/GetConversationAsync/{SelectedUser.Id}");
            ChatList = result;
            ShouldScroll = true;
        }

        public async Task OnUserClick(ApplicationUser user)
        {
            SelectedUser = user;
            await LoadUserChat();
        }

        public async Task OnMessageInput(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                await SendMessage();
            }
            else
            {
                await HubConnection.SendAsync("MessageInput", SelectedUser.Email);
            }
        }

        public async Task SendMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(SelectedUser.Id))
                {
                    var chatHistory = new ChatModel()
                    {
                        Message = CurrentMessage,
                        ToUserId = SelectedUser.Id,
                        CreatedDate = DateTime.Now
                    };
                    chatHistory.FromUserId = CurrentUserId;
                    var result = await _httpClient.PostAsJsonAsync($"{ApplicationRoutes.Url}Chat/SaveMessageAsync", chatHistory);
                    ChatList.Add(chatHistory);
                    await HubConnection.SendAsync("SendMessage", chatHistory, SelectedUser.Email);
                    CurrentMessage = string.Empty;
                    ResetTypingTimer();
                    ShouldScroll = true;
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (ShouldScroll)
            {
               ScrollToBottom();
            }
        }

        private void ResetTypingTimer()
        {
            typingTimer?.Dispose();
            typingTimer = new Timer(_ =>
            {
                showTypingIndicator = false;
                StateHasChanged();
            }, null, 2000, Timeout.Infinite);
        }

        public async ValueTask DisposeAsync()
        {
            
        }
    }
}
