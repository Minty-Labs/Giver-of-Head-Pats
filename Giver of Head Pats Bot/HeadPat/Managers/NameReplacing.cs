﻿namespace HeadPats.Managers; 

public static class NameReplacing {
    public static string ReplaceTheNamesWithTags(this string input) {
        input = input.Replace("MintLily#0001", "Lily");
        input = input.Replace("Silentt.#5610", "Elly");
        input = input.Replace("Penny#9538", "Penny");
        input = input.Replace(".FS.#8519", "Autumn");
        input = input.Replace("Nail#3021", "Iana");
        input = input.Replace("Nail#9290", "Iana");
        input = input.Replace("jettsd#1111", "Emily");
        input = input.Replace("AxolotlFren#9690", "Erin");
        return input;
    }
    
    public static string ReplaceTheNames(this string input) {
        input = input.Replace("MintLily", "Lily");
        input = input.Replace("Silentt.", "Elly");
        input = input.Replace("Penny", "Penny");
        input = input.Replace(".FS.", "Autumn");
        input = input.Replace("Nail", "Iana");
        input = input.Replace("jettsd", "Emily");
        input = input.Replace("AxolotlFren", "Erin");
        return input;
    }
}