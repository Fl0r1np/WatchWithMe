namespace WatchWithMeAPI.Model;

public class TextMessage : Message
{
    public string Content { get; set; } = null!;
    
    public override void markAsRead()
    {
        throw new NotImplementedException();
    }

    public void Edit(string newContent)
    {   
        throw new NotImplementedException();
    }
}