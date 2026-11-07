MarkdownRenderX.Avalonia - Markdown rendering for Avalonia
==========================================================

Thank you for installing this package. Two quick setup steps:

1. Include the control styles once, in App.axaml:

   <StyleInclude Source="avares://MarkdownRenderX.Avalonia/Index.axaml" />

2. Add the namespace and use the control:

   xmlns:md="https://github.com/RYCBStudio/MarkdownRenderX.Avalonia"
   <md:MarkdownRender Value="# Hello" />

Supported Markdown: headings, lists, blockquotes, GFM tables, GitHub-style
alerts, images, inline HTML and syntax-highlighted code blocks. The core
package also supports inline math delimiters, subscripts (H~2~O),
superscripts (X^2^), highlights (==key==), strikethrough and insertion;
math rendering itself needs the KaTeX extension below.

OPTIONAL EXTENSIONS - separate NuGet packages. IMPORTANT: an extension does
NOTHING until you call Register() once at application startup:

   MarkdownRenderX.Avalonia.KaTeX    -> KaTeXExtension.Register()
   MarkdownRenderX.Avalonia.Emoji    -> EmojiExtension.Register()
   MarkdownRenderX.Avalonia.Mermaid  -> MermaidExtension.Register()

One registration applies to both this renderer and the companion package
FluentAvalonia.MarkdownRender.

Full documentation and examples:
https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer
