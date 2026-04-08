using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Utils;

public class UserAccountUtils
{

    public static UserStatus ConvertToDisplayStatus(UserStatus status)
    {
        if (status.Equals(UserStatus.Public))
        {
            return UserStatus.Online;
        }

        if (status.Equals(UserStatus.Private))
        {
            return UserStatus.Offline;
        }

        if (status.Equals(UserStatus.DoNotDisturb))
        {
            return UserStatus.DoNotDisturb;
        }
        
        return UserStatus.Online;
    }

}