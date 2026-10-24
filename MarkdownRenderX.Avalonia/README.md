# MarkdownRenderX.Avalonia

[English](README.md) | [简体中文](README.zh-CN.md)

A Markdown rendering control for [Avalonia](https://github.com/AvaloniaUI/Avalonia) with **no FluentAvalonia dependency**. It works with any Avalonia theme (Fluent, Simple or your own), and follows light/dark theme variants automatically.

> This package is derived from [FluentAvalonia.MarkdownRender](https://www.nuget.org/packages/FluentAvalonia.MarkdownRender).
> Both packages offer the same features and visual design; the only difference is that this one drops the FluentAvalonia
> dependency — GitHub-style alerts are drawn in-house and accent colors come from the package's own resource keys.
> If your app already uses FluentAvalonia, prefer `FluentAvalonia.MarkdownRender`.

## Features

- Standard Markdown: headings, paragraphs, lists, blockquotes, horizontal rules, bold/italic, inline code and links
- Code blocks: syntax highlighting via AvaloniaEdit + TextMate for dozens of languages (C/C++, C#, Java, Python, Go, Rust, TypeScript, JSON, YAML, Dockerfile, PowerShell and more), with one-click copy
- GFM tables
- GitHub-style alerts (`> [!NOTE]`, `[!TIP]`, `[!IMPORTANT]`, `[!WARNING]`, `[!CAUTION]`) rendered by a built-in, dependency-free control
- Images, including SVG and animated images
- Inline HTML blocks
- 5 built-in color themes: Default, OrangeHeart, Inkiness, ColorfulPurple, TechnologyBlue
- Built-in lightweight localization (English, Simplified Chinese, Traditional Chinese) driven by `CurrentUICulture`

## Versioning

The version is four-part: **the first three parts match the Avalonia version it targets, the last part is the component's own revision**.

| Package version | Targets Avalonia | Target framework | FluentAvalonia dependency |
|---|---|---|---|
| `12.1.3.x` | 12.1.3 | `net10.0` | None |

Main dependencies: `Avalonia` 12.1.3, `Avalonia.AvaloniaEdit` 12.0.0, `AvaloniaEdit.TextMate` 12.0.0, `Svg.Controls.Skia.Avalonia` 12.0.0.17, `Markdig` 1.4.0, `SkiaSharp` 4.153.1, `TextMateSharp` 2.0.4.

## Installation

```bash
dotnet add package MarkdownRenderX.Avalonia
```

## Quick start

**1. Include the styles in `App.axaml`** (required, otherwise the control has no visual styling):

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://MarkdownRenderX.Avalonia/Index.axaml" />
</Application.Styles>
```

**2. Use the control in your view:**

```xml
<UserControl xmlns:mrx="https://github.com/RYCBStudio/MarkdownRenderX.Avalonia">

    <!-- Bound -->
    <mrx:MarkdownRender Value="{Binding MarkdownText}" />

    <!-- Or with literal content -->
    <mrx:MarkdownRender>
        <mrx:MarkdownRender.Value>
            # Heading
            | Column A | Column B |
            | --- | --- |
            | 1 | 2 |
        </mrx:MarkdownRender.Value>
    </mrx:MarkdownRender>

</UserControl>
```

`Value` is the `string` property holding the Markdown source.

## Themes

The `Default` theme is loaded automatically. To use another one, reference the corresponding theme file after including `Index.axaml`:

```
avares://MarkdownRenderX.Avalonia/Themes/Styles/Default.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/OrangeHeart.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/Inkiness.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/ColorfulPurple.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/TechnologyBlue.axaml
```

Light and dark variants are provided by `Themes/MarkdownThemes/Light.axaml` and `Dark.axaml`, and switch automatically with the application theme variant.

### Customizing colors

The package does not rely on any external theme resources — every color key is shipped with the component. Override the keys below in your application resources to fine-tune the appearance:

| Resource key | Used for |
|---|---|
| `MdAccentTextFillColorPrimaryBrush` | Link foreground, link underline |
| `MdAccentTextFillColorTertiaryBrush` | Link pressed state |
| `MdAlertBodyBrush` | Alert body text |
| `MdAlert{Kind}AccentBrush` / `MdAlert{Kind}BackgroundBrush` | Alert colors per kind, where `Kind` ∈ `Tip` / `Note` / `Warning` / `Important` / `Caution` |

## Localization

All built-in strings (alert titles, copy prompts) live in `i18n/MdStrings.cs` and are selected from `CultureInfo.CurrentUICulture`: English, Simplified Chinese (`zh-CN`) and Traditional Chinese (`zh-Hant`). No configuration required — an English UI culture shows English text.

## Development

```bash
dotnet build -c Release
dotnet pack -c Release -o ./artifacts
```

## License & acknowledgements

- Original project: [AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender), MIT licensed
- This repository: RYCBStudio, MIT licensed (see [LICENSE](../LICENSE))

Source code comments are written in Chinese; contributions in English are welcome.
