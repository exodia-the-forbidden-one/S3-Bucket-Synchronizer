using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Uploader_UI.Helpers;
using Uploader_UI.Models;
using Uploader_UI.Views;

namespace Uploader_UI.ViewModels;

public class ManageAwsCredentialsWindowViewModel
{
    private readonly ManageAwsCredentialsWindow _window;
    public static ObservableCollection<AwsCredentials>? Credentials { get; set; }

    public ManageAwsCredentialsWindowViewModel()
    {
        
    }
    public ManageAwsCredentialsWindowViewModel(ManageAwsCredentialsWindow window)
    {
        Credentials = new ObservableCollection<AwsCredentials>();
        _window = window;
        ConfigurationHelper configurationHelper = new ConfigurationHelper(@"C:\S3FileUploader\CRD");
        var listOfCredentials = configurationHelper.GetConfigurations<List<AwsCredentials>>();

        if (listOfCredentials != null)
            foreach (var cfg in listOfCredentials)
            {
                if (cfg != null)
                    Credentials.Add(cfg);
            }
    }

    public void OnDeleteRow()
    {
        var x = _window.FindControl<DataGrid>("DgCredentials").SelectedIndex;
        Credentials.RemoveAt(x);

        ConfigurationHelper configurationHelper = new(@"C:\S3FileUploader\CRD");
        var json = JsonHelper.Serialize(Credentials.ToList());
        configurationHelper.SaveConfigurations(json);
    }

    public void OnAddNewCredentialButtonClicked()
    {
        Window dialog = new AddCredentialsFormWindow();
        dialog.ShowDialog(_window);
    }
}