using Avalonia.Controls;
using Uploader_UI.ViewModels;

namespace Uploader_UI.Views;

public partial class ManageAwsCredentialsWindow : Window
{
    public ManageAwsCredentialsWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.Width;
        DataContext = new ManageAwsCredentialsWindowViewModel(this);
    }
}