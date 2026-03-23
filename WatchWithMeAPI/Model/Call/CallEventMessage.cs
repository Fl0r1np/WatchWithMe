namespace WatchWithMeAPI.Model;

public class CallEventMessage : Message
{
    public int? CallDurationSeconds { get; set; }

    public CallEventType? CallEventType { get; set; }
    
    public override void markAsRead()
    {
        throw new NotImplementedException();
    }
}