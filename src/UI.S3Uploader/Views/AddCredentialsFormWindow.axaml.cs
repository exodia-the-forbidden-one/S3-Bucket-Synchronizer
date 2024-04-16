using Avalonia.Controls;
using Uploader_UI.ViewModels;

namespace Uploader_UI.Views;

public partial class AddCredentialsFormWindow : Window
{
    public AddCredentialsFormWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.Height;
        DataContext = new AddCredentialsFormWindowViewModel(this);
    }
}