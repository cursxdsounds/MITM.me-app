using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MITM.me_app.Models;
using MITM.me_app.Models.Enums;
using MITM.me_app.SpyClient;

namespace MITM.me_app.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    public ObservableCollection<Message> Messages { get; } = [];

    private Client client1;
    private Client client2;

    [ObservableProperty] private bool canLeaveDialog;

    [ObservableProperty] private bool isSearching;
    
    [ObservableProperty] private bool isConnected;
    
    [ObservableProperty] private bool isConnecting;
    
    [ObservableProperty] private bool isDisconnecting;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string inputText = string.Empty;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    private string inputText2 = string.Empty;

    public ChatViewModel()
    {
        client1 = new Client(1, new SettingsService.SettingsService());
        client2 = new Client(2, new SettingsService.SettingsService());
    }

    public async Task Initialize()
    {
        await client1.Initialize();
        await client2.Initialize();
        
        WireUpClients(client1, client2);
        WireUpClients(client2, client1);
    }

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task Send(int clientId)
    {
        var text = clientId == 2 ? InputText2 : InputText;
        
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (client1.IsInDialog && client2.IsInDialog)
        {
            await GetPartnerClientById(clientId).SendMessage(text);
        }
        
        Messages.Add(new Message(text, 0, clientId, true));
        
        if (clientId == 2) 
        {
            InputText2 = string.Empty;
        } 
        else 
        {
            InputText = string.Empty;
        }
    }
    
    [RelayCommand]
    private async Task Search()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            CanLeaveDialog = false;
            IsSearching = true;
        });

        try
        {
            await Task.WhenAll(client1.RunSearch(), client2.RunSearch());
        }
        catch (Exception ex)
        {
            LogsViewModel.AddLogEntry($"Search error: {ex.Message}", Level.Error, 0);
            MainThread.BeginInvokeOnMainThread(() => IsSearching = false);
        }
    } 

    [RelayCommand]
    private async Task LeaveDialog()
    {
        try
        {
            await Task.WhenAll(client1.LeaveDialog(), client2.LeaveDialog());
        }
        catch (Exception ex)
        {
            LogsViewModel.AddLogEntry($"Leave error: {ex.Message}", Level.Error, 0);
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CanLeaveDialog = false;
                IsSearching = false;
            });
        }
    }

    [RelayCommand]
    private async Task Disconnect()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsDisconnecting = true;
            });
            await Task.WhenAll(client1.Disconnect(), client2.Disconnect());
        }
        catch (Exception ex)
        {
            LogsViewModel.AddLogEntry($"DisconnectError: {ex.Message}", Level.Error, 0);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsDisconnecting = false;
            });
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsDisconnecting = false;
                IsConnected = false;
            });
        }
    }
    
    [RelayCommand]
    private async Task Connect()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsConnecting = true;
            });
            await Task.WhenAll(client1.Connect(), client2.Connect());
        }
        catch (Exception ex)
        {
            LogsViewModel.AddLogEntry($"ConnectError: {ex.Message}", Level.Error, 0);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsConnecting = false;
            });
        }
        finally
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsConnecting = false;
                IsConnected = true;
            });
        }
    }
    

    private Client GetPartnerClientById(int clientId)
    {
        return clientId == client1.ClientId ? client2 : client1;
    }
    
    private void WireUpClients(Client source, Client target)
    {
        source.MessageReceived += async message =>
        {
            try
            {
                await target.SendMessage(message.Text);
            }
            catch (Exception ex)
            {
                LogsViewModel.AddLogEntry($"Send error: {ex.Message}", Level.Error, source.ClientId);
            }
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(message);
            });
        };

        source.DialogClosed += async () =>
        {
            try
            {
                if (target.IsInDialog) 
                    await target.LeaveDialog();
                
                LogsViewModel.AddLogEntry("dialog closed", Level.ChatLog, source.ClientId);
            }
            catch (Exception ex)
            {
                LogsViewModel.AddLogEntry($"Close error: {ex.Message}", Level.Error, source.ClientId);
            }
            finally
            {
                if (!target.IsInDialog && !source.IsInDialog)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CanLeaveDialog = false;
                        IsSearching = false;
                    });
                }
            }
        };

        source.DialogOpened += async () =>
        {
            try
            {
                LogsViewModel.AddLogEntry("partner found", Level.ChatLog, source.ClientId);
                
                if (source.CurrentDialogId == target.CurrentDialogId)
                {
                    await target.LeaveDialog();
                    LogsViewModel.AddLogEntry("Clients found each other! Leaving...", Level.Warning, source.ClientId);
                }

                if (target.IsInDialog && source.IsInDialog)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CanLeaveDialog = true;
                        IsSearching = false;
                    });
                }
            }
            catch (Exception ex)
            {
                LogsViewModel.AddLogEntry($"Open error: {ex.Message}", Level.Error, source.ClientId);
            }
        };
        
        source.SearchStarted += () =>
        {
            LogsViewModel.AddLogEntry("search started", Level.ChatLog, source.ClientId);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsSearching = true;
            });
        };
    }

    public void DisposeClients()
    {
        client1?.Dispose();
        client2?.Dispose();
    }
    
    private bool CanSend() => !string.IsNullOrWhiteSpace(InputText) || !string.IsNullOrWhiteSpace(InputText2);
}