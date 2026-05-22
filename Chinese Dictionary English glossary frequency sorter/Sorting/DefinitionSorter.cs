namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting;

public static class DefinitionSorter
{
    private enum GlossTier
    {
        Core = 1,
        Notes = 2,
        Reference = 3,
        Variant = 4,
        Classifier = 5,
        Surname = 6,
        Archaic = 7
    }

    public static IReadOnlyList<string> Sort(IReadOnlyList<string> definitions)
    {
        return definitions
            .Select((definition, index) => new { Definition = definition, Index = index })
            .OrderBy(item => GetTier(item.Definition))
            .ThenBy(item => item.Definition.Length)
            .ThenBy(item => item.Index)
            .Select(item => item.Definition)
            .ToList();
    }

    public static string JoinSorted(IReadOnlyList<string> definitions)
    {
        return string.Join('|', Sort(definitions));
    }

    private static GlossTier GetTier(string definition)
    {
        var gloss = definition.Trim();
        if (gloss.Length == 0)
        {
            return GlossTier.Core;
        }

        if (StartsWithAny(gloss, "archaic", "old name for", "historical variant"))
        {
            return GlossTier.Archaic;
        }

        if (StartsWith(gloss, "surname "))
        {
            return GlossTier.Surname;
        }

        if (StartsWith(gloss, "CL:"))
        {
            return GlossTier.Classifier;
        }

        if (StartsWithAny(gloss,
                "variant of",
                "old variant of",
                "also written",
                "simplified form of"))
        {
            return GlossTier.Variant;
        }

        if (StartsWithAny(gloss,
                "used in ",
                "see also",
                "abbr. for",
                "erroneous variant of"))
        {
            return GlossTier.Reference;
        }

        if (StartsWithAny(gloss,
                "(bound form)",
                "(literary)",
                "(in data tables)",
                "(Tw)") || gloss.StartsWith('('))
        {
            return GlossTier.Notes;
        }

        return GlossTier.Core;
    }

    private static bool StartsWith(string text, string prefix) =>
        text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

    private static bool StartsWithAny(string text, params string[] prefixes) =>
        prefixes.Any(prefix => StartsWith(text, prefix));
}
