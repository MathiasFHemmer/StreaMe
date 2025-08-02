param(
    [switch]$nc
)

$COMPOSE_PROJECT_NAME = "streame"
$POSTGRES_VOLUME = "${COMPOSE_PROJECT_NAME}_pg-data"

Write-Host "Stopping all containers..." -ForegroundColor Cyan
docker compose -p $COMPOSE_PROJECT_NAME down

Write-Host "Removing PostgreSQL volume..." -ForegroundColor Cyan
docker volume rm $POSTGRES_VOLUME -f

# Conditional build command
Write-Host "Building and starting services..." -ForegroundColor Cyan

if ($nc) {
    Write-Host "Building with --no-cache..." -ForegroundColor Yellow
    docker compose -p $COMPOSE_PROJECT_NAME build --no-cache
} else {
    Write-Host "Building without --no-cache..." -ForegroundColor Yellow
    docker compose -p $COMPOSE_PROJECT_NAME build
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful, starting services..." -ForegroundColor Green
    docker compose -p $COMPOSE_PROJECT_NAME up -d
} else {
    Write-Host "Build failed, not starting services" -ForegroundColor Red
    exit $LASTEXITCODE
}