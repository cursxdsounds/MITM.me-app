using MITM.me_app.ViewModels;

namespace MITM.me_app;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        BindingContext = new SettingsViewModel(new SettingsService.SettingsService());
    }
}