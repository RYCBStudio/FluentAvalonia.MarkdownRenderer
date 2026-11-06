# MarkdownRenderX.Avalonia.Emoji

[English](README.md) | [简体中文](README.zh-CN.md)

为 [MarkdownRenderX.Avalonia](https://www.nuget.org/packages/MarkdownRenderX.Avalonia) 提供 GitHub / GitMoji 短码支持。`:beer:`、`:tada:`、`:+1:` 等约 1900 个短码会被转成**原生 Unicode emoji**——文本可选中可复制，无图片、无 WebView、无需引入样式文件。

## 安装

```bash
dotnet add package MarkdownRenderX.Avalonia.Emoji
```

## 快速开始

启动时注册一次（如 `App.OnFrameworkInitializationCompleted` 或 `Program.Main`）：

```csharp
using MarkdownRenderX.Avalonia.Emoji;

EmojiExtension.Register();
```

不需要 `StyleInclude`，也没有额外样式文件。所有 `MarkdownRender` 实例自动生效。自 12.1.3.6 起，副包 `FluentAvalonia.MarkdownRender` 共用同一个扩展注册表，调用一次 `Register()` 即对两个渲染器生效：

```markdown
Ship it! :beer: :tada: :+1:
```

渲染为 🍺 🎉 👍。

## 数据来源

- 短码表由 [github/gemoji](https://github.com/github/gemoji) 的 `db/emoji.json`（全部 aliases）生成；
- 同时合并了 [GitMoji](https://gitmoji.dev/) 的短码；经比对，GitMoji 的全部 code 已被 gemoji 覆盖，因此没有产生重复条目；
- 共 **1913** 个短码，压缩为约 41 KB 的嵌入资源，首次使用时懒加载；
- 反序列化使用源生成的 `JsonSerializerContext`，兼容 AOT / trimming。

## 行为对照

| 输入 | 结果 |
|---|---|
| 正文中的 `:beer:` | 🍺 |
| `:BEER:` | 🍺（大小写不敏感） |
| `:+1:` / `:-1:` | 👍 / 👎 |
| 行内代码 `` `:beer:` `` | 不替换 |
| 围栏代码块内的 `:beer:` | 不替换 |
| `:definitely_not_an_emoji_xyz:` | 原样保留（未知短码不处理） |

只处理 `LiteralInline` 节点。行内代码与围栏代码块是不同的 Markdig 节点类型，**永远不会**被改写。替换结果返回 `Run`，因此仍是可选中、可复制的文本，而不是嵌入控件。

> 短码匹配规则为 `:([a-z0-9_+\-]+):`。`12:30:` 这类时间、URL 中的冒号不会被误伤：未知短码一律原样保留。

## 与其它扩展共存

注册是全局且可叠加的，可与 KaTeX 公式扩展同时使用：

```csharp
using MarkdownRenderX.Avalonia.KaTeX;
using MarkdownRenderX.Avalonia.Emoji;

KaTeXExtension.Register();
EmojiExtension.Register();
```

`EmojiExtension.Unregister()` 可注销渲染器；`EmojiExtension.ShortcodeCount` 返回已加载的短码数量，便于诊断。

## 限制

- emoji 字形来自操作系统字体，各平台外观不同——这是纯文本渲染方案的固有特点；
- 暂不支持自定义短码；
- 不展开肤色修饰符（`:+1::skin-tone-2:`）。

## 许可

- 本包：MIT
- 短码数据：MIT（gemoji / GitMoji），见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)
- emoji 字形由系统字体提供，未随包分发任何字体。
