namespace Chinese_Dictionary_English_glossary_frequency_sorter.Models;

public sealed class CedictEntry
{
    public required string Simplified { get; init; }
    public required string Traditional { get; init; }
    public required string Pinyin { get; init; }
    public required IReadOnlyList<string> Definitions { get; init; }
}
