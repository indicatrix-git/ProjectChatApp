using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewModels;

namespace ChatClient.Views;

public partial class ChatWindow : Window
{
    private readonly MainViewModel _viewModel;

    public ChatWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        // Auto-scroll to the newest message whenever the list changes.
        _viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
    }

    private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (MessagesList.Items.Count > 0)
            MessagesList.ScrollIntoView(MessagesList.Items[^1]);
    }

    private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        // Press Enter to send.
        if (e.Key == Key.Enter && _viewModel.SendCommand.CanExecute(null))
        {
            _viewModel.SendCommand.Execute(null);
            e.Handled = true;
        }
    }
}
