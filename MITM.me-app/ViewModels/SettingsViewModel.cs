using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MITM.me_app.SettingsService.Interfaces;

namespace MITM.me_app.ViewModels;

public partial class SettingsViewModel(ISettingsService settingsService) : ObservableObject
{
    [ObservableProperty]
    public partial string? Client1Token { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? Client1UserAgent { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? Client2Token { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string? Client2UserAgent { get; set; } = string.Empty;

    [RelayCommand]
    private void Save(string clientId)
    {
        var intClientId = int.Parse(clientId);
        if (intClientId == 1)
        {
            if (Client1Token != null) settingsService.SetTokenAsync(intClientId, Client1Token);
            if (Client1UserAgent != null) settingsService.SetUserAgentAsync(intClientId, Client1UserAgent);
        }
        else
        {
            if (Client2Token != null) settingsService.SetTokenAsync(intClientId, Client2Token);
            if (Client2UserAgent != null) settingsService.SetUserAgentAsync(intClientId, Client2UserAgent);
        }
    }
}