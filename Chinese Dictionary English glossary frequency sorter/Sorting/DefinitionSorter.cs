using Chinese_Dictionary_English_glossary_frequency_sorter.Models;
using Chinese_Dictionary_English_glossary_frequency_sorter.Sorting.CharacterRules;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting;

public static class DefinitionSorter
{
    public static IReadOnlyList<RankedGloss> Rank(IReadOnlyList<string> definitions) =>
        GlossScorer.RankAll(definitions);

    public static IReadOnlyList<string> Sort(IReadOnlyList<string> definitions) =>
        Rank(definitions).Select(ranked => ranked.Gloss).ToList();

    public static string JoinSorted(CedictEntry entry) =>
        JoinSorted(CharacterGlossOrderRuleRegistry.Sort(entry));

    public static string JoinSorted(IReadOnlyList<string> definitions) =>
        string.Join('|', definitions).Replace(';', '|');
}
