using Avalonia.Controls;
using Avalonia.Input;
using Uploader_UI.ViewModels;

namespace Uploader_UI.Views;

public partial class AddConfigurationForm : Window
{
    public AddConfigurationForm(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        SizeToContent = SizeToContent.Width;
        DataContext = new AddConfigurationFormViewModel(mainWindowViewModel, this);
    }

    public AddConfigurationForm()
    {
        
    }

    public async void ShowFolderDialogAsync(object sender, GotFocusEventArgs e)
    {
        tbTag.Focus();
        OpenFolderDialog openFolderDialog = new OpenFolderDialog
        {
            Directory = "C:\\"
        };
        var result = await openFolderDialog.ShowAsync(this);

        if (!string.IsNullOrEmpty(result))
        {
            tbFolderPath.Text = result;
        }
    }
}