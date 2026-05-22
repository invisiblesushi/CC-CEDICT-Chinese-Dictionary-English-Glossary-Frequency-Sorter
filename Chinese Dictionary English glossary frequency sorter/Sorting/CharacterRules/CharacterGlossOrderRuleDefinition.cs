namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting.CharacterRules;

internal sealed record CharacterGlossOrderRuleDefinition(
    string Simplified,
    string? Traditional,
    IReadOnlyList<string> PriorityGlosses);
