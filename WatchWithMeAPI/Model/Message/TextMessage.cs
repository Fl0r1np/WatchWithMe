namespace WatchWithMeAPI.Model;

public class TextMessage : Message
{
    public string? Content { get; set; }
    
    public override void markAsRead()
    {
        throw new NotImplementedException();
    }

    public void Edit(string newContent)
    {   
        throw new NotImplementedException();
    }
}