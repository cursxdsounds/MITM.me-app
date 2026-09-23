using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MITM.me_app.SettingsService.Interfaces;

namespace MITM.me_app.ViewModels;

public partial class SettingsViewModel(ISettingsService settingsService) : ObservableObject
{
    [ObservableProperty] private string? client1Token = string.Empty;
    [ObservableProperty] private string? client1UserAgent = string.Empty;
    [ObservableProperty] private string? client2Token = string.Empty;
    [ObservableProperty] private string? client2UserAgent = string.Empty;

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