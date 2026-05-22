using System.Text.RegularExpressions;
using Chinese_Dictionary_English_glossary_frequency_sorter.Models;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Parsing;

public static partial class CedictParser
{
    [GeneratedRegex(@"^(\S+)\s+(\S+)\s+\[([^\]]+)\]\s+/(.+)/$")]
    private static partial Regex EntryLineRegex();

    public static IEnumerable<CedictEntry> ParseFile(string filePath)
    {
        return File.ReadLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Where(line => !line.StartsWith('#'))
            .Select(ParseLine)
            .Where(entry => entry is not null)
            .Select(entry => entry!);
    }

    private static CedictEntry? ParseLine(string line)
    {
        var match = EntryLineRegex().Match(line);
        if (!match.Success)
            return null;

        var definitions = match.Groups[4].Value
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (definitions.Count == 0)
            return null;

        return new CedictEntry
        {
            Traditional = match.Groups[1].Value,
            Simplified = match.Groups[2].Value,
            Pinyin = PinyinToneConverter.ToToneMarks(match.Groups[3].Value),
            Definitions = definitions
        };
    }
}
