# MarkdownRenderer

[English](README.md) | [简体中文](README.zh-CN.md)

[![publish](https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer/actions/workflows/publish.yml/badge.svg)](https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer/actions/workflows/publish.yml)

Markdown rendering components for [Avalonia](https://github.com/AvaloniaUI/Avalonia). One implementation ships as two packages — pick the one matching whether your app uses [FluentAvalonia](https://github.com/amwx/FluentAvalonia):

| Package | Dependencies | Best for | Docs |
|---|---|---|---|
| [`FluentAvalonia.MarkdownRender`](https://www.nuget.org/packages/FluentAvalonia.MarkdownRender) | Avalonia + **FluentAvaloniaUI** | Apps already using FluentAvalonia; alerts reuse its InfoBar | [README](FluentAvalonia.MarkdownRender/README.md) |
| [`MarkdownRenderX.Avalonia`](https://www.nuget.org/packages/MarkdownRenderX.Avalonia) | Avalonia only (**no FluentAvalonia**) | Any Avalonia app; alerts are drawn in-house | [README](MarkdownRenderX.Avalonia/README.md) |

Both packages behave and look the same; the only difference is the FluentAvalonia dependency. Versions are four-part: **the first three parts match the Avalonia version targeted, the last part is the component revision** (e.g. `12.1.3.3` targets Avalonia 12.1.3).

## Repository layout

```
FluentAvalonia.MarkdownRender/    FluentAvalonia-based component (source + package metadata)
MarkdownRenderX.Avalonia/         Dependency-free variant (in-house alerts, own color resources)
assets/                           Package icon sources (SVG)
Directory.Packages.props          Central package management (CPM), shared by both projects
.github/workflows/publish.yml     Release pipeline (tag push publishes both packages)
tools/MrxSmoke/                   Headless regression probe (alerts and theme resource keys)
```

## Build & pack

```bash
dotnet pack FluentAvalonia.MarkdownRender/FluentAvalonia.MarkdownRender.csproj -c Release -o ./artifacts
dotnet pack MarkdownRenderX.Avalonia/MarkdownRenderX.Avalonia.csproj -c Release -o ./artifacts
```

## Regression check

```bash
dotnet run --project tools/MrxSmoke -c Release
```

The probe loads the component style dictionary headlessly and asserts that styles load, alert `ControlTheme`s and per-kind colors resolve, Markdown parses into controls, and accent color keys resolve. Run it after touching themes or the alert implementation in `MarkdownRenderX.Avalonia`.

## Release

Pushing a `v*` tag triggers the pipeline, which packs and publishes **both** packages to GitHub Packages and nuget.org:

```bash
git tag -a v12.1.3.4 -m "..." && git push origin v12.1.3.4
```

nuget.org publishing uses [Trusted Publishing (OIDC)](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing): `NuGet/login` exchanges an OIDC token for a short-lived API key, so no secret is stored in the repository.

## License & acknowledgements

- Original project: [AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender), MIT licensed
- This repository: RYCBStudio, MIT licensed (see [LICENSE](LICENSE))

The original copyright and permission notice is retained as required by the MIT license.
