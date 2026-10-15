using System.Text.Json;
using MITM.me_app.Models;
using MITM.me_app.Models.Enums;
using MITM.me_app.SettingsService.Enums;
using MITM.me_app.SettingsService.Interfaces;
using MITM.me_app.ViewModels;
using SocketIOClient;
using SocketIOClient.Common;

namespace MITM.me_app.SpyClient;

public sealed class Client : IDisposable
{
    private readonly SocketIO socket;

    private string? Token;
    
    private string? UserAgent;

    private readonly long currentTimestamp;
    private int userId;
    private readonly ISettingsService settingsService;
    
    public int CurrentDialogId { get; private set; }
    public int ClientId
    {
        get;
        private init => field = value > 1 ? 2 : value;
    }

    public bool IsInDialog { get; private set; }
    
    public delegate void MessageReceivedHandler(Message message);
    public event MessageReceivedHandler? MessageReceived;
    
    public delegate void DialogClosedHandler();
    public event DialogClosedHandler? DialogClosed;
        
    public delegate void DialogOpenedHandler();
    public event DialogOpenedHandler? DialogOpened;
    
    public delegate void SearchStartedHandler();
    public event SearchStartedHandler? SearchStarted;


    public async void Dispose()
    {
        if(IsInDialog)
            await LeaveDialog();
        await Disconnect();
        socket.Dispose();
    }
    
    public Client(int clientId, ISettingsService settingsService)
    {
        this.settingsService = settingsService;
        
        socket = new SocketIO(new Uri("wss://msg.nekto.me/"), new SocketIOOptions
        {
            EIO = EngineIO.V3,
            Transport = TransportProtocol.WebSocket,
            ExtraHeaders = new Dictionary<string, string>
            {
                { "User-Agent", UserAgent! },
                { "Origin", "https://nekto.me" },
            }
        });
        ClientId = clientId;
        
        socket.OnConnected += async (_, _) => {
            await AuthSendToken();
            socket.Once("notice", async ctx =>
            {
                if (ctx.RawText.Contains("notice\":\"auth.successToken"))
                {
                    
                    userId = JsonDocument.Parse(ctx.RawText).RootElement[1].GetProperty("data").GetProperty("id").GetInt32();
                
                    await AuthSendWebAgent();
                    await SetOnlineTrack();
                    await AuthSetFpt();
                    LogsViewModel.AddLogEntry("Auth succes", Level.ChatLog, clientId);
                }
                else
                {
                    LogsViewModel.AddLogEntry("Auth failed: " + ctx.RawText, Level.Error, clientId);
                    await Disconnect();
                }
            });
        };
        socket.OnDisconnected += (_, _) => {
            IsInDialog = false;
        };
        
        currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public async Task Initialize()
    {
        socket.OnAny(async (s, ctx) =>
        {
            if (s == "notice" && ctx.RawText.Contains("messages.new") && JsonDocument.Parse(ctx.RawText).RootElement[1].GetProperty("data").GetProperty("senderId").GetInt32() is var senderId&& senderId != userId)
            {
                var message = JsonDocument.Parse(ctx.RawText).RootElement[1].GetProperty("data").GetProperty("message").GetString();
                if (message != null) OnMessageReceived(new Message(message, senderId, ClientId, false));
            }
            
            if (s == "notice" && ctx.RawText.Contains("dialog.opened"))
            {
                IsInDialog = true;
                CurrentDialogId = JsonDocument.Parse(ctx.RawText).RootElement[1].GetProperty("data").GetProperty("id").GetInt32();
                var partnerId = JsonDocument.Parse(ctx.RawText).RootElement[1].GetProperty("data").GetProperty("interlocutors")[1].GetInt32();
                OnDialogOpened();
            }
            
            if (s == "notice" && ctx.RawText.Contains("search.success"))
            {
                OnSearchStarted();
            }
            
            if (s == "notice" && ctx.RawText.Contains("dialog.closed"))
            {
                IsInDialog = false;
                OnDialogClosed();
            }
        });
    }

    public async Task Connect()
    {
        Token = settingsService.GetToken(ClientId);
        UserAgent = settingsService.GetUserAgent(ClientId);
        if (settingsService.GetUserAgent(ClientId) is var ua && ua != UserAgent)
        {
            socket.Options.ExtraHeaders = new Dictionary<string, string>
            {
                { "User-Agent", ua },
                { "Origin", "https://nekto.me" },
            };
            UserAgent = ua;
        }
        await socket.ConnectAsync();
    }

    public async Task Disconnect()
    {
        await socket.DisconnectAsync();
    }

    private async Task AuthSendToken()
    {
        await socket.EmitAsync("action", [new 
        {
            action = "auth.sendToken",
            token = Token,
            isTouch = false,
            version = 14,
            t = currentTimestamp,
            timeZone = "Europe/Berlin",
            locale = "en"
        }]);
    }

    private async Task AuthSendWebAgent()
    {
        await socket.EmitAsync("action", [new 
        {
            data = CryptoUtils.GenerateWebAgent(
                Token!,
                userId, 
                currentTimestamp
            ),
            type = "web-agent"
        }]);
    }

    private async Task SetOnlineTrack()
    {
        await socket.EmitAsync("action", [new 
        {
            action = "online.track",
            on = true
        }]);
    }
    
    private async Task AuthSetFpt()
    {
        await socket.EmitAsync("action", [new 
        {
            action = "auth.setFpt",
            fpt = CryptoUtils.GenerateVisitorId(Token!),
            infoDataS = CryptoUtils.GenerateFpt(userId, CryptoUtils.GenerateVisitorId(Token!)),
            token = Token
        }]);
    }
    
    public async Task RunSearch()
    {
        object[] payload;

        payload = [new
        {
            action = "search.run"
        }];
        
        /*switch(settingsService.CurrentChatTheme)
        {
            case ChatTheme.Communication:
                payload = [new
                    {

                    }
                ];
                break;
            case ChatTheme.Filter18:
                payload = [new
                    {

                    }
                ];
                break;
            case ChatTheme.RolePlay:
                payload = [new
                    {

                    }
                ];
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }*/
        
        await socket.EmitAsync("action", payload);
    }

    public async Task LeaveDialog()
    {
        await socket.EmitAsync("action", [new 
        {
            action = "anon.leaveDialog",
            dialogId = CurrentDialogId
        }]);
        IsInDialog = false;
    }
    
    public async Task SendMessage(string message)
    {
        await socket.EmitAsync("action", [new 
        {
            action = "anon.message",
            dialogId = CurrentDialogId,
            fileId = (object)null!,
            message = message,
            randomId = CryptoUtils.GenerateRandomId()

        }]);
    }

    private void OnMessageReceived(Message message)
    {
        MessageReceived!(message);
    }

    private void OnDialogClosed()
    {
        DialogClosed?.Invoke();
    }

    private void OnDialogOpened()
    {
        DialogOpened?.Invoke();
    }

    private void OnSearchStarted()
    {
        SearchStarted?.Invoke();
    }
}