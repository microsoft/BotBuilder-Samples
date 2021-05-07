#!/usr/bin/env bash
SCHEMA_FILE=sdk.schema
UISCHEMA_FILE=sdk.uischema
BACKUP_SCHEMA_FILE=sdk-backup.schema
BACKUP_UISCHEMA_FILE=sdk-backup.uischema

while [ $# -gt 0 ]; do
   if [[ $1 == *"-"* ]]; then
      param="${1/-/}"
      declare $param="$2"
   fi
  shift
done

echo "Running schema merge."
[ -f "$SCHEMA_FILE" ] && mv "./$SCHEMA_FILE" "./$BACKUP_SCHEMA_FILE"
[ -f "$UISCHEMA_FILE" ] && mv "./$UISCHEMA_FILE" "./$BACKUP_UISCHEMA_FILE"

bf dialog:merge "*.schema" "!**/sdk-backup.schema" "*.uischema" "!**/sdk-backup.uischema" "!**/sdk.override.uischema" "!**/generated" "../*.csproj" "../package.json" -o $SCHEMA_FILE

if [ -f "$SCHEMA_FILE" ]; then
  rm -rf "./$BACKUP_SCHEMA_FILE"
  rm -rf "./$BACKUP_UISCHEMA_FILE"
  echo "Schema merged succesfully."
  [ -f "$SCHEMA_FILE" ] && echo "  Schema:    $SCHEMA_FILE"
  [ -f "$UISCHEMA_FILE" ] && echo "  UI Schema: $UISCHEMA_FILE"
else
  echo "Schema merge failed.  Restoring previous versions."
  [ -f "$BACKUP_SCHEMA_FILE" ] && mv "./$BACKUP_SCHEMA_FILE" "./$SCHEMA_FILE"
  [ -f "$BACKUP_UISCHEMA_FILE" ] && mv "./$BACKUP_UISCHEMA_FILE" "./$UISCHEMA_FILE"
fi
