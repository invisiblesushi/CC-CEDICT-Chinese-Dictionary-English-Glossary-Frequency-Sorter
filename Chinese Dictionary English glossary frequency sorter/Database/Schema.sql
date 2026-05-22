CREATE TABLE IF NOT EXISTS entries (
    simplified TEXT NOT NULL,
    traditional TEXT NOT NULL,
    pinyin TEXT NOT NULL,
    definitionsInEnglish TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_entries_simplified ON entries(simplified);
CREATE INDEX IF NOT EXISTS idx_entries_traditional ON entries(traditional);
