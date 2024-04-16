using System;
using System.IO;
using System.Net;
using Uploader_UI.Views;
using System.IO.Compression;
using System.Diagnostics;
using Uploader_UI.Helpers;

namespace Uploader_UI.ViewModels;

public class AskDownloadDialogViewModel : ViewModelBase
{
    private readonly AskDownloadDialog _askDownloadDialog;
    private readonly string _zipPath;
    private readonly string _extractPath;

    public AskDownloadDialogViewModel(AskDownloadDialog askDownloadDialog)
    {
        _askDownloadDialog = askDownloadDialog;
        _zipPath = Path.Combine(Directory.GetCurrentDirectory(), "WinService.zip");
        _extractPath = Path.Combine(Directory.GetCurrentDirectory(), "WinService");
        OnDownloadCommand();
    }

    public void OnDownloadCommand()
    {
        using WebClient client = new();

        client.DownloadProgressChanged += (sender, e) =>
        {
            _askDownloadDialog.PbInstallService.Value = e.ProgressPercentage;
        };

        client.DownloadFileCompleted += (sender, e) =>
        {
            Directory.CreateDirectory(_extractPath);
            ZipFile.ExtractToDirectory(_zipPath, _extractPath,true);
            File.Delete(_zipPath);

            string installCommand = $"sc create S3UploaderService binPath= \"{Path.Combine(_extractPath, "S3Uploader.exe")}\" start= auto";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {installCommand}",
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas",
                RedirectStandardOutput = true
            };

            Process process = new Process { StartInfo = psi };

            process.Start();

            process.WaitForExit();
            process.Close();

            WindowsServiceHelper.StartService("S3UploaderService");
            MainWindow mainWindow = new();
            mainWindow.Show();
            _askDownloadDialog.Close();
        };

        client.DownloadFileAsync(
            new Uri("https://github.com/exodia-the-forbidden-one/S3-Uploader/releases/download/WinService/WindowsService.zip"), _zipPath);
    }
}