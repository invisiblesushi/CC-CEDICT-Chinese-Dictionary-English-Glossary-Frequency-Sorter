CREATE TABLE IF NOT EXISTS entries (
    simplified TEXT NOT NULL,
    traditional TEXT NOT NULL,
    pinyin TEXT NOT NULL,
    definitionsInEnglish TEXT NOT NULL
);

-- Only primary lookup rows (non-empty simplified) must be unique per pair.
-- Secondary forms use simplified = '' and may share traditional across source characters.
CREATE UNIQUE INDEX IF NOT EXISTS idx_entries_simplified_traditional
    ON entries(simplified, traditional)
    WHERE simplified <> '';
CREATE INDEX IF NOT EXISTS idx_entries_simplified ON entries(simplified);
CREATE INDEX IF NOT EXISTS idx_entries_traditional ON entries(traditional);
