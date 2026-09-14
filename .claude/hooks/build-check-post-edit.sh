#!/usr/bin/env bash
# PostToolUse hook (Edit|Write|MultiEdit): after a *.cs edit under NZWalks.API, rebuild and
# surface only compiler ERRORS (this repo has 50 pre-existing nullable-reference warnings
# that are noise, so warnings are intentionally not checked here).
input=$(cat)
file=$(printf '%s' "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | tr -s '\134' '/')

json_escape() {
  sed -e 's/\\/\\\\/g' -e 's/"/\\"/g' | sed -e ':a' -e 'N' -e '$!ba' -e 's/\n/\\n/g'
}

case "$file" in
  */NZWalks.API/*.cs)
    cd /c/NZWalks || exit 0
    errors=$(dotnet build 2>&1 | grep ': error ')
    if [ -n "$errors" ]; then
      count=$(printf '%s\n' "$errors" | wc -l | tr -d ' ')
      summary=$(printf '%s\n' "$errors" | sed -n '1,20p')
      reason="dotnet build has $count compiler error(s) after this edit:
$summary"
      esc=$(printf '%s' "$reason" | json_escape)
      printf '{"decision":"block","reason":"%s"}\n' "$esc"
    fi
    ;;
esac
