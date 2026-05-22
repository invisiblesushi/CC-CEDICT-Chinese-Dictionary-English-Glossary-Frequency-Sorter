using Chinese_Dictionary_English_glossary_frequency_sorter.Models;
using Chinese_Dictionary_English_glossary_frequency_sorter.Sorting;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Parsing;

/// <summary>
/// When several rows share the same simplified form (e.g. 玩/玩 and 玩/翫), keeps simplified
/// only on the best-scoring row. Others use an empty simplified string so simplified-only
/// keyboard lookup does not treat them as duplicate headwords.
/// </summary>
public static class SimplifiedCanonicalAssigner
{
    public const string EmptySimplified = "";

    public static List<CedictEntry> Assign(IEnumerable<CedictEntry> entries)
    {
        return entries
            .GroupBy(entry => entry.Simplified)
            .SelectMany(AssignGroup)
            .ToList();
    }

    private static IEnumerable<CedictEntry> AssignGroup(IGrouping<string, CedictEntry> group)
    {
        var rows = group.ToList();
        if (rows.Count == 1)
            return rows;

        var winner = rows
            .Select(entry => new { Entry = entry, Quality = ScoreEntry(entry) })
            .OrderByDescending(item => item.Quality.TopGlossScore)
            .ThenByDescending(item => item.Quality.HasCanonicalTraditionalForm)
            .ThenByDescending(item => item.Quality.StrongGlossCount)
            .ThenByDescending(item => item.Quality.DefinitionCount)
            .First()
            .Entry;

        return rows.Select(entry => entry == winner ? entry : ClearSimplified(entry));
    }

    private static CedictEntry ClearSimplified(CedictEntry entry) =>
        new()
        {
            Simplified = EmptySimplified,
            Traditional = entry.Traditional,
            Pinyin = entry.Pinyin,
            Definitions = entry.Definitions
        };

    private static EntryQuality ScoreEntry(CedictEntry entry)
    {
        var ranked = DefinitionSorter.Rank(entry.Definitions);
        return new EntryQuality(
            TopGlossScore: ranked.Count > 0 ? ranked[0].Score : 0,
            HasCanonicalTraditionalForm: entry.Traditional == entry.Simplified,
            StrongGlossCount: ranked.Count(gloss => gloss.Score >= 0.40),
            DefinitionCount: entry.Definitions.Count);
    }

    private sealed record EntryQuality(
        double TopGlossScore,
        bool HasCanonicalTraditionalForm,
        int StrongGlossCount,
        int DefinitionCount);
}
