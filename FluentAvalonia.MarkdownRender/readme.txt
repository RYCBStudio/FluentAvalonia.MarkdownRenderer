FluentAvalonia.MarkdownRender - Markdown rendering for FluentAvalonia apps
=========================================================================

Thank you for installing this package. Two quick setup steps:

1. Include the control styles once, in App.axaml:

   <StyleInclude Source="avares://FluentAvalonia.MarkdownRender/Index.axaml" />

2. Add the namespace and use the control:

   xmlns:md="https://github.com/RYCBStudio/FluentAvalonia.MarkdownRender"
   <md:MarkdownRender Value="# Hello" />

NOTE: since 12.1.3.6 this package is a companion of MarkdownRenderX.Avalonia.
It now has one additional transitive dependency (MarkdownRenderX.Avalonia),
but its package id, namespaces, XAML URI, styles and InfoBar alerts are
unchanged. Existing code keeps compiling.

The companion shares one extension registry, so registering an extension
ONCE enables it for BOTH renderers:

   MarkdownRenderX.Avalonia.KaTeX    -> KaTeXExtension.Register()
   MarkdownRenderX.Avalonia.Emoji    -> EmojiExtension.Register()
   MarkdownRenderX.Avalonia.Mermaid  -> MermaidExtension.Register()

Extensions are separate NuGet packages and do nothing until registered.

Full documentation and examples:
https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer
