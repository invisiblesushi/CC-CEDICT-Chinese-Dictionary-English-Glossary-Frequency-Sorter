using Chinese_Dictionary_English_glossary_frequency_sorter.Configuration;
using Chinese_Dictionary_English_glossary_frequency_sorter.Database;
using Chinese_Dictionary_English_glossary_frequency_sorter.Models;
using Chinese_Dictionary_English_glossary_frequency_sorter.Parsing;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var paths = configuration.GetSection("Paths").Get<PathSettings>()
    ?? throw new InvalidOperationException("Paths settings are missing from appsettings.json.");

var cedictPath = AppConfiguration.ResolveRepositoryPath(paths.CedictInput);
var sqlitePath = AppConfiguration.ResolveRepositoryPath(paths.SqliteOutput);

if (!File.Exists(cedictPath))
{
    Console.Error.WriteLine($"CC-CEDICT file not found: {cedictPath}");
    return 1;
}

var parsedEntries = CedictParser.ParseFile(cedictPath).ToList();
if (parsedEntries.Count == 0)
{
    Console.Error.WriteLine($"No dictionary entries were parsed from: {cedictPath}");
    return 1;
}

var merged = CedictEntryMerger.MergeDuplicateWords(parsedEntries);
var entries = SimplifiedCanonicalAssigner.Assign(merged);
SqliteWriter.WriteEntries(sqlitePath, entries);

Console.WriteLine($"Parsed {parsedEntries.Count} CC-CEDICT lines.");
Console.WriteLine($"Merged to {entries.Count} entries.");
Console.WriteLine($"Wrote SQLite database to: {sqlitePath}");
return 0;
