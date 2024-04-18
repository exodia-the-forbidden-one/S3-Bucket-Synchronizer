using Avalonia.Controls;
using Uploader_UI.ViewModels;

namespace Uploader_UI.Views;

public partial class InstallServiceWindow : Window
{
    public InstallServiceWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.Width;
        DataContext = new InstallServiceWindowViewModel(this);
    }
}