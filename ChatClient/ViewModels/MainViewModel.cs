using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using ChatClient.Models;
using ChatClient.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatClient.ViewModels;

/// <summary>
/// View model for the chat window (MVVM pattern, CommunityToolkit.Mvvm).
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SignalRService _signalRService;

    /// <summary>Nickname used for the current session (not persisted as a user).</summary>
    public string Nickname { get; }

    /// <summary>Messages shown in the ListBox.</summary>
    public ObservableCollection<Message> Messages { get; } = new();

    /// <summary>Text currently typed in the message box.</summary>
    [ObservableProperty]
    private string _newMessage = string.Empty;

    public MainViewModel(string nickname, ApiService apiService, SignalRService signalRService)
    {
        Nickname = nickname;
        _apiService = apiService;
        _signalRService = signalRService;

        _signalRService.MessageReceived += OnMessageReceived;
    }

    /// <summary>
    /// Loads chat history (REST) and then connects to the SignalR hub.
    /// </summary>
    public async Task InitializeAsync()
    {
        // 1. Load previous messages via REST API
        var history = await _apiService.GetMessagesAsync();
        foreach (var message in history)
            Messages.Add(message);

        // 2. Connect to the SignalR hub for live communication
        await _signalRService.ConnectAsync();
    }

    /// <summary>
    /// Called when the server broadcasts a new message.
    /// We marshal back to the UI thread before touching the ObservableCollection.
    /// </summary>
    private void OnMessageReceived(string sender, string text, DateTime sentAt)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Messages.Add(new Message
            {
                Sender = sender,
                Text = text,
                SentAt = sentAt
            });
        });
    }

    /// <summary>
    /// Send button command (generated as "SendCommand" by CommunityToolkit.Mvvm).
    /// </summary>
    [RelayCommand]
    private async Task SendAsync()
    {
        var text = NewMessage?.Trim();

        // Message cannot be empty.
        if (string.IsNullOrWhiteSpace(text))
            return;

        await _signalRService.SendMessageAsync(Nickname, text);

        // Clear the input box (the message will arrive back via ReceiveMessage).
        NewMessage = string.Empty;
    }
}
