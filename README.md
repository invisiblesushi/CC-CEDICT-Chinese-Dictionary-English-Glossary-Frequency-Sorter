# Chinese Dictionary English Glossary Frequency Sorter

A .NET console tool that builds a SQLite dictionary from [CC-CEDICT](https://www.mdbg.net/chinese/dictionary?page=cedict) for use in an Android app. English definitions are merged, sorted (everyday meanings before surname / variant / classifier notes), and written to a single database file.

## What this tool does

1. Parse CC-CEDICT lines (numbered pinyin → diacritic tone marks)  
2. Merge CC-CEDICT lines only when **both** simplified and traditional match  
3. **Canonical simplified:** if multiple rows share the same simplified (e.g. 玩/玩 and 玩/翫), keep `simplified` only on the best-scoring row; others get `simplified = ""` (empty string) so simplified-only keyboard lookup hits one headword  
4. Sort English glosses by heuristic score (common words and short glosses first; metadata/classifier/surname glosses last)  
5. Write SQLite with indexes  

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
| `simplified` | Simplified Chinese lookup key; empty string when this row is a secondary traditional form for the same simplified (see canonical simplified step) |
| `traditional` | Traditional Chinese (lookup key; use when `simplified` is empty) |
| `pinyin` | Tone-marked pinyin (converted from CC-CEDICT numbered form, e.g. `ni3 hao3` → `nǐ hǎo`; multiple readings joined with `\|`) |
| `definitionsInEnglish` | Glosses joined with `\|` (any `;` in source text is normalized to `\|`), sorted by score (highest first) |

Indexes: unique `(simplified, traditional)` when `simplified` is not empty; lookups on `simplified` and `traditional`. Multiple secondary rows may use `simplified = ''` with the same `traditional` (they are excluded from the unique index).

**玩 example:** CC-CEDICT has 玩/翫 (variant note) and 玩/玩 (to play, toy, …). Definitions stay on two rows, but after canonical simplified assignment:

| simplified | traditional | Role |
|------------|-------------|------|
| 玩 | 玩 | Primary row (higher gloss scores) |
| *(empty)* | 翫 | Secondary form; lookup by `traditional` only |

Keyboard query `WHERE simplified = '玩'` returns one primary entry. Query `WHERE traditional = '翫'` still finds the variant row.

## Gloss ranking (heuristic scores)

Each gloss gets a score in `[0, 1]` from `GlossScorer` (see [`GlossScorer.cs`](Chinese Dictionary English glossary frequency sorter/Sorting/GlossScorer.cs)):

**Formula:** `Base (0.50) + simplicity + position + POS bonus − penalties`, then clamped.

| Component | Effect |
|-----------|--------|
| Simplicity | 1 word +0.18, 2 words +0.10 |
| Position | 1st +0.10, 2nd +0.06, 3rd +0.03 |
| POS | starts with `to ` +0.08; single-word noun +0.06 |
| Metadata | `surname`, `CL:`, `classifier`, `variant of`, `archaic`, etc. −0.25 to −0.80 |
| Complexity | long gloss, semicolons, parentheses, many commas |

Glosses are sorted **descending by score** (stable tie-break: original CC-CEDICT order). Goal: best learner-facing primary gloss first, not linguistic perfection.

| Score | Typical use |
|-------|-------------|
| 0.90+ | excellent primary gloss |
| 0.75+ | strong |
| 0.60+ | acceptable |
| &lt;0.40 | likely metadata/noise |

`RankedGloss` (`Gloss`, `Score`, `SourceIndex`) is available via `DefinitionSorter.Rank()` for debugging or tooling.

## Character gloss overrides (optional)

When heuristics are wrong for a headword, add entries to [`CharacterGlossOrderRules.cs`](Chinese Dictionary English glossary frequency sorter/Sorting/CharacterRules/CharacterGlossOrderRules.cs). These **pin** matching glosses first; the rest are score-sorted.

- `Simplified` (required), optional `Traditional` (`null` = any traditional form)
- `PriorityGlosses` — case-insensitive exact or `"{key} "` prefix match

Semicolon normalization (`;` → `|`) runs when glosses are joined for SQLite.

## License

### This project (tool source code)

This repository’s **source code** is under the [MIT License](LICENSE).

### CC-CEDICT data (important)

CC-CEDICT is **not** included in this repo. When you download and use it:

- License: [Creative Commons Attribution-ShareAlike 4.0](https://creativecommons.org/licenses/by-sa/4.0/)
- **Attribution:** credit [CC-CEDICT / MDBG](https://www.mdbg.net/chinese/dictionary?page=cc-cedict) in your app (e.g. About screen)
- **ShareAlike:** if you distribute the **generated** `dictionary.db` (a derivative of CC-CEDICT), you may need to allow others to use and share it under the same license — confirm with the [CC-CEDICT wiki](https://cc-cedict.org/wiki/) and your legal needs

This tool only processes data you supply locally; it does not redistribute CC-CEDICT.
