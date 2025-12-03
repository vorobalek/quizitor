#!/bin/bash

BASEDIR=$(dirname "$0")
set -x
MIGRATION_NAME=$1
cd "$BASEDIR/../src/Quizitor.Migrator" || exit
(dotnet ef migrations add "ef_$MIGRATION_NAME" || true)
exit