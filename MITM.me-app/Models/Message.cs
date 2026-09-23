namespace MITM.me_app.Models;

public class Message(string text, int senderId, int senderClientId, bool isUserMessage)
{
    public string Text { get; set; } = text;
    public int SenderId { get; set; } = senderId;
    public int SenderClientId { get; set; } = senderClientId;
    public bool IsUserMessage { get; set; } = isUserMessage;
}