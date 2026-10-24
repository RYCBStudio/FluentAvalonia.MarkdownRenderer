# FluentAvalonia.MarkdownRender

[English](README.md) | [简体中文](README.zh-CN.md)

A Markdown rendering control for [Avalonia](https://github.com/AvaloniaUI/Avalonia) / [FluentAvalonia](https://github.com/amwx/FluentAvalonia), ready to use out of the box and following the Fluent theme and light/dark variants automatically.

> If your app does **not** use FluentAvalonia, use the sibling package [MarkdownRenderX.Avalonia](../MarkdownRenderX.Avalonia) instead —
> same features and look, without this dependency.

This component was renamed from [AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender) (MIT) and further developed; GFM table support was added on top.

## Features

- Standard Markdown: headings, paragraphs, lists, blockquotes, horizontal rules, bold/italic, inline code and links
- Code blocks: syntax highlighting via AvaloniaEdit + TextMate for dozens of languages (C/C++, C#, Java, Python, Go, Rust, TypeScript, JSON, YAML, Dockerfile, PowerShell and more), with one-click copy
- GFM tables
- GitHub-style alerts (`> [!NOTE]`, `[!TIP]`, `[!IMPORTANT]`, `[!WARNING]`, `[!CAUTION]`), backed by FluentAvalonia's InfoBar
- Images, including SVG and animated images
- Inline HTML blocks
- 5 built-in color themes: Default, OrangeHeart, Inkiness, ColorfulPurple, TechnologyBlue
- Built-in lightweight localization (English, Simplified Chinese, Traditional Chinese) driven by `CurrentUICulture`

## Versioning

The version is four-part: **the first three parts match the Avalonia version it targets, the last part is the component's own revision**.

| Package version | Targets Avalonia | Target framework | FluentAvaloniaUI |
|---|---|---|---|
| `12.1.3.x` | 12.1.3 | `net10.0` | 3.1.0 |

Main dependencies: `Avalonia` 12.1.3, `Avalonia.AvaloniaEdit` 12.0.0, `AvaloniaEdit.TextMate` 12.0.0, `Svg.Controls.Skia.Avalonia` 12.0.0.17, `FluentAvaloniaUI` 3.1.0, `Markdig` 1.4.0, `SkiaSharp` 4.153.1, `TextMateSharp` 2.0.4.

## Installation

### From nuget.org

```bash
dotnet add package FluentAvalonia.MarkdownRender
```

### From GitHub Packages

GitHub Packages requires authentication. Add a `nuget.config` at the root of your repository:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/RYCBStudio/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

The PAT needs the `read:packages` scope. Then:

```bash
dotnet add package FluentAvalonia.MarkdownRender
```

## Quick start

**1. Include the styles in `App.axaml`** (required, otherwise the control has no visual styling):

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://FluentAvalonia.MarkdownRender/Index.axaml" />
</Application.Styles>
```

**2. Use the control in your view:**

```xml
<UserControl xmlns:mdRender="https://github.com/RYCBStudio/FluentAvalonia.MarkdownRender">

    <!-- Bound -->
    <mdRender:MarkdownRender Value="{Binding MarkdownText}" />

    <!-- Or with literal content -->
    <mdRender:MarkdownRender>
        <mdRender:MarkdownRender.Value>
            # Heading
            | Column A | Column B |
            | --- | --- |
            | 1 | 2 |
        </mdRender:MarkdownRender.Value>
    </mdRender:MarkdownRender>

</UserControl>
```

`Value` is the `string` property holding the Markdown source.

## Themes

The `Default` theme is loaded automatically. To use another one, reference the corresponding theme file after including `Index.axaml`:

```
avares://FluentAvalonia.MarkdownRender/Themes/Styles/Default.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/OrangeHeart.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/Inkiness.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/ColorfulPurple.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/TechnologyBlue.axaml
```

Light and dark variants are provided by `Themes/MarkdownThemes/Light.axaml` and `Dark.axaml`, and switch automatically with the application theme variant.

## Localization

All built-in strings (alert titles, copy prompts) live in `i18n/MdStrings.cs` and are selected from `CultureInfo.CurrentUICulture`: English, Simplified Chinese (`zh-CN`) and Traditional Chinese (`zh-Hant`). An English UI culture shows English text without any configuration.

## Development

```bash
dotnet build -c Release
dotnet pack -c Release -o ./artifacts
```

## License & acknowledgements

- Original project: [AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender), MIT licensed
- This repository: RYCBStudio, MIT licensed (see [LICENSE](../LICENSE))

The original copyright and permission notice is retained as required by the MIT license.
