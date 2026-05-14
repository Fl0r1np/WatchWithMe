using WatchWithMeAPI.Model;

namespace WatchWithMeAPI.Utils;

public class RoomServiceUtils
{

    /// <summary>
    /// Method to generate a random ShareCode for a room
    /// </summary>
    /// <returns>
    /// A string containing the ShareCode
    /// </returns>
    public static string GenerateShareCode()
    {
        
        // The pool of characters to draw from (uppercase letters and numbers)
        const string CharacterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        // An array to hold our 9 characters (8 alphanumeric + 1 hyphen)
        char[] codeBuffer = new char[9];

        for (int i = 0; i < codeBuffer.Length; i++)
        {
            if (i == 4)
            {
                // Insert the hyphen at the 5th position (index 4)
                codeBuffer[i] = '-';
            }
            else
            {
                // Pick a random character from the pool
                int randomIndex = Random.Shared.Next(CharacterPool.Length);
                codeBuffer[i] = CharacterPool[randomIndex];
            }
        }

        return new string(codeBuffer);
        
    }

}