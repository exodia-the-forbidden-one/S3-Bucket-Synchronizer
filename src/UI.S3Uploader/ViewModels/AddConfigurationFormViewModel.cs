using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Uploader_UI.Helpers;
using Uploader_UI.Models;
using Uploader_UI.Views;

namespace Uploader_UI.ViewModels
{
    public class AddConfigurationFormViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public Dictionary<string, List<AwsCredentials>> GroupedByBuckets { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly AddConfigurationForm _addConfigurationForm;

        public AddConfigurationFormViewModel()
        {
            
        }

        public AddConfigurationFormViewModel(MainWindowViewModel mainWindowViewModel,
            AddConfigurationForm addConfigurationForm)
        {
            GroupedByBuckets = new();
            _mainWindowViewModel = mainWindowViewModel;
            _addConfigurationForm = addConfigurationForm;

            var credentialsList = new List<AwsCredentials>();
            ConfigurationHelper configurationHelper = new ConfigurationHelper(@"C:\S3FileUploader\CRD");
            var listOfCredentials = configurationHelper.GetConfigurations<List<AwsCredentials>>();

            if (listOfCredentials != null)
                foreach (var cfg in listOfCredentials)
                {
                    if (cfg != null)
                        credentialsList.Add(cfg);
                }

            GroupedByBuckets = credentialsList.SelectMany(credentials => credentials.Buckets
                    .Select(bucket => new { Bucket = bucket, Credentials = credentials }))
                .GroupBy(x => x.Bucket)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Credentials).ToList());

            var bucketNames = GroupedByBuckets.Keys.ToList();

            addConfigurationForm.cbRegion.ItemsSource = bucketNames;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void AddConfigurationCommand()
        {
            string? selectedBucketName = _addConfigurationForm.cbRegion.SelectionBoxItem?.ToString();
            if (string.IsNullOrEmpty(_addConfigurationForm.tbTag.Text) ||
                string.IsNullOrEmpty(_addConfigurationForm.tbFolderPath.Text) ||
                string.IsNullOrEmpty(selectedBucketName))
            {
                return;
            }
            else
            {
                AwsSettings awsSettings = new AwsSettings()
                {
                    AccessKey = GroupedByBuckets[selectedBucketName].First().AccessKey,
                    BucketName = selectedBucketName,
                    Region = GroupedByBuckets[selectedBucketName].First().Region,
                    SecretKey = GroupedByBuckets[selectedBucketName].First().SecretKey,
                    Tag = "0"
                };

                AwsHelper awsHelper = new();
                var isCredentialsValid = await awsHelper.IsCredentialsValidAsync(awsSettings.AccessKey,
                    awsSettings.SecretKey, awsSettings.Region);

                if (isCredentialsValid)
                {
                    MainWindowViewModel.Configurations.Add(
                        new Configuration
                        {
                            Tag = _addConfigurationForm.tbTag.Text,
                            AwsSettings = awsSettings,
                            Id = Guid.NewGuid(),
                            FolderPath = _addConfigurationForm.tbFolderPath.Text
                        });

                    _addConfigurationForm.Close();
                    _mainWindowViewModel.IsButtonEnabled = true;
                }
                else
                {
                    _addConfigurationForm.tbAddConfigurationFormError.Text = "Invalid credentials";
                    _addConfigurationForm.tbAddConfigurationFormError.Foreground = Brushes.Red;
                }
            }
        }
    }
}