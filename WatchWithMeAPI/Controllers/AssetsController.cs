using Microsoft.AspNetCore.Mvc;

namespace WatchWithMeAPI.Controllers;

[Route("api/assets")]
[ApiController]
public class AssetsController : ControllerBase
{
    
    private readonly IWebHostEnvironment _env;
    
    public AssetsController(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    /// <summary>
    /// Method to get the list of avatars
    /// </summary>
    /// <returns>
    /// Returns a list of available avatars
    /// </returns>
    [HttpGet("avatars")]
    public IActionResult GetAvatarList()
    {
        
        try
        {

            // Define the physical path to the avatars folder
            var avatarsPath = Path.Combine(_env.ContentRootPath, "wwwroot","assets", "avatars");
            
            // Verify if the avatars folder exists
            if (!Directory.Exists(avatarsPath))
            {
                return NotFound("Avatars folder not found.");
            }
            
            // Get all the *png files in the avatars folder
            var files = Directory.GetFiles(avatarsPath, "*.png")
                .Select(Path.GetFileName) // Get only the file name
                .ToList();
            
            // Return the list of avatars
            return Ok(files);
            
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        
    }
    
}