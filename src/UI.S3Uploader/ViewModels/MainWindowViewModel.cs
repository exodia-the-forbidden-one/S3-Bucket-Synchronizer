using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Uploader_UI.Helpers;
using Uploader_UI.Views;
using Configuration = Uploader_UI.Models.Configuration;

namespace Uploader_UI.ViewModels;

public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public static ObservableCollection<Configuration> Configurations { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly Window _window;

    public MainWindowViewModel(Window window)
    {
        _window = window;

        Configurations = new();
        ConfigurationHelper configurationHelper = new(@"C:\S3FileUploader\CFG");
        var cfgs = configurationHelper.GetConfigurations<List<Configuration>>();
        foreach (var cfg in cfgs)
        {
            Configurations.Add(cfg);
        }
    }

    public MainWindowViewModel()
    {
        
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool _isButtonEnabled;

    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set
        {
            _isButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    public void SaveChangesCommand()
    {
        ConfigurationHelper configurationHelper = new(@"C:\S3FileUploader\CFG");
        configurationHelper.SaveConfigurations(JsonHelper.Serialize(Configurations));
        WindowsServiceHelper.RestartService("Service1");
        IsButtonEnabled = false;
    }

    public void OnAddConfigurationButtonClicked()
    {
        Window dialog = new AddConfigurationForm(this);
        dialog.ShowDialog(_window);
    }
    
    public void OnManageCredentialsButtonClicked()
    {
        Window dialog = new ManageAwsCredentialsWindow();
        dialog.ShowDialog(_window);
    }

    public void OnDeleteRow()
    {
        try
        {
            var x = _window.FindControl<DataGrid>("dgConfigurations").SelectedIndex;
            Configurations.RemoveAt(x);

            IsButtonEnabled = true;
        }
        catch
        {
        }
    }
}