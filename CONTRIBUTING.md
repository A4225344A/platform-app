# Contributing

## Branch strategy

- `main` is always deployable. No direct pushes — all changes land via PR.
- Branch names: `feat/<short-desc>`, `fix/<short-desc>`, `chore/<short-desc>`.
- Rebase (not merge commits from `main`) to keep history linear; squash-merge PRs into `main`.

## Commit messages

Conventional, imperative mood: `feat: add readiness check for downstream dependency`,
`fix: correct shutdown timeout units`. The "why" matters more than the "what" — the diff
already shows what changed.

## Pull requests

1. Open a PR against `main`; CI (`.github/workflows/ci.yml`) must pass (build + all tests).
2. Fill in the PR template's test plan — don't skip it.
3. At least one approval required before merge (see branch protection below).
4. Squash-merge once approved and green.

## Branch protection (configure once in GitHub Settings → Branches → `main`)

These can't be expressed as repo files — set them manually after creating the repo on GitHub:

- Require a pull request before merging (no direct pushes to `main`)
- Require status checks to pass before merging → select the `build-and-test` job from CI
- Require branches to be up to date before merging
- Require at least 1 approving review (uses `.github/CODEOWNERS` if you enable
  "Require review from Code Owners")
- Do not allow force pushes or branch deletion on `main`

## Releasing an image

Pushing a `vX.Y.Z` tag (or merging to `main`) triggers `.github/workflows/docker-publish.yml`,
which builds and pushes to `ghcr.io/<owner>/platform-service`. Promoting a specific tag into
an actual environment (e.g. AWS ECR) is the responsibility of the infra/deploy repo, not this
one — see the "已知取捨" note in the W1 doc about the CI-script layer still containing a small
amount of vendor-specific glue at the deploy boundary.

## Local development

```bash
cd src/PlatformService
SERVICE_NAME=platform-service ASPNETCORE_ENVIRONMENT=Development dotnet run
```

```bash
dotnet build src/PlatformService.sln
dotnet test src/PlatformService.sln
```

```bash
docker build -t platform-service:local .
docker run --rm -p 8080:8080 platform-service:local
```
