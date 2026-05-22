using System.Text;
using System.Text.RegularExpressions;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Parsing;

/// <summary>
/// Converts CC-CEDICT numbered pinyin (ni3 hao3) to tone-marked pinyin (nǐ hǎo).
/// </summary>
public static partial class PinyinToneConverter
{
    private static readonly (char Plain, char Tone1, char Tone2, char Tone3, char Tone4)[] VowelForms =
    [
        ('a', 'ā', 'á', 'ǎ', 'à'),
        ('e', 'ē', 'é', 'ě', 'è'),
        ('i', 'ī', 'í', 'ǐ', 'ì'),
        ('o', 'ō', 'ó', 'ǒ', 'ò'),
        ('u', 'ū', 'ú', 'ǔ', 'ù'),
        ('ü', 'ǖ', 'ǘ', 'ǚ', 'ǜ'),
    ];

    public static string ToToneMarks(string numberedPinyin)
    {
        if (string.IsNullOrWhiteSpace(numberedPinyin))
            return numberedPinyin;

        return NumberedSyllableRegex().Replace(numberedPinyin, match =>
        {
            var syllable = match.Groups["syllable"].Value;
            var tone = int.Parse(match.Groups["tone"].Value);
            return ConvertSyllable(syllable, tone);
        });
    }

    private static string ConvertSyllable(string syllable, int tone)
    {
        if (tone is < 1 or > 5)
            return syllable + tone;

        var normalized = NormalizeSyllable(syllable);
        var vowelIndex = GetToneMarkVowelIndex(normalized);
        if (vowelIndex < 0)
            return syllable;

        if (tone == 5)
            return normalized;

        var vowelChar = normalized[vowelIndex];
        if (vowelChar == 'v')
            vowelChar = 'ü';

        var markedVowel = GetMarkedVowel(vowelChar, tone);
        if (markedVowel is null)
            return normalized;

        var result = new StringBuilder(normalized);
        result[vowelIndex] = markedVowel.Value;
        return result.ToString();
    }

    private static string NormalizeSyllable(string syllable)
    {
        var lower = syllable.ToLowerInvariant().Replace("u:", "ü", StringComparison.Ordinal);
        if (lower.Length >= 2 && lower[^1] == 'v' && lower[^2] is 'n' or 'l')
            lower = lower[..^1] + "ü";

        return lower;
    }

    private static int GetToneMarkVowelIndex(string syllable)
    {
        var lower = syllable.ToLowerInvariant();

        if (lower.Contains('a'))
            return lower.IndexOf('a');

        if (lower.Contains('e'))
            return lower.IndexOf('e');

        if (lower.Contains("ou", StringComparison.Ordinal))
            return lower.IndexOf('o');

        if (lower.Contains("iu", StringComparison.Ordinal))
            return lower.IndexOf('u');

        if (lower.Contains("ui", StringComparison.Ordinal))
            return lower.IndexOf('i');

        for (var i = lower.Length - 1; i >= 0; i--)
        {
            if (lower[i] is 'i' or 'u' or 'ü' or 'v')
                return i;
        }

        return -1;
    }

    private static char? GetMarkedVowel(char plain, int tone)
    {
        foreach (var forms in VowelForms)
        {
            if (forms.Plain != plain)
                continue;

            return tone switch
            {
                1 => forms.Tone1,
                2 => forms.Tone2,
                3 => forms.Tone3,
                4 => forms.Tone4,
                _ => null
            };
        }

        return null;
    }

    [GeneratedRegex(@"(?<syllable>[a-zA-Z]+:?[a-zA-Z]*)(?<tone>[1-5])", RegexOptions.IgnoreCase)]
    private static partial Regex NumberedSyllableRegex();
}
