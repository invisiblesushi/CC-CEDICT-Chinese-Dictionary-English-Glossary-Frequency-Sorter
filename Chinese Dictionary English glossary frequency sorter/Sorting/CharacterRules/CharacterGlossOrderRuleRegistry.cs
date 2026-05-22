using Chinese_Dictionary_English_glossary_frequency_sorter.Models;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting.CharacterRules;

public static class CharacterGlossOrderRuleRegistry
{
    private static readonly ICharacterGlossOrderRule[] Rules =
        CharacterGlossOrderRules.Definitions
            .Select(definition => (ICharacterGlossOrderRule)new CharacterGlossOrderRuleBase.Configured(definition))
            .ToArray();

    public static IReadOnlyList<string> Sort(CedictEntry entry)
    {
        var rule = Rules.FirstOrDefault(r => r.Matches(entry));
        return rule?.Sort(entry.Definitions) ?? DefinitionSorter.Sort(entry.Definitions);
    }
}
