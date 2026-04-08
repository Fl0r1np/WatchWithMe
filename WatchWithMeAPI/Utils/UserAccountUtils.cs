using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Utils;

public class UserAccountUtils
{

    /// <summary>
    /// Converts the status to the display status
    /// </summary>
    /// <param name="status">
    /// The status to convert
    /// </param>
    /// <returns>
    /// The status converted to the display status
    /// </returns>
    public static UserStatus ConvertToDisplayStatus(UserStatus status)
    {
        // Check for stricter statuses
        if (status.Equals(UserStatus.Private))
        {
            return UserStatus.Offline;
        }
        if (status.Equals(UserStatus.DoNotDisturb))
        {
            return UserStatus.DoNotDisturb;
        }
        
        // Default to Online
        return UserStatus.Online;
    }

}