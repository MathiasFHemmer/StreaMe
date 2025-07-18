$COMPOSE_PROJECT_NAME = "streame"
$POSTGRES_VOLUME = "${COMPOSE_PROJECT_NAME}_pg-data"

Write-Host "Stopping all containers"
docker compose -p $COMPOSE_PROJECT_NAME down

Write-Host "Removing PostgreSQL volume"
docker volume rm $POSTGRES_VOLUME

Write-Host "Starting"
docker compose -p $COMPOSE_PROJECT_NAME up -d --build
