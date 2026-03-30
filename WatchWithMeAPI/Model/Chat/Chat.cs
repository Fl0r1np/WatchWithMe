namespace WatchWithMeAPI.Model;

public class Chat
{
    public int? Id { get; set; }

    public int? RoomId { get; set; }
    
    public Room? Room { get; set; }
    
    public List<Message>? ListOfMessages { get; set; }
    
    public List<User>? ListOfUserWithMessages { get; set; }
    
    
}