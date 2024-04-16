using System.Linq;
using System.Threading.Tasks;
using Uploader_UI.Helpers;
using Uploader_UI.Models;
using Uploader_UI.Views;

namespace Uploader_UI.ViewModels;

public class AddCredentialsFormWindowViewModel
{
    private readonly AddCredentialsFormWindow _window;

    public AddCredentialsFormWindowViewModel(AddCredentialsFormWindow window)
    {
        _window = window;
    }

    public AddCredentialsFormWindowViewModel()
    {
        
    }

    public async Task OnAddCredentialsCommand()
    {
        if (string.IsNullOrWhiteSpace(_window.tbTag.Text) ||
            string.IsNullOrWhiteSpace(_window.tbAccessKey.Text) ||
            string.IsNullOrWhiteSpace(_window.tbSecretKey.Text))
        {
            _window.txtError.Text = "Please fill all fields";
        }
        else
        {
            var newCredential = new AwsCredentials()
            {
                Tag = _window.tbTag.Text,
                AccessKey = _window.tbAccessKey.Text,
                SecretKey = _window.tbSecretKey.Text,
                Region = _window.cbRegion.SelectionBoxItem.ToString()
            };

            AwsHelper awsHelper = new();
            if (!await awsHelper.IsCredentialsValidAsync(newCredential.AccessKey, newCredential.SecretKey,
                    newCredential.Region))
            {
                _window.txtError.Text = "Invalid credentials";
                return;
            }

            var buckets =
                await awsHelper.GetBuckets(newCredential.AccessKey, newCredential.SecretKey, newCredential.Region);
            if (buckets.Count == 0)
            {
                _window.txtError.Text = "No buckets found";
                return;
            }

            newCredential.Buckets = buckets;
            ManageAwsCredentialsWindowViewModel.Credentials.Add(newCredential);
            
            ConfigurationHelper configurationHelper = new(@"C:\S3FileUploader\CRD");
            var json = JsonHelper.Serialize(ManageAwsCredentialsWindowViewModel.Credentials.ToList());
            configurationHelper.SaveConfigurations(json);
            
            _window.Close();
        }
    }
}