using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using WatchWithMeAPI.Utils;

namespace WatchWithMeAPI.Services.NekoService;

public class NekoService : INekoService
{
    
    private readonly DockerClient _dockerClient;
    private readonly ILogger<NekoService> _logger;

    public NekoService(ILogger<NekoService> logger)
    {
        _logger = logger;
        
        // Connect to the Docker Daemon safety based on the OS
        var dockerUri = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new Uri("npipe://./pipe/docker_engine") // Windows Docker Desktop
            : new Uri("unix:///var/run/docker.sock"); // Linux Servers
        
        _dockerClient = new DockerClientConfiguration(dockerUri).CreateClient();
        
    }
    
    /// <summary>
    /// Method that creates and start the n.eko container
    /// </summary>
    /// <param name="shareCode">
    /// A string containing the room's share code
    /// </param>
    /// <returns>
    /// Returns an object containing the necessary data for Front-End to handle the room's virtual browser
    /// </returns>
    /// <exception cref="Exception">
    /// Throws an exception if the n.eko container fails to start or to be created
    /// </exception>
    public async Task<NekoContainerResult> CreateAndStartNekoContainerAsync(string shareCode)
    {
        
        // Generate secured passwords for this specific session
        var viewerPassword = NekoServiceUtils.GeneratePassword();
        var hostPassword = NekoServiceUtils.GeneratePassword();
        
        // Try to generate and start the container
        var containerName = $"neko-room-{shareCode}-{Guid.NewGuid().ToString().Substring(0,5)}";

        try
        {
        
            // Define the container parameters 
            var createParams = new CreateContainerParameters
            {
                Image = "ghcr.io/m1k1o/neko/brave:latest",
                Name = containerName,
                Env = new List<string>
                {
                    $"NEKO_PASSWORD={viewerPassword}",
                    $"NEKO_PASSWORD_ADMIN={hostPassword}",
                    "NEKO_SCREEN=1280x720@30", // Set a standard resolution
                    "NEKO_EPR=52000-52099" // WebRTC UDP Port range
                },
                HostConfig = new HostConfig
                {
                    // Map n.eko's internal port 8080 to a random available port on your server
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "8080/tcp", new List<PortBinding> { new PortBinding { HostPort = "" } } // Empty string tells Docker to pick a random open port
                        }
                    },
                    AutoRemove = true // Automatically deletes the container if it crashes
                }
            };
            
            // Create the container
            var response = await _dockerClient.Containers.CreateContainerAsync(createParams);
            var containerId = response.ID;

            // Start the container
            await _dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            
            
            // Inspect the running container to find out which random port Docker assigned it
            var containerInfo = await _dockerClient.Containers.InspectContainerAsync(containerId);
            var mappedPortStr = containerInfo.NetworkSettings.Ports["8080/tcp"].First().HostPort;
            var mappedPort = int.Parse(mappedPortStr);
            
            _logger.LogInformation("Started n.eko container {ContainerId} on port {Port}", containerId, mappedPort);

            return new NekoContainerResult
            {
                ContainerId = containerId,
                ViewerPassword = viewerPassword,
                HostPassword = hostPassword,
                MappedPort = mappedPort
            };

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to start n.eko container for room {shareCode}.", shareCode);
            throw new Exception("Failed to spin up the virtual browser.");
        }

    }

    /// <summary>
    /// Methode that stops and removes an existing n.eko virtual browser
    /// </summary>
    /// <param name="containerId">
    /// A string containing the id of the container that needs to be stopped and removed
    /// </param>
    /// <returns></returns>
    /// <exception cref="Exception">
    /// If there is any problem related to stopping or removing the container
    /// </exception>
    public async Task StopAndRemoveContainerAsync(string containerId)
    {
        
        // Try to stop and remove the container
        try
        {
            // Stops the container
            // AutoRemove = true => Docker will also delete it
            await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters { WaitBeforeKillSeconds = 2 });
            _logger.LogInformation("Successfully stopped n.eko container {ContainerId}", containerId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to stop n.eko container {ContainerId}", containerId);
        }
        
    }
}