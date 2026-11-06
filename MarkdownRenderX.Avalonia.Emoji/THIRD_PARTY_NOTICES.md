# Third-party notices

## Shortcode data (`Resources/emoji-map.json`)

This file is a **derived, compacted** dataset: only `shortcode -> Unicode emoji` pairs are kept.

Sources:

| Project | Source file | License |
|---|---|---|
| [github/gemoji](https://github.com/github/gemoji) | `db/emoji.json` | MIT |
| [carloscuesta/gitmoji](https://github.com/carloscuesta/gitmoji) | `packages/gitmojis/src/gitmojis.json` | MIT |

Both projects provide their data under the MIT License. The generated table contains **1913** shortcodes:
all aliases from gemoji, plus GitMoji codes for anything not already covered (in practice GitMoji added
no new entries because gemoji already contains every GitMoji code). Where both sources defined the same
code, gemoji's value was kept (the only differences were variation selectors, e.g. `⚡` vs `⚡️`).

The emoji characters themselves are Unicode code points defined by the Unicode Standard; the glyphs are
rendered by the operating system's emoji font. **No font or image is redistributed by this package.**

Full license texts are available in the source repositories linked above.
