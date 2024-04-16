using Avalonia.Controls;
using Uploader_UI.ViewModels;

namespace Uploader_UI.Views;

public partial class AskDownloadDialog : Window
{
    public AskDownloadDialog()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.Width;
        DataContext = new AskDownloadDialogViewModel(this);
    }
}