#!/bin/bash

BASEDIR=$(dirname "$0")
set -x
cd "$BASEDIR/../src/Quizitor.Migrator" || exit
(dotnet ef migrations remove || true)
exit