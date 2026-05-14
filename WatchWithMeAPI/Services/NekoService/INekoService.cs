namespace WatchWithMeAPI.Services.NekoService;

public class NekoContainerResult
{
    public string ContainerId { get; set; } = null!;
    public string ViewerPassword { get; set; } = null!;
    public string HostPassword { get; set; } = null!;
    public int MappedPort { get; set; }

}

public interface INekoService
{
    Task<NekoContainerResult> CreateAndStartNekoContainerAsync(string shareCode);
    Task StopAndRemoveContainerAsync(string containerId);
}