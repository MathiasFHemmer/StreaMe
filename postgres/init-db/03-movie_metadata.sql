\c streame
SET search_path TO streame, public;

BEGIN;

CREATE TABLE movie_metadata (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    length_minutes INTEGER NOT NULL 
        CHECK (length_minutes > 0),
    file_name VARCHAR(255) NOT NULL,
    file_location TEXT NOT NULL,
    movie_id UUID REFERENCES movies(id),
    status SMALLINT NOT NULL 
        CHECK (status IN (1, 2, 3)),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_movie_metadata_movie_id ON movie_metadata(movie_id);

CREATE INDEX idx_movie_metadata_status ON movie_metadata(status);

CREATE OR REPLACE FUNCTION update_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_movie_metadata_updated_at
BEFORE UPDATE ON movie_metadata
FOR EACH ROW
EXECUTE FUNCTION update_updated_at();

COMMIT;