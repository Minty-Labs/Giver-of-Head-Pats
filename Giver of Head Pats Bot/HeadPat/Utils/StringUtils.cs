namespace HeadPats.Utils; 

public static class StringUtils {
    /// <summary>
    /// Replaces all of the old characters (as char) to anything of your choosing.
    /// </summary>
    /// <param name="theStringToBeEdited">this</param>
    /// <param name="oldCharacters">Characters (char[]) to be replaced</param>
    /// <param name="newCharacters">What the old characters will be replaced to. If left blank, outcome to be blank</param>
    /// <returns>new string with the replaced chars</returns>
    public static string ReplaceAll(this string theStringToBeEdited, string oldCharacters, string newCharacters = "") // Idea from Java String.ReplaceAll()
        => oldCharacters.ToCharArray().Aggregate(theStringToBeEdited, (current, c) => current.Replace($"{c}", newCharacters));
    
    /// <summary>
    /// Returns a bool if the inputted string is "true" or not
    /// </summary>
    /// <param name="input"></param>
    /// <returns>boolean (true|false)</returns>
    public static bool GetBooleanFromString(string input) => input.ToLower().Equals("true");
    
    /// <summary>
    /// Returns a random string of a specified length
    /// </summary>
    /// <param name="length">Length of string</param>
    /// <returns>random string of characters and numbers</returns>
    public static string GetRandomString(int length = 15) => new (Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz", length)
        .Select(s => s[new Random().Next(s.Length)]).ToArray());

    /// <summary>
    /// Checks if the string contains the given string, if it does, it replaces it with your given string.
    /// </summary>
    /// <param name="theStringToBeEdited">this</param>
    /// <param name="contains">string to check if it contains to</param>
    /// <param name="replaceWith">string to replace contains string with</param>
    /// <returns>new string with replaced content</returns>
    public static string ContainsAndReplace(this string theStringToBeEdited, string contains, string replaceWith)
        => !theStringToBeEdited.Contains(contains) ? theStringToBeEdited : theStringToBeEdited.Replace(contains, replaceWith);

    /// <summary>
    /// Checks if the string contains the given string, if it does, it replaces the entire string with your given string.
    /// </summary>
    /// <param name="theStringToBeEdited">this</param>
    /// <param name="contains">string to check if it contains to</param>
    /// <param name="replaceWith">string to replace entire string with</param>
    /// <returns>new string with replaced content</returns>
    public static string ContainsAndReplaceWhole(this string theStringToBeEdited, string contains, string replaceWith)
        => !theStringToBeEdited.Contains(contains) ? theStringToBeEdited : replaceWith;
    
    /// <summary>
    /// Escapes Discord text modifiers (bold, italics, strikethrough, code, and spoiler) in a string.
    /// </summary>
    /// <param name="thisString">this</param>
    /// <returns>Returns a string with the modifers escaped.</returns>
    public static string EscapeTextModifiers(this string thisString) {
        var asChars = thisString.ToCharArray();
        var hasTwoOrMoreUnderscores = asChars.Count(c => c == '_') >= 2;
        var hasTwoOrMoreAsterisks = asChars.Count(c => c == '*') >= 2;
        var hasTwoOrMoreTildes = asChars.Count(c => c == '~') >= 2;
        var hasTwoOrMoreBackticks = asChars.Count(c => c == '`') >= 2;
        var hasBackSlash = asChars.Count(c => c == '\\') >= 1;

        if (hasTwoOrMoreUnderscores)
            thisString = thisString.ReplaceAll("_", "\\_");
        if (hasTwoOrMoreAsterisks)
            thisString = thisString.ReplaceAll("*", "\\*");
        if (hasTwoOrMoreTildes)
            thisString = thisString.ReplaceAll("~", "\\~");
        if (hasTwoOrMoreBackticks)
            thisString = thisString.ReplaceAll("`", "\\`");
        if (hasBackSlash)
            thisString = thisString.ReplaceAll("\\", "\\\\");
        return thisString;
    }
}