namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting.CharacterRules;

/// <summary>
/// Optional manual gloss order overrides. Most entries are ranked by heuristic scoring in GlossScorer.
/// Add entries here only when scores are wrong; first match wins.
/// </summary>
internal static class CharacterGlossOrderRules
{
    internal static readonly CharacterGlossOrderRuleDefinition[] Definitions =
    [
        new(Simplified: "你", Traditional: null, PriorityGlosses: ["you", "your"]),
        new(Simplified: "元", Traditional: null, PriorityGlosses: ["currency"]),
        new(Simplified: "他", Traditional: null, PriorityGlosses: ["him"]),
        new(Simplified: "的", Traditional: null, PriorityGlosses: ["of"]),
        new(Simplified: "了", Traditional: null, PriorityGlosses: ["to finish"]),
        new(Simplified: "水平", Traditional: null, PriorityGlosses: ["level"]),
        new(Simplified: "玩", Traditional: null, PriorityGlosses: ["play"]),
    ];
}
