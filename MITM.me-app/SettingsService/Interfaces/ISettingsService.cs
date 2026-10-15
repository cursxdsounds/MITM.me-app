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
    public string? GetToken(int clientId);

    public void SetToken(int clientId, string value);

    public string? GetUserAgent(int clientId);

    public void SetUserAgent(int clientId, string value);
    
    
}