#!/usr/bin/env bash
# Stop hook: deterministic backstop - refuse to end the turn if the solution has compiler
# errors. This repo is expected to build cleanly, so this blocks on ANY compiler error,
# regardless of whether it predates this session.
cd /c/NZWalks || exit 0

json_escape() {
  sed -e 's/\\/\\\\/g' -e 's/"/\\"/g' | sed -e ':a' -e 'N' -e '$!ba' -e 's/\n/\\n/g'
}

errors=$(dotnet build NZWalks.sln 2>&1 | grep ': error ')
if [ -n "$errors" ]; then
  count=$(printf '%s\n' "$errors" | wc -l | tr -d ' ')
  summary=$(printf '%s\n' "$errors" | sed -n '1,20p')
  reason="Build has $count compiler error(s) - fix before ending the turn:
$summary"
  esc=$(printf '%s' "$reason" | json_escape)
  printf '{"continue":false,"stopReason":"%s"}\n' "$esc"
fi
