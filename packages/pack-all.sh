#!/bin/bash

BASEDIR=$(dirname "$0")
SUFFIX="$(date -u +%Y%m%d%H%M)"

echo "$BASEDIR" && \

(rm -rf "$BASEDIR/tr.lplus" || :) && \
dotnet pack "$BASEDIR/../../trlplus/TR.LPlus.sln" -o "$BASEDIR/tr.lplus" -c Release --version-suffix "$SUFFIX"