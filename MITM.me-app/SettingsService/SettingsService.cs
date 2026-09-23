using MITM.me_app.SettingsService.Enums;
using MITM.me_app.SettingsService.Interfaces;

namespace MITM.me_app.SettingsService;

public class SettingsService : ISettingsService
{
    private const string ChatThemeKey = "chat_theme";

    public ChatTheme CurrentChatTheme
    {
        get => (ChatTheme)Preferences.Get(ChatThemeKey, (int)ChatTheme.Communication);
        set => Preferences.Set(ChatThemeKey, (int)value);
    }

    private static string ClientGenderKey(int id) => $"client_gender_{id}";
    private static string PartnerGenderKey(int id) => $"partner_gender_{id}";
    private static string ClientAgeKey(int id) => $"client_age_{id}";
    private static string PartnerAgeKey(int id) => $"partner_age_{id}";

    public Gender? GetClientGender(int clientId)
        => (Gender?)Preferences.Get(ClientGenderKey(clientId), (int)Gender.Unspecified);

    public void SetClientGender(int clientId, Gender? value) 
        => Preferences.Set(ClientGenderKey(clientId), (int)(value ?? Gender.Unspecified));

    public Gender? GetPartnerGender(int clientId)
        => (Gender?)Preferences.Get(PartnerGenderKey(clientId), (int)Gender.Unspecified);

    public void SetPartnerGender(int clientId, Gender? value)
        => Preferences.Set(PartnerGenderKey(clientId), (int)(value ?? Gender.Unspecified));

    public Age? GetClientAge(int clientId)
        => (Age?)Preferences.Get(ClientAgeKey(clientId), (int)Age.Unspecified);

    public void SetClientAge(int clientId, Age? value)
        => Preferences.Set(ClientAgeKey(clientId), (int)(value ?? Age.Unspecified));

    public Age? GetPartnerAge(int clientId)
        => (Age?)Preferences.Get(PartnerAgeKey(clientId), (int)Age.Unspecified);

    public void SetPartnerAge(int clientId, Age? value)
        => Preferences.Set(PartnerAgeKey(clientId), (int)(value ?? Age.Unspecified));
    
    private static string UserAgentKey(int id) => $"ua_{id}";
    private static string TokenKey(int id) => $"token_{id}";
    
    public async Task<string?> GetTokenAsync(int clientId)
    {
        return await SecureStorage.GetAsync(TokenKey(clientId));
    }

    public async Task SetTokenAsync(int clientId, string token)
    {
        await SecureStorage.SetAsync(TokenKey(clientId), token);
    }
    
    public async Task<string?> GetUserAgentAsync(int clientId)
    {
        return await SecureStorage.GetAsync(UserAgentKey(clientId));
    }

    public async Task SetUserAgentAsync(int clientId, string token)
    {
        await SecureStorage.SetAsync(UserAgentKey(clientId), token);
    }
}