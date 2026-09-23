namespace MITM.me_app.Models;

public class LogEntry(DateTime timeStamp, string message,string level, int clientId)
{
    public DateTime Timestamp { get; set; } = timeStamp;
    public string Message { get; set; } = message;
    public string Level { get; set; } = level;
    public int ClientId { get; set; } = clientId;
}