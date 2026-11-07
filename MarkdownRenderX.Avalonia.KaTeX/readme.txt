MarkdownRenderX.Avalonia.KaTeX - native math for MarkdownRenderX
===============================================================

IMPORTANT: this extension does NOTHING until you register it.

1. Call once at application startup:

   using MarkdownRenderX.Avalonia.KaTeX;
   KaTeXExtension.Register();

2. Include BOTH style dictionaries in App.axaml:

   <StyleInclude Source="avares://MarkdownRenderX.Avalonia/Index.axaml" />
   <StyleInclude Source="avares://MarkdownRenderX.Avalonia.KaTeX/Index.axaml" />

Now inline $E=mc^2$ and display $$...$$ are typeset natively by CSharpMath
through SkiaSharp. No WebView, no browser, no JavaScript, no system math
font required (Latin Modern Math is embedded, GUST Font License).

Registration applies to both MarkdownRenderX.Avalonia and the companion
FluentAvalonia.MarkdownRender - call it once, not once per renderer.

Supported syntax is the LaTeX math subset (fractions, roots, integrals,
sums, matrices, Greek letters). Mermaid-style \begin{align} and custom
macros are NOT supported.

Documentation and third-party notices:
https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer
