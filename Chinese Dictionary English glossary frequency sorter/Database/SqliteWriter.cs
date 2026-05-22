using Chinese_Dictionary_English_glossary_frequency_sorter.Models;
using Chinese_Dictionary_English_glossary_frequency_sorter.Sorting;
using Microsoft.Data.Sqlite;

namespace Chinese_Dictionary_English_glossary_frequency_sorter.Database;

public static class SqliteWriter
{
    public static void WriteEntries(string databasePath, IEnumerable<CedictEntry> entries)
    {
        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        if (File.Exists(databasePath))
            File.Delete(databasePath);

        using var connection = new SqliteConnection($"Data Source={databasePath}");
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Database", "Schema.sql"));
            command.ExecuteNonQuery();
        }

        using var transaction = connection.BeginTransaction();
        using var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = """
            INSERT INTO entries 
                (simplified, traditional, pinyin, definitionsInEnglish)
            VALUES 
                ($simplified, $traditional, $pinyin, $definitionsInEnglish);
            """;
        
        insertCommand.Parameters.Add("$simplified", SqliteType.Text);
        insertCommand.Parameters.Add("$traditional", SqliteType.Text);
        insertCommand.Parameters.Add("$pinyin", SqliteType.Text);
        insertCommand.Parameters.Add("$definitionsInEnglish", SqliteType.Text);
        
        foreach (var entry in entries)
        {
            insertCommand.Parameters["$simplified"].Value = entry.Simplified;
            insertCommand.Parameters["$traditional"].Value = entry.Traditional;
            insertCommand.Parameters["$pinyin"].Value = entry.Pinyin;
            insertCommand.Parameters["$definitionsInEnglish"].Value = DefinitionSorter.JoinSorted(entry);
            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }
}