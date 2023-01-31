namespace HeadPats.Utils; 

public static class StringUtils {
    /// <summary>
    /// Replaces all of the old characters (as char) to anything of your choosing.
    /// </summary>
    /// <param name="theStringToBeEdited"></param>
    /// <param name="oldCharacters">Characters (char[]) to be replaced</param>
    /// <param name="newCharacters">What the old characters will be replaced to. If left blank, outcome to be blank</param>
    /// <returns></returns>
    public static string ReplaceAll(this string theStringToBeEdited, string oldCharacters, string newCharacters = "") // Idea from Java String.ReplaceAll()
        => oldCharacters.ToCharArray().Aggregate(theStringToBeEdited, (current, c) => current.Replace($"{c}", newCharacters));
    
    /// <summary>
    /// Returns a bool if the inputted string is "true" or not
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool GetBooleanFromString(string input) => input.ToLower().Equals("true");
    
    /// <summary>
    /// Returns a random string of a specified length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GetRandomString(int length = 15) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}