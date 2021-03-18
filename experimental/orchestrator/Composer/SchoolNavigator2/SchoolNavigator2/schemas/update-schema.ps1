param (
  [string]$runtime = "azurewebapp"
)
$SCHEMA_FILE="sdk.schema"
$UISCHEMA_FILE="sdk.uischema"
$BACKUP_SCHEMA_FILE="sdk-backup.schema"
$BACKUP_UISCHEMA_FILE="sdk-backup.uischema"

Write-Host "Running schema merge on $runtime runtime."

if (Test-Path $SCHEMA_FILE -PathType leaf) { Move-Item -Force -Path $SCHEMA_FILE -Destination $BACKUP_SCHEMA_FILE }
if (Test-Path $UISCHEMA_FILE -PathType leaf) { Move-Item -Force -Path $UISCHEMA_FILE -Destination $BACKUP_UISCHEMA_FILE }

bf dialog:merge "*.schema" "!sdk-backup.schema" "*.uischema" "!sdk-backup.uischema" "!sdk.override.uischema" "../runtime/$runtime/*.csproj" -o $SCHEMA_FILE

if (Test-Path $SCHEMA_FILE -PathType leaf)
{
  if (Test-Path $BACKUP_SCHEMA_FILE -PathType leaf) { Remove-Item -Force -Path $BACKUP_SCHEMA_FILE }
  if (Test-Path $BACKUP_UISCHEMA_FILE -PathType leaf) { Remove-Item -Force -Path $BACKUP_UISCHEMA_FILE }

  Write-Host "Schema merged succesfully."
  if (Test-Path $SCHEMA_FILE -PathType leaf) { Write-Host "  Schema:    $SCHEMA_FILE" }
  if (Test-Path $UISCHEMA_FILE -PathType leaf) { Write-Host "  UI Schema: $UISCHEMA_FILE" }
}
else
{
  Write-Host "Schema merge failed. Restoring previous versions."
  if (Test-Path $BACKUP_SCHEMA_FILE -PathType leaf) { Move-Item -Force -Path $BACKUP_SCHEMA_FILE -Destination $SCHEMA_FILE }
  if (Test-Path $BACKUP_UISCHEMA_FILE -PathType leaf) { Move-Item -Force -Path $BACKUP_UISCHEMA_FILE -Destination $UISCHEMA_FILE }
}
