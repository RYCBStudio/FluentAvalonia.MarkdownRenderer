MarkdownRenderX.Avalonia.Mermaid - native Mermaid diagrams
=========================================================

IMPORTANT: this extension does NOTHING until you register it.

Call once at application startup:

   using MarkdownRenderX.Avalonia.Mermaid;
   MermaidExtension.Register();

That is all - no StyleInclude, no resource dictionary, no extra assets.

Then fenced blocks tagged mermaid are drawn as native SVG diagrams:

   ```mermaid
   flowchart LR
       A[Start] --> B[End]
   ```

Layout comes from Mermaider (pure .NET) and drawing from Svg.Skia. No
WebView, no browser, no JavaScript runtime. Flowcharts, sequence diagrams,
class/state/ER diagrams, Gantt, pie, mind maps, timelines and Kanban are
supported. Light/dark colors follow the app theme.

Optional tuning:

   MermaidExtension.Instance.Theme = MermaidTheme.Dark;
   MermaidExtension.Instance.MaxHeight = 400;

Registration applies to both MarkdownRenderX.Avalonia and the companion
FluentAvalonia.MarkdownRender - call it once, not once per renderer.

This is a native implementation, not the Mermaid.js runtime: %%{init}%%
directives, click handlers and plugins are not available. Unparsable
diagrams fall back to a normal syntax-highlighted code block.

Documentation and third-party notices:
https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer
