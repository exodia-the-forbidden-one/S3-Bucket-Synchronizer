using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Uploader_UI.Helpers;
using Uploader_UI.Views;

namespace Uploader_UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (!WindowsServiceHelper.IsServiceRunning("S3UploaderService"))
            {
                try
                {
                    WindowsServiceHelper.StartService("S3UploaderService");
                    desktop.MainWindow = new MainWindow();
                }
                catch (ArgumentException)
                {
                    desktop.MainWindow = new AskDownloadDialog();
                }
            }
            else
            {
                desktop.MainWindow = new MainWindow
                {
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}