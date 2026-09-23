using MITM.me_app.SettingsService.Enums;

namespace MITM.me_app.SettingsService.Interfaces;

public interface ISettingsService
{
    public ChatTheme CurrentChatTheme { get; set; }

    public Gender? GetClientGender(int clientId);

    public void SetClientGender(int clientId, Gender? value);

    public Gender? GetPartnerGender(int clientId);

    public void SetPartnerGender(int clientId, Gender? value);

    public Age? GetClientAge(int clientId);

    public void SetClientAge(int clientId, Age? value);

    public Age? GetPartnerAge(int clientId);
    
    public void SetPartnerAge(int clientId, Age? value);

    public Task<string?> GetTokenAsync(int clientId);

    public Task SetTokenAsync(int clientId, string token);

    public Task<string?> GetUserAgentAsync(int clientId);

    public Task SetUserAgentAsync(int clientId, string token);
    
    
}