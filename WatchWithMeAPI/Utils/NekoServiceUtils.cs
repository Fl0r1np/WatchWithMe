namespace WatchWithMeAPI.Utils;

public class NekoServiceUtils
{

    public static string GeneratePassword()
    {
        // The pool of characters to draw from (uppercase letters and numbers)
        const string CharacterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        
        // An array to hold our 8 characters
        char[] codeBuffer = new char[8];
        
        for (int i = 0; i < codeBuffer.Length; i++)
        {
            // Pick a random character from the pool
            int randomIndex = Random.Shared.Next(CharacterPool.Length);
            codeBuffer[i] = CharacterPool[randomIndex];
        }

        return new string(codeBuffer);
        
    }

}