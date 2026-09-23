using System.Collections.ObjectModel;
using MITM.me_app.Models;

namespace MITM.me_app.ViewModels;

public static class LogsViewModel
{
    public static ObservableCollection<LogEntry> Logs { get; } = [];

    public static void AddLogEntry(string message, Models.Enums.Level level, int clientId)
    {
        MainThread.BeginInvokeOnMainThread( () =>
        {
            var strLevel = level switch
            {
                Models.Enums.Level.Error => "ERROR",
                Models.Enums.Level.Warning => "WARNING",
                Models.Enums.Level.ChatLog => "CHATLOG",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };

            Logs.Add(new LogEntry(DateTimeOffset.Now.DateTime, message, strLevel, clientId));
        });
        
    }
}