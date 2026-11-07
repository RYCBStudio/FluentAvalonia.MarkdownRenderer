# MarkdownRenderer

[English](README.md) | [简体中文](README.zh-CN.md)

[![publish](https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer/actions/workflows/publish.yml/badge.svg)](https://github.com/RYCBStudio/FluentAvalonia.MarkdownRenderer/actions/workflows/publish.yml)

面向 [Avalonia](https://github.com/AvaloniaUI/Avalonia) 的 Markdown 渲染组件集合。同一份实现派生两个包，按你的应用是否使用 [FluentAvalonia](https://github.com/amwx/FluentAvalonia) 选用：

| 包 | 依赖 | 适用场景 | 文档 |
|---|---|---|---|
| [`FluentAvalonia.MarkdownRender`](https://www.nuget.org/packages/FluentAvalonia.MarkdownRender) | `MarkdownRenderX.Avalonia` + **FluentAvaloniaUI** | 应用已使用 FluentAvalonia，Alert 复用其 InfoBar（副包） | [README](FluentAvalonia.MarkdownRender/README.zh-CN.md) |
| [`MarkdownRenderX.Avalonia`](https://www.nuget.org/packages/MarkdownRenderX.Avalonia) | 仅 Avalonia（**无 FluentAvalonia**） | 通用 Avalonia 应用，Alert 为自绘实现 | [README](MarkdownRenderX.Avalonia/README.zh-CN.md) |
| [`MarkdownRenderX.Avalonia.KaTeX`](https://www.nuget.org/packages/MarkdownRenderX.Avalonia.KaTeX) | 上述无依赖版 + **CSharpMath** | 显示数学公式（`$...$` / `$$...$$`），不使用 WebView | [README](MarkdownRenderX.Avalonia.KaTeX/README.zh-CN.md) |
| [`MarkdownRenderX.Avalonia.Emoji`](https://www.nuget.org/packages/MarkdownRenderX.Avalonia.Emoji) | 上述无依赖版 + **emoji 短码表** | 把 `:beer:` / `:+1:` 显示为原生 Unicode emoji | [README](MarkdownRenderX.Avalonia.Emoji/README.zh-CN.md) |
| [`MarkdownRenderX.Avalonia.Mermaid`](https://www.nuget.org/packages/MarkdownRenderX.Avalonia.Mermaid) | 上述无依赖版 + **Mermaider** | 把 `mermaid` 围栏块渲染为原生 SVG 图表，不使用 WebView | [README](MarkdownRenderX.Avalonia.Mermaid/README.zh-CN.md) |

两者功能与外观对等，区别仅在于是否依赖 FluentAvalonia。

自 12.1.3.6 起，`FluentAvalonia.MarkdownRender` 是**基于 `MarkdownRenderX.Avalonia` 的副包**。两个渲染器查询同一个扩展注册表，因此 KaTeX 与 Emoji 扩展只需注册一次，对两版同时生效，无需重复注册或适配包。

版本号四段式：**前三位对齐所适配的 Avalonia 版本，末位为组件自身修订号**（如 `12.1.3.2` = 适配 Avalonia 12.1.3）。

## 仓库结构

```
FluentAvalonia.MarkdownRender/    FluentAvalonia 版组件源码与包元数据
MarkdownRenderX.Avalonia/         无 FluentAvalonia 依赖版（自绘 Alert、自带配色资源）
MarkdownRenderX.Avalonia.KaTeX/   数学公式扩展（Markdig 数学节点 → CSharpMath 排版，内置字体）
MarkdownRenderX.Avalonia.Emoji/   emoji 扩展（gemoji / GitMoji 短码 → Unicode，内置 41KB 短码表）
MarkdownRenderX.Avalonia.Mermaid/ Mermaid 扩展（围栏图表 → Mermaider SVG → Svg.Skia）
assets/                           包图标源文件（SVG）
Directory.Packages.props          中央包版本管理（CPM）
.github/workflows/publish.yml     发布流水线（打 tag 自动发全部五个包）
tools/MrxSmoke/                   无头回归探针（Alert / 本地化 / 公式 / emoji / Mermaid）
```

## 安装时提示

每个包的根目录都带一个 `readme.txt`。安装包时 Visual Studio 会自动打开它
（与 CsWin32 同一机制），因此扩展包必须调用的 `Register()` 不用翻 nuget.org
就能看到。`README.md` 仍是 NuGet 页面展示的说明（`PackageReadmeFile`），
两者互不冲突。注意只有直接安装的包才会弹窗，传递依赖不会。

## 构建与打包

```bash
dotnet pack FluentAvalonia.MarkdownRender/FluentAvalonia.MarkdownRender.csproj -c Release -o ./artifacts
dotnet pack MarkdownRenderX.Avalonia/MarkdownRenderX.Avalonia.csproj -c Release -o ./artifacts
```

## 回归验证

```bash
dotnet run --project tools/MrxSmoke -c Release
```

无头（Headless）加载组件样式字典并断言：样式可加载、自绘 Alert 的 ControlTheme 与各类型配色正确、Markdown 解析可产出控件、链接强调色资源键可解析。修改 `MarkdownRenderX.Avalonia` 的主题或 Alert 实现后建议跑一次。

## 发布

推送 `v*` 标签即触发流水线，自动打包并同时发布到 **GitHub Packages** 与 **nuget.org**：

```bash
git tag -a v12.1.3.3 -m "..." && git push origin v12.1.3.3
```

nuget.org 走[受信发布（Trusted Publishing / OIDC）](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing)，由 `NuGet/login` 用 OIDC 换取短期 API key，仓库无需配置任何 Secret。

## 许可与致谢

- 原始项目：[AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender)，MIT 许可
- 本仓库：RYCBStudio，MIT 许可（见 [LICENSE](LICENSE)）

依据 MIT 许可证要求，原始版权声明与许可声明予以保留。
