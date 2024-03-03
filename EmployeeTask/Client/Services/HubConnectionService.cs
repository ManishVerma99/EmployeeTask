namespace EmployeeTask.Client.Services
{
    public class HubConnectionService
    {
        private readonly HubConnection _hubConnection;
        private readonly NavigationManager _navigationManager;
        public HubConnectionService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/signalRHub")).WithAutomaticReconnect().Build();

            _ = InitializeConnection();
        }

        private async Task InitializeConnection()
        {
            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("Hub connection started");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting hub connection: {ex.Message}");
            }
        }

        public HubConnection GetHubConnection()
        {
            return _hubConnection;
        }
    }
}
