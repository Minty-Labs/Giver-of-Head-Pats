namespace HeadPats.Utils; 

public static class StringUtils {
    public static string ReplaceAll(this string theStringToBeEdited, string oldCharacters, string newCharacters) // Idea from Java String.ReplaceAll()
        => oldCharacters.ToCharArray().Aggregate(theStringToBeEdited, (current, c) => current.Replace($"{c}", newCharacters));
    
    public static bool GetBooleanFromString(string input) => input.ToLower().Equals("true");
}