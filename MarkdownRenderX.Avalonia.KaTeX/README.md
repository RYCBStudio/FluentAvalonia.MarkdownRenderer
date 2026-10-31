# MarkdownRenderX.Avalonia.KaTeX

[English](README.md) | [简体中文](README.zh-CN.md)

KaTeX-style math rendering for [MarkdownRenderX.Avalonia](https://www.nuget.org/packages/MarkdownRenderX.Avalonia) — **native SkiaSharp layout, no WebView and no JavaScript runtime**.

## How it works

1. The core package already enables Markdig's Mathematics extension, so `$...$` and `$$...$$` become `MathInline` / `MathBlock` nodes.
2. This package registers an `IMarkdownNodeRenderer` (see `MarkdownRendererRegistry`) for those nodes.
3. LaTeX is laid out by [CSharpMath](https://github.com/verybadcat/CSharpMath) and drawn through Avalonia's Skia backend.
4. **Latin Modern Math is embedded in the package** (GUST Font License), so no system math font is required on Windows, Linux or macOS.

## Install

```bash
dotnet add package MarkdownRenderX.Avalonia.KaTeX
```

## Quick start

**1. Register once at startup** (e.g. `App.OnFrameworkInitializationCompleted` or `Program.Main`):

```csharp
using MarkdownRenderX.Avalonia.KaTeX;

KaTeXExtension.Register();
```

**2. Include both style files in `App.axaml`:**

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://MarkdownRenderX.Avalonia/Index.axaml" />
    <StyleInclude Source="avares://MarkdownRenderX.Avalonia.KaTeX/Index.axaml" />
</Application.Styles>
```

**3. Write math in your Markdown:**

```markdown
Inline math: $E=mc^2$ works inside a paragraph.

$$
\frac{1}{2} + \sqrt{x^2 + y^2} = \int_0^\infty e^{-t}\,dt
$$
```

That's it — existing `MarkdownRender` instances pick it up automatically.

## Syntax notes

| Input | Node | Rendering |
|---|---|---|
| `$ ... $` | `MathInline` | inline, 16px |
| `$$ ... $$` on its own lines | `MathBlock` | centered, 18px |
| `$$ ... $$` on a single line | `MathInline` (Markdig behavior) | still rendered at display size |

Invalid LaTeX is reported in place by CSharpMath as red error text; it never breaks the rest of the document because the registry swallows renderer exceptions and falls back to the built-in path.

## Customization

```csharp
// Sizes apply to content rendered afterwards
KaTeXExtension.Instance.InlineFontSize = 15;
KaTeXExtension.Instance.DisplayFontSize = 20;
KaTeXExtension.Instance.DisplayMargin = 10;   // vertical gap around display formulas
```

Formula color follows the theme through the `MdMathTextColor` resource (a `Color`). Override it in your own theme dictionaries to change it:

```xml
<ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Light">
        <Color x:Key="MdMathTextColor">#202030</Color>
    </ResourceDictionary>
    <ResourceDictionary x:Key="Dark">
        <Color x:Key="MdMathTextColor">#F0F0F5</Color>
    </ResourceDictionary>
</ResourceDictionary.ThemeDictionaries>
```

## Limitations

- CSharpMath implements the LaTeX **math** subset, not the whole KaTeX engine: matrices, fractions, roots, integrals, sums, limits, accents and Greek letters work; `\begin{align}`, `\textcolor`, custom macros and arbitrary CSS do not.
- Output is native vector text, so spacing may differ slightly from KaTeX's HTML/CSS output.
- If you need pixel-identical KaTeX output (full macro support, colors, `\htmlClass`), a WebView-based renderer is the usual alternative — deliberately **not** used here.

## Development

```bash
dotnet run --project tools/MrxSmoke -c Release   # asserts inline + block formula rendering
```

## License

- This package: MIT
- Latin Modern Math font: GUST Font License (see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md))
- CSharpMath: see its [repository](https://github.com/verybadcat/CSharpMath) for license terms
