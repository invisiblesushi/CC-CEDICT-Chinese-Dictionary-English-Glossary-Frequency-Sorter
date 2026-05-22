# Chinese Dictionary English Glossary Frequency Sorter

A .NET console tool that builds a SQLite dictionary from [CC-CEDICT](https://www.mdbg.net/chinese/dictionary?page=cedict) for use in an Android app. English definitions are merged, sorted (everyday meanings before surname / variant / classifier notes), and written to a single database file.

## What this tool does

1. Parse CC-CEDICT lines  
2. Merge rows with the same simplified + traditional  
3. Sort English glosses (rule-based: core meanings first; surname, `variant of`, `CL:`, etc. last)  
4. Write SQLite with indexes  

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Quick start

1. Clone this repository.
2. Download CC-CEDICT (UTF-8, simplified + traditional) from [MDBG CC-CEDICT export](https://www.mdbg.net/chinese/export/cedict/).
3. Save the file as `data/input/cedict_ts.u8` (create the folder if needed).
4. From the repository root, run:

```bash
dotnet run --project "Chinese Dictionary English glossary frequency sorter"
```

5. Copy `data/output/dictionary.db` into your Android project.

Input and output files are **not** included in the repo (see `.gitignore`). You must provide CC-CEDICT yourself.

## Configuration

Edit `Chinese Dictionary English glossary frequency sorter/appsettings.json`:

| Setting | Default | Description |
|---------|---------|-------------|
| `CedictInput` | `data/input/cedict_ts.u8` | Path to CC-CEDICT `.u8` file |
| `SqliteOutput` | `data/output/dictionary.db` | Output SQLite path |

Paths are relative to the repository root (folder containing the `.sln` file).

## Database schema

Table `entries`:

| Column | Description |
|--------|-------------|
| `simplified` | Simplified Chinese (lookup key) |
| `traditional` | Traditional Chinese (lookup key) |
| `pinyin` | Pinyin (multiple readings joined with `\|`) |
| `definitionsInEnglish` | Glosses joined with `\|`, sorted by built-in rules |

Indexes: `idx_entries_simplified`, `idx_entries_traditional`.

## License

### This project (tool source code)

This repository’s **source code** is under the [MIT License](LICENSE).

### CC-CEDICT data (important)

CC-CEDICT is **not** included in this repo. When you download and use it:

- License: [Creative Commons Attribution-ShareAlike 4.0](https://creativecommons.org/licenses/by-sa/4.0/)
- **Attribution:** credit [CC-CEDICT / MDBG](https://www.mdbg.net/chinese/dictionary?page=cc-cedict) in your app (e.g. About screen)
- **ShareAlike:** if you distribute the **generated** `dictionary.db` (a derivative of CC-CEDICT), you may need to allow others to use and share it under the same license — confirm with the [CC-CEDICT wiki](https://cc-cedict.org/wiki/) and your legal needs

This tool only processes data you supply locally; it does not redistribute CC-CEDICT.
