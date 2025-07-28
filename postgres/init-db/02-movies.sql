\c streame
SET search_path TO streame, public;

BEGIN;

CREATE TABLE movies (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(512) NOT NULL 
        CHECK (title <> ''),
    release_year SMALLINT NULL 
        CHECK (release_year IS NULL OR (release_year BETWEEN 1880 AND EXTRACT(YEAR FROM CURRENT_DATE) + 5)),
    description TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_movies_title ON movies(title);

CREATE INDEX idx_movies_release_year ON movies(release_year);

CREATE OR REPLACE FUNCTION update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_movies_updated_at
BEFORE UPDATE ON movies
FOR EACH ROW
EXECUTE FUNCTION update_updated_at();

COMMIT;