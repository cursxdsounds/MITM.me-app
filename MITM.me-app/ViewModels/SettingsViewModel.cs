using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace MITM.me_app.ViewModels;

public partial class SettingsViewModel: ObservableObject
{
    private readonly SettingsService.SettingsService settingsService;
    
    [ObservableProperty] 
    private bool isClient1Selected;
    [ObservableProperty] 
    private bool isClient2Selected;
    [ObservableProperty] 
    private string? client1Token;
    [ObservableProperty] 
    private string? client2Token;
    [ObservableProperty] 
    private string? client1UserAgent;
    [ObservableProperty] 
    private string? client2UserAgent;

    public SettingsViewModel(SettingsService.SettingsService settingsService)
    {
        this.settingsService = settingsService;
        Load();
    }
    
    [RelayCommand]
    private void Save(int clientId)
    {
        switch (clientId)
        {
            case 1:
                if(Client1Token != null) settingsService.SetToken(clientId, Client1Token);
                if(Client1UserAgent != null) settingsService.SetUserAgent(clientId, Client1UserAgent);
                break;
            case 2:
                if(Client2Token != null) settingsService.SetToken(clientId, Client2Token);
                if(Client2UserAgent != null) settingsService.SetUserAgent(clientId, Client2UserAgent);
                break;
        }
    }
    
    private void Load()
    {
        Client1Token = settingsService.GetToken(1);
        Client1UserAgent = settingsService.GetUserAgent(1);
        Client2Token = settingsService.GetToken(2);
        Client2UserAgent = settingsService.GetUserAgent(2);
    }

    [RelayCommand]
    private void SelectClient(int clientId)
    {
        switch (clientId)
        {
            case 1:
                IsClient1Selected = true;
                IsClient2Selected = false;
                break;
            case 2:
                IsClient1Selected = false;
                IsClient2Selected = true;
                break;
        }
    }
}