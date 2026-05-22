using Chinese_Dictionary_English_glossary_frequency_sorter.Models;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Parsing;

public static class CedictEntryMerger
{
    /// <summary>
    /// Merges CC-CEDICT lines only when both simplified and traditional match.
    /// Same simplified with different traditional (e.g. 玩/玩 vs 玩/翫) stay separate entries.
    /// </summary>
    public static List<CedictEntry> MergeDuplicateWords(IEnumerable<CedictEntry> entries)
    {
        return entries
            .GroupBy(entry => (entry.Simplified, entry.Traditional))
            .Select(MergeGroup)
            .ToList();
    }

    private static CedictEntry MergeGroup(IGrouping<(string Simplified, string Traditional), CedictEntry> group)
    {
        var entries = group.ToList();
        var first = entries[0];

        var definitions = entries
            .SelectMany(entry => entry.Definitions)
            .Where(definition => !string.IsNullOrWhiteSpace(definition))
            .GroupBy(definition => definition, StringComparer.OrdinalIgnoreCase)
            .Select(definitionGroup => definitionGroup.First())
            .ToList();

        var pinyinReadings = entries
            .Select(entry => entry.Pinyin)
            .Where(pinyin => !string.IsNullOrWhiteSpace(pinyin))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        return new CedictEntry
        {
            Simplified = first.Simplified,
            Traditional = first.Traditional,
            Pinyin = string.Join('|', pinyinReadings),
            Definitions = definitions
        };
    }
}
