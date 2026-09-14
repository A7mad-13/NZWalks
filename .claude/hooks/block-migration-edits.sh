#!/usr/bin/env bash
# PreToolUse hook (Edit|Write|MultiEdit): block hand-edits to EF Core generated files.
input=$(cat)
file=$(printf '%s' "$input" | sed -n 's/.*"file_path"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | tr -s '\134' '/')

case "$file" in
  */NZWalks.API/Migrations/*.cs)
    reason='This file is EF Core generated (a migration or model snapshot) and must not be hand-edited. Use dotnet ef migrations add <Name> --project NZWalks.API (add --context NZWalksAuthDbContext --output-dir Migrations/NZWalksAuthDb for the auth context) - see CLAUDE.md for the exact commands.'
    printf '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"deny","permissionDecisionReason":"%s"}}\n' "$reason"
    ;;
esac
