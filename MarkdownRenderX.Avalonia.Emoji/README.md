# MarkdownRenderX.Avalonia.Emoji

[English](README.md) | [简体中文](README.zh-CN.md)

GitHub / GitMoji shortcode support for [MarkdownRenderX.Avalonia](https://www.nuget.org/packages/MarkdownRenderX.Avalonia). Turns `:beer:`, `:tada:`, `:+1:` and ~1900 other shortcodes into **native Unicode emoji** — selectable text, no images, no WebView, no style files.

## Install

```bash
dotnet add package MarkdownRenderX.Avalonia.Emoji
```

## Quick start

Register once at startup (e.g. `App.OnFrameworkInitializationCompleted` or `Program.Main`):

```csharp
using MarkdownRenderX.Avalonia.Emoji;

EmojiExtension.Register();
```

That's all — no `StyleInclude`, no extra style file. All `MarkdownRender` instances pick it up. Since 12.1.3.6 the companion `FluentAvalonia.MarkdownRender` package shares the same extension registry, so one `Register()` call covers both renderers:

```markdown
Ship it! :beer: :tada: :+1:
```

renders as 🍺 🎉 👍.

## Data

- The shortcode table is generated from [github/gemoji](https://github.com/github/gemoji)'s `db/emoji.json` (all aliases).
- [GitMoji](https://gitmoji.dev/) shortcodes were merged as well; every GitMoji code was already covered by gemoji, so it contributes no duplicates.
- **1913 shortcodes** total, shipped as a compact 41 KB embedded resource (~41 KB to the package), loaded lazily on first use.
- Deserialization uses a source-generated `JsonSerializerContext`, so it works under AOT / trimming.

## Behavior

| Input | Result |
|---|---|
| `:beer:` in normal text | 🍺 |
| `:BEER:` | 🍺 (case-insensitive) |
| `:+1:` / `:-1:` | 👍 / 👎 |
| `` `:beer:` `` (inline code) | unchanged |
| `:beer:` inside a fenced code block | unchanged |
| `:definitely_not_an_emoji_xyz:` | unchanged (unknown codes are left as-is) |

Only `LiteralInline` nodes are touched. Code inlines and fenced code blocks are different Markdig node types, so they are **never** rewritten. The replacement returns a `Run`, so the result stays selectable and copyable text — not an embedded control.

> Shortcodes are matched with `:([a-z0-9_+\-]+):`. Times like `12:30:` and URLs are safe: unknown codes are kept verbatim.

## Combining with other extensions

Registration is global and additive, so it works alongside the KaTeX math extension:

```csharp
using MarkdownRenderX.Avalonia.KaTeX;
using MarkdownRenderX.Avalonia.Emoji;

KaTeXExtension.Register();
EmojiExtension.Register();
```

`EmojiExtension.Unregister()` removes the renderer again. `EmojiExtension.ShortcodeCount` reports the loaded table size (useful for diagnostics).

## Limitations

- Emoji glyphs come from the operating system's emoji font, so their appearance varies by platform — this is a text-only renderer by design.
- Custom shortcodes are not supported yet.
- Skin-tone modifiers (`:+1::skin-tone-2:`) are not expanded.

## License

- This package: MIT
- Shortcode data: MIT (gemoji / GitMoji) — see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)
- Emoji glyphs are rendered by the OS font; no font is redistributed.
