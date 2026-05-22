using Chinese_Dictionary_English_glossary_frequency_sorter.Models;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting.CharacterRules;

public interface ICharacterGlossOrderRule
{
    bool Matches(CedictEntry entry);
    IReadOnlyList<string> Sort(IReadOnlyList<string> definitions);
}
