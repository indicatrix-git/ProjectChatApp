using System.Windows;
using System.Windows.Input;

namespace ChatClient.Views;

public partial class NicknameWindow : Window
{
    /// <summary>The nickname entered by the user (valid only when DialogResult == true).</summary>
    public string Nickname { get; private set; } = string.Empty;

    public NicknameWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => NicknameTextBox.Focus();
    }

    private void JoinButton_Click(object sender, RoutedEventArgs e) => Join();

    private void NicknameTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            Join();
    }

    private void Join()
    {
        var name = NicknameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Please enter a nickname.", "Chat",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Nickname = name;
        DialogResult = true; // closes the dialog and returns true
    }
}
