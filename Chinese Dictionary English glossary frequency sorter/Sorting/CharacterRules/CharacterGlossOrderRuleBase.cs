using Chinese_Dictionary_English_glossary_frequency_sorter.Models;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting.CharacterRules;

public abstract class CharacterGlossOrderRuleBase : ICharacterGlossOrderRule
{
    protected abstract string Simplified { get; }
    protected virtual string? Traditional => null;
    protected abstract IReadOnlyList<string> PriorityGlosses { get; }

    internal sealed class Configured(CharacterGlossOrderRuleDefinition definition) : CharacterGlossOrderRuleBase
    {
        protected override string Simplified => definition.Simplified;
        protected override string? Traditional => definition.Traditional;
        protected override IReadOnlyList<string> PriorityGlosses => definition.PriorityGlosses;
    }

    public bool Matches(CedictEntry entry) =>
        entry.Simplified == Simplified
        && (Traditional is null || entry.Traditional == Traditional);

    public IReadOnlyList<string> Sort(IReadOnlyList<string> definitions)
    {
        var remaining = definitions.ToList();
        var result = new List<string>();

        foreach (var priorityKey in PriorityGlosses)
        {
            var matches = remaining.Where(definition => MatchesGloss(definition, priorityKey)).ToList();
            foreach (var match in matches)
            {
                result.Add(match);
                remaining.Remove(match);
            }
        }

        result.AddRange(DefinitionSorter.Sort(remaining));
        return result;
    }

    protected virtual bool MatchesGloss(string gloss, string priorityKey)
    {
        var trimmed = gloss.Trim();
        return trimmed.Equals(priorityKey, StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith(priorityKey + " ", StringComparison.OrdinalIgnoreCase);
    }
}
