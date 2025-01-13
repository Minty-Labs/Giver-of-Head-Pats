namespace HeadPats.Utils;

public static class MarkdownUtils {
    #region Complex Markdown

    public static string MakeLink(string text, string url) => $"[{text}]({url})";

    #endregion

    #region Simple Markdown

    public static string ToItalics(string text) => $"*{text}*";
    public static string ToBold(string text) => $"**{text}**";
    public static string ToUnderline(string text) => $"__{text}__";
    public static string ToStrikeThrough(string text) => $"~~{text}~~";

    public static string ToCodeBlockSingleline(string text) => $"`{text}`";
    public static string ToCodeBlockMultiline(string text, string? languageCode = null) => $"```{languageCode ?? ""}\n{text}```";
    public static string ToCodeBlockMultiline(string text, CodingLanguages codingLanguages) => $"```{Enum.GetName(typeof(CodingLanguages), codingLanguages)}\n{text}```";

    public static string ToBlockQuoteSingleline(string text) => $"> {text}";
    public static string ToBlockQuoteMultiline(string text) => $">>> {text}";

    public static string ToSpoiler(string text) => $"||{text}||";
    
    public static string ToHeading1(string text) => $"# {text}";
    public static string ToHeading2(string text) => $"## {text}";
    public static string ToHeading3(string text) => $"### {text}";
    public static string ToSubtext(string text) => $"-# {text}";

    #endregion

    #region Composite Markdown

    public static string ToBoldItalics(string text) => ToItalics(ToBold(text));
    public static string ToUnderlineItalics(string text) => ToUnderline(ToItalics(text));
    public static string ToUnderlineBold(string text) => ToUnderline(ToBold(text));
    public static string ToUnderlineBoldItalics(string text) => ToUnderline(ToBold(ToItalics(text)));

    #endregion
}

public enum CodingLanguages {
    actionscript3,
    apache,
    applescript,
    asp,
    brainfuck,
    c,
    cfm,
    clojure,
    cmake,

    /// <summary>CoffeeScript</summary>
    coffeescript,

    /// <summary>CoffeeScript</summary>
    coffee,

    /// <summary>C++</summary>
    cpp,
    cs,
    csharp,
    css,
    csv,
    bash,
    diff,
    elixir,

    /// <summary>HTML + Embedded Ruby On Rails</summary>
    erb,
    go,
    haml,
    http,
    java,
    javascript,
    json,
    jsx,
    less,
    lolcode,

    /// <summary>Makefile</summary>
    make,
    markdown,
    matlab,
    nginx,
    objectivec,
    pascal,
    PHP,
    Perl,
    python,

    /// <summary> Python profiler output</summary>
    profile,
    rust,

    /// <summary>Salt</summary>
    salt,

    /// <summary>Salt</summary>
    saltstate,

    /// <summary>Shell scripting</summary>
    shell,

    /// <summary>Shell scripting</summary>
    sh,

    /// <summary>Shell scripting</summary>
    zsh,

    /// <summary>Shell scripting</summary>
    scss,
    sql,
    svg,
    swift,

    /// <summary>Ruby On Rails</summary>
    rb,

    /// <summary>Ruby On Rails</summary>
    jruby,

    /// <summary>Ruby On Rails</summary>
    ruby,
    smalltalk,

    /// <summary>Vim Script</summary>
    vim,

    /// <summary>Vim Script</summary>
    viml,
    volt,
    vhdl,
    vue,

    /// <summary>XML and also used for HTML with inline CSS and Javascript</summary>
    xml,
    yaml
}