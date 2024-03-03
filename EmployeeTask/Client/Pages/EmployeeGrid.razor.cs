using Microsoft.JSInterop;
using static MudBlazor.Colors;

namespace EmployeeTask.Client.Pages
{
    public class EmployeeGridBase: ComponentBase
    {
        public static EmployeeGridBase Instance { get; private set; }
        public EmployeeGridBase()
        {
            Instance = this;
        }
        public string placeholder = "Users disconnected";
        public string text = "";
        public List<string> messageList = new List<string>();

        [JSInvokable("HandleOnlineEvent")]
        public static async Task HandleOnlineEvent()
        {
            await Instance.Hello();
        }

        public async Task Hello()
        {
            await HubConnection.SendAsync("UserConnected");
        }
        public async Task SendMessage(string message)
        {
            text = string.Empty;
            messageList.Add(message);
            await HubConnection.SendAsync("RefreshMessages", message);
        }
        public void RefreshMessages(List<string> message)
        {
            messageList.AddRange(message);
        }
        public void OnDisConnected()
        {
            placeholder = "User disconnected";
        }
        public void OnConnected()
        {
            placeholder = "";
        }
        #region Services
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public HttpClient _httpClient { get; set; }
        [Inject] public IDialogService _dialogService { get; set; }
        #endregion

        #region Parameters
        [CascadingParameter] public HubConnection? HubConnection { get; set; }
        #endregion

        #region Private Fields
        public MudTable<RegisterModel> table { get; set; } = new MudTable<RegisterModel>();
        public string Id { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (HubConnection != null && HubConnection.State == HubConnectionState.Connected)
            {
               
                HubConnection.On("RefreshEmployees", () =>
                {
                    table.ReloadServerData();
                });
                HubConnection.On<List<string>>("RefreshMessagess", (message) =>
                {
                     RefreshMessages(message);
                    StateHasChanged();
                });
                HubConnection.Closed += async (s) =>
                {
                    OnDisConnected();
                };
                HubConnection.On("UserDisConnectedd", () =>
                {
                    OnDisConnected();
                    StateHasChanged();
                });
                HubConnection.On<string>("UserConnectedd", (user) =>
                {
                    OnConnected();
                    StateHasChanged();
                });
               // await HubConnection.SendAsync("UserConnected");
            }
        }

        public async Task<TableData<RegisterModel>> GetEmployees(TableState tableState)
        {
            var result = await _httpClient.GetFromJsonAsync<List<RegisterModel>>($"{ApplicationRoutes.Url}Employee/GetEmployees");
            return new TableData<RegisterModel> { Items = result };
        }

        public async Task OpenEditEmployeeDialog(RegisterModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("Register", model);
            parameters.Add("HubConnection", HubConnection);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<CreateEmployee>("Modal", parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await table.ReloadServerData();
            }
        }

        public async Task DeleteEmployee(string id)
        {
            var result = await _httpClient.DeleteAsync($"{ApplicationRoutes.Url}Employee/DeleteEmployee/{id}");
            var response = await result.Content.ReadFromJsonAsync<Response>();
            if (response != null && response.Status == "Success")
            {
                await table.ReloadServerData();
                HubConnection?.InvokeAsync("RefreshEmployees","Hello");
            }
        }

        public async Task OpenTaskDialog(string id)
        {
            var parameters = new DialogParameters();
            parameters.Add("Id", id);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            _dialogService.Show<CreateTask>("Modal", parameters, options);
        }

        public async Task NavigatToTaskGrid(string id)
        {
            var result = await _httpClient.GetAsync($"{ApplicationRoutes.Url}Employee/GetEmployee/{id}");
            var employee = await result.Content.ReadFromJsonAsync<Employee>();
            if (employee == null)
            {
                return;
            }
            NavigationManager.NavigateTo($"/TaskGrid/{employee.Id}");
        }
    }
}
