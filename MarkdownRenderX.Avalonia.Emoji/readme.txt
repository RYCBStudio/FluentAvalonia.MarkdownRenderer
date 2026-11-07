MarkdownRenderX.Avalonia.Emoji - GitHub/GitMoji shortcodes
==========================================================

IMPORTANT: this extension does NOTHING until you register it.

Call once at application startup:

   using MarkdownRenderX.Avalonia.Emoji;
   EmojiExtension.Register();

That is all - no StyleInclude, no resource dictionary, no extra assets.

Shortcodes such as :beer:, :tada:, :+1: and :sparkles: are replaced with
native Unicode emoji. About 1900 aliases from github/gemoji and GitMoji are
built in. Text stays selectable and copyable; inline code and fenced code
blocks are never rewritten; unknown shortcodes are left verbatim.

Registration applies to both MarkdownRenderX.Avalonia and the companion
FluentAvalonia.MarkdownRender - call it once, not once per renderer.

The shortcode table is a build-time snapshot (MIT licensed data sources).
Documentation:
https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer
