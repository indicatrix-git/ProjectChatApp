using System;
using System.Windows;
using ChatClient.Services;
using ChatClient.ViewModels;
using ChatClient.Views;

namespace ChatClient;

public partial class App : Application
{
    // Backend address. Change here if your server runs on another machine/port.
    private const string BaseUrl = "http://localhost:5000";
    private const string HubUrl = "http://localhost:5000/chathub";

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        // Keep the app alive while we juggle windows manually.
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // 1. Ask for a nickname.
        var nicknameWindow = new NicknameWindow();
        var result = nicknameWindow.ShowDialog();

        if (result != true)
        {
            Shutdown();
            return;
        }

        // 2. Build services and the view model.
        var apiService = new ApiService(BaseUrl);
        var signalRService = new SignalRService(HubUrl);
        var mainViewModel = new MainViewModel(nicknameWindow.Nickname, apiService, signalRService);

        // 3. Show the chat window.
        var chatWindow = new ChatWindow(mainViewModel);
        MainWindow = chatWindow;
        ShutdownMode = ShutdownMode.OnLastWindowClose;
        chatWindow.Show();

        // 4. Load history + connect to SignalR.
        try
        {
            await mainViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not connect to the server.\n\nIs the backend (ChatApi) running on {BaseUrl}?\n\n{ex.Message}",
                "Connection error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
