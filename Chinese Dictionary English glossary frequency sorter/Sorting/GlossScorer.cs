namespace Chinese_Dictionary_English_glossary_frequency_sorter.Sorting;

internal static class GlossScorer
{
    private const double BaseScore = 0.50;
    private const double MinScore = 0.0;
    private const double MaxScore = 1.0;

    public static RankedGloss Rank(string gloss, int sourceIndex) =>
        new(gloss, Score(gloss, sourceIndex), sourceIndex);

    public static double Score(string gloss, int sourceIndex)
    {
        var trimmed = gloss.Trim();
        var wordCount = CountWords(trimmed);

        var score = BaseScore
                      + GetSimplicityBonus(trimmed, wordCount)
                      + GetPositionBonus(sourceIndex)
                      + GetPosBonus(trimmed, wordCount)
                      - GetMetadataPenalties(trimmed)
                      - GetComplexityPenalties(trimmed, wordCount);

        return Math.Clamp(score, MinScore, MaxScore);
    }

    public static IReadOnlyList<RankedGloss> RankAll(IReadOnlyList<string> definitions) =>
        definitions
            .Select((definition, index) => Rank(definition, index))
            .OrderByDescending(ranked => ranked.Score)
            .ThenBy(ranked => ranked.SourceIndex)
            .ToList();

    private static double GetSimplicityBonus(string gloss, int wordCount) => wordCount switch
    {
        1 => 0.18,
        2 => 0.10,
        _ => 0.00
    };

    private static double GetPositionBonus(int sourceIndex) => sourceIndex switch
    {
        0 => 0.10,
        1 => 0.06,
        2 => 0.03,
        _ => 0.00
    };

    private static double GetPosBonus(string gloss, int wordCount)
    {
        if (StartsWithIgnoreCase(gloss, "to "))
            return 0.08;

        if (wordCount == 1 && !LooksLikeMetadata(gloss))
            return 0.06;

        return 0.00;
    }

    private static double GetMetadataPenalties(string gloss)
    {
        var penalty = 0.0;
        var lower = gloss.ToLowerInvariant();

        if (ContainsIgnoreCase(lower, "surname"))
            penalty += 0.60;
        if (ContainsIgnoreCase(lower, "variant of"))
            penalty += 0.50;
        if (ContainsIgnoreCase(lower, "old variant"))
            penalty += 0.50;
        if (ContainsIgnoreCase(lower, "classifier") || StartsWithIgnoreCase(gloss, "CL:"))
            penalty += 0.70;
        if (ContainsIgnoreCase(lower, "kangxi radical"))
            penalty += 0.80;
        if (ContainsIgnoreCase(lower, "used in "))
            penalty += 0.30;
        if (ContainsIgnoreCase(lower, "archaic"))
            penalty += 0.45;
        if (ContainsIgnoreCase(lower, "literary"))
            penalty += 0.35;
        if (ContainsIgnoreCase(lower, "dialect"))
            penalty += 0.25;
        if (ContainsIgnoreCase(lower, "transliteration"))
            penalty += 0.50;
        if (ContainsIgnoreCase(lower, "abbreviation") || ContainsIgnoreCase(lower, "abbr."))
            penalty += 0.25;

        return penalty;
    }

    private static double GetComplexityPenalties(string gloss, int wordCount)
    {
        var penalty = 0.0;

        if (wordCount > 5)
            penalty += 0.15;

        if (IsSemicolonHeavy(gloss))
            penalty += 0.10;

        if (gloss.Contains('('))
            penalty += 0.08;

        if (gloss.Count(c => c == ',') > 2)
            penalty += 0.08;

        return penalty;
    }

    private static bool IsSemicolonHeavy(string gloss) =>
        gloss.Count(c => c == ';') >= 2
        || (gloss.Contains(';') && gloss.Length > 30);

    private static bool LooksLikeMetadata(string gloss)
    {
        var lower = gloss.ToLowerInvariant();
        return StartsWithIgnoreCase(gloss, "CL:")
               || ContainsIgnoreCase(lower, "surname")
               || ContainsIgnoreCase(lower, "variant")
               || ContainsIgnoreCase(lower, "classifier")
               || ContainsIgnoreCase(lower, "archaic");
    }

    private static int CountWords(string gloss)
    {
        if (gloss.Length == 0)
            return 0;

        return gloss.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static bool StartsWithIgnoreCase(string text, string prefix) =>
        text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

    private static bool ContainsIgnoreCase(string text, string value) =>
        text.Contains(value, StringComparison.OrdinalIgnoreCase);
}
