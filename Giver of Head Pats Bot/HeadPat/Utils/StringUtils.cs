using System.Text.RegularExpressions;

namespace HeadPats.Utils; 

public static class StringUtils {
    /// <summary>
    /// Replaces each character in a string with an asterisk
    /// </summary>
    /// <param name="thisString"></param>
    /// <returns>Each character as an asterisk</returns>
    public static string Redact(this string? thisString) {
        var temp = "";
        // for (var i = 0; i < s.Length; i++)
        //     temp += "*";
        temp = thisString!.ToCharArray().Aggregate(temp, (current, empty) => current + "*");
        return temp ?? "***************";
    }
    
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
    
    private static string VerifyString(object data, Type? error = null, string? errorMessage = null, bool allowEmpty = true) {
        errorMessage ??= $"Expected a string, got {data} instead.";
        error ??= typeof(Exception);

        if (!(data is string)) throw (Exception)Activator.CreateInstance(error, errorMessage)!;
        if (!allowEmpty && ((string)data).Length == 0) throw (Exception)Activator.CreateInstance(error, errorMessage)!;
        return (string)data;
    }

    /// <summary>
    /// Safely splits a string into multiple messages.
    /// </summary>
    /// <param name="text">Main message text</param>
    /// <param name="maxLength">Length to where you want the text to split at</param>
    /// <param name="charSeparator">char separator</param>
    /// <param name="prepend">Text to add before the next message</param>
    /// <param name="append">Text to add after the first and all new split messages</param>
    /// <returns>List of strings to be used in a for or foreach loop to send messages</returns>
    /// <exception cref="Exception">Provided string was empty, nothing to split</exception>
    public static List<string> SplitMessage(string text, int maxLength = 2000, string charSeparator = "\n", string prepend = "", string append = "") {
        text = VerifyString(text);

        if (text.Length <= maxLength) return new List<string> { text };

        var messages = new List<string>();
        var msg = "";

        var splitText = text.Split(new [] { charSeparator }, StringSplitOptions.None).ToList();

        if (splitText.Any(elem => elem.Length > maxLength)) throw new Exception("SPLIT_MAX_LEN");

        foreach (var chunk in splitText) {
            if (!string.IsNullOrEmpty(msg) && (msg + charSeparator + chunk + append).Length > maxLength) {
                messages.Add(msg + append);
                msg = prepend;
            }

            msg += (string.IsNullOrEmpty(msg) || msg != prepend ? "" : charSeparator) + chunk;
        }

        messages.Add(msg);
        return messages.Where(m => !string.IsNullOrEmpty(m)).ToList();
    }
}