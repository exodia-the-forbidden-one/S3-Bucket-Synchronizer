using Avalonia.Controls;
using Avalonia.Media;
using Uploader_UI.Helpers;
using Uploader_UI.ViewModels;

namespace Uploader_UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Height = 430;
        SizeToContent = SizeToContent.Width;
        DataContext = new MainWindowViewModel(this);

        if (WindowsServiceHelper.IsServiceRunning("S3UploaderService"))
        {
            BtnServiceStatus.Content = "Service Running";
            BtnServiceStatus.Background = SolidColorBrush.Parse("#00ff50");
            BtnServiceStatus.Foreground = SolidColorBrush.Parse("#006400");
        }
    }
}