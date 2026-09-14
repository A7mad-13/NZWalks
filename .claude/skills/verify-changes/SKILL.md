---
name: verify-changes
description: Use after making code changes in NZWalks.API — before telling the user a fix or feature works — to actually confirm it, not just that it compiles. Covers build regressions, EF model/migration drift, and runtime endpoint checks. Trigger on requests like "verify this works", "did that fix it", "check your work", "make sure the API still works", or at the end of any implementation task in this repo.
---

# Verify changes in NZWalks.API

A clean `dotnet build` is not proof the change works. This repo has no test project, so verification
means: did the build get *worse*, did the EF model drift out of sync with migrations, and — if an
endpoint changed — does it actually respond correctly at runtime.

Run only the checks relevant to what changed. Report each check's pass/fail explicitly; never
summarize as "looks good" without saying what was actually checked.

## 1. Build — compare against baseline, not zero

This project currently builds with **0 errors and 50 warnings** (nullable-reference warnings in the
DTOs/repositories — pre-existing, not something to fix incidentally as part of an unrelated change).

```bash
dotnet build 2>&1 | tail -5
```

- Any error → fail, stop here.
- Warning count above 50 → new warning(s) were introduced. Find them (diff the warning list against
  the baseline set — `git stash` + rebuild is the reliable way to get a clean before/after) and either
  fix them or flag them explicitly; don't silently absorb them into "pre-existing."
- Warning count at or below 50 → pass.

## 2. EF model/migration drift — only if a Domain model or a DbContext changed

`dotnet-ef` is **not installed** in this environment by default. Check first, install once if missing:

```bash
dotnet ef --version || dotnet tool install --global dotnet-ef --version 9.0.7
```

Then check each context whose model could have moved:

```bash
# NZWalksDbContext — Difficulty/Region/Walk/Image changes
dotnet ef migrations has-pending-model-changes --project NZWalks.API

# NZWalksAuthDbContext — only if Identity-related config changed
dotnet ef migrations has-pending-model-changes --project NZWalks.API --context NZWalksAuthDbContext
```

If either reports pending changes and the diff didn't add a matching migration under `Migrations/`
(or `Migrations/NZWalksAuthDb/` for the auth context), that's a fail — the model and the database are
now out of sync. See `CLAUDE.md` for the exact `migrations add` commands per context.

## 3. Runtime check — only if a controller/route/endpoint behavior changed

Compiling and mapping a route is not the same as it returning the right response. If a DB connection
is available (user-secrets configured — see `CLAUDE.md`), actually exercise the changed endpoint:

```bash
dotnet run --project NZWalks.API &
# wait for "Now listening on:" in the output, then hit the changed endpoint, e.g.:
curl -s -o /dev/null -w "%{http_code}\n" https://localhost:<port>/api/v1/Regions
# stop the app afterward
```

Check the actual status code and body against what the change was supposed to do — a 401 on an
`[Authorize]`-protected route with no token is expected, not a failure; a 500 or an unhandled exception
is.

If no DB connection is configured, say so explicitly rather than skipping silently — the runtime check
was not performed, which is different from "runtime check passed."

## Reporting

State each check that applies, what it found, and pass/fail. If something wasn't checked (e.g. no DB
configured), say that too — never let an unstated check read as an implied pass.
