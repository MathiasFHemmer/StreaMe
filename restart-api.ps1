$COMPOSE_PROJECT_NAME = "streame"
$POSTGRES_VOLUME = "${COMPOSE_PROJECT_NAME}_pg-data"

Write-Host "Restarting and Rebuilding API"
docker compose -p $COMPOSE_PROJECT_NAME rm -fs streame; docker compose -p $COMPOSE_PROJECT_NAME build streame; docker compose -p $COMPOSE_PROJECT_NAME up -d streame
