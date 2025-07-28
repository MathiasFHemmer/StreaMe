CREATE DATABASE streame WITH ENCODING 'UTF8';

\c streame

CREATE SCHEMA IF NOT EXISTS streame;

SET search_path TO streame, public;

CREATE ROLE streame_api WITH LOGIN PASSWORD '${STREAME_API_PASSWORD}';

-- Grant full permissions on the schema
GRANT CONNECT ON DATABASE streame TO streame_api;
GRANT USAGE ON SCHEMA streame TO streame_api;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA streame TO streame_api;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA streame TO streame_api;

-- For future tables (important!)
ALTER DEFAULT PRIVILEGES IN SCHEMA streame 
GRANT ALL PRIVILEGES ON TABLES TO streame_api;

ALTER DEFAULT PRIVILEGES IN SCHEMA streame 
GRANT ALL PRIVILEGES ON SEQUENCES TO streame_api;