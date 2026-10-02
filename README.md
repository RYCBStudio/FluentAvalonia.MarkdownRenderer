# FluentAvalonia.MarkdownRender

面向 [Avalonia](https://github.com/AvaloniaUI/Avalonia) / [FluentAvalonia](https://github.com/amwx/FluentAvalonia) 的 Markdown 渲染控件，开箱即用，自动跟随 Fluent 主题与明暗色切换。

本组件由 [AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender)（MIT）重命名并二次开发而来，新增了 GFM 表格渲染等能力。

## 特性

- 标准 Markdown：标题、段落、列表、引用、分割线、粗斜体、行内代码、链接
- 代码块：基于 AvaloniaEdit + TextMate 的语法高亮，覆盖 C/C++、C#、Java、Python、Go、Rust、TypeScript、JSON、YAML、Dockerfile、PowerShell 等数十种语言，附带一键复制
- GFM 表格渲染
- GitHub 风格警告框（Alert）：`> [!NOTE]` / `[!TIP]` / `[!IMPORTANT]` / `[!WARNING]` / `[!CAUTION]`
- 图片渲染（含 SVG、动图）
- 内联 HTML 块渲染
- 5 套内置配色主题：Default、OrangeHeart、Inkiness、ColorfulPurple、TechnologyBlue
- 内置轻量多语言（zh-CN / zh-Hant / en-US），随 `CurrentUICulture` 自动切换

## 版本与依赖对照

版本号采用四段式：**前三位对齐所适配的 Avalonia 版本，末位是组件自身修订号**。

| 包版本 | 适配 Avalonia | 目标框架 | FluentAvaloniaUI |
|---|---|---|---|
| `12.1.3.x` | 12.1.3 | `net10.0` | 3.1.0 |

主要依赖：`Avalonia` 12.1.3、`Avalonia.AvaloniaEdit` 12.0.0、`AvaloniaEdit.TextMate` 12.0.0、`Svg.Controls.Skia.Avalonia` 12.0.0.17、`FluentAvaloniaUI` 3.1.0、`Markdig` 1.4.0、`SkiaSharp` 4.151.2、`TextMateSharp` 2.0.4。

## 安装

### 从 nuget.org

```bash
dotnet add package FluentAvalonia.MarkdownRender
```

### 从 GitHub Packages

GitHub Packages 需要身份验证，先在仓库根目录添加 `nuget.config`：

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/RYCBStudio/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

PAT 需要具备 `read:packages` 权限。然后：

```bash
dotnet add package FluentAvalonia.MarkdownRender
```

## 快速开始

**1. 在 `App.axaml` 中引入样式**（必须，否则控件无视觉外观）：

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://FluentAvalonia.MarkdownRender/Index.axaml" />
</Application.Styles>
```

**2. 在页面中使用控件**：

```xml
<UserControl xmlns:mdRender="https://github.com/RYCBStudio/FluentAvalonia.MarkdownRender">

    <!-- 绑定 -->
    <mdRender:MarkdownRender Value="{Binding MarkdownText}" />

    <!-- 或直接写字面量 -->
    <mdRender:MarkdownRender>
        <mdRender:MarkdownRender.Value>
            # 标题
            | 列 A | 列 B |
            | --- | --- |
            | 1 | 2 |
        </mdRender:MarkdownRender.Value>
    </mdRender:MarkdownRender>

</UserControl>
```

`Value` 为 `string` 类型的 Markdown 源文本。

## 主题

默认加载 `Default` 主题。若需切换，可在引入 `Index.axaml` 之后覆盖 `Themes/Styles/_index.axaml` 中的 `StyleInclude`，或直接在应用中引用所需的主题文件：

```
avares://FluentAvalonia.MarkdownRender/Themes/Styles/Default.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/OrangeHeart.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/Inkiness.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/ColorfulPurple.axaml
avares://FluentAvalonia.MarkdownRender/Themes/Styles/TechnologyBlue.axaml
```

明暗色由 `Themes/MarkdownThemes/Light.axaml` 与 `Dark.axaml` 提供，跟随应用主题变体自动切换。

## 本地化

控件内置字符串（警告框标题、复制提示等）位于 `i18n/MdStrings.cs`，依据 `CultureInfo.CurrentUICulture` 在 zh-CN / zh-Hant / en-US 之间自动选择，无需额外配置。如需覆盖，直接修改该文件或在你的应用中另建同名资源即可。

## 开发

```bash
dotnet build -c Release
dotnet pack -c Release -o ./artifacts
```

发布由 GitHub Actions 驱动：推送 `v*` 标签后自动打包，并推送到 GitHub Packages 与 nuget.org。nuget.org 走[受信发布（Trusted Publishing / OIDC）](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing)，由 `NuGet/login` 用 OIDC 换取短期 API key，无需在仓库里配置任何 Secret。

## 许可与致谢

- 原始项目：[AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender)，MIT 许可
- 本仓库：RYCBStudio，MIT 许可（见 [LICENSE](LICENSE)）

依据 MIT 许可证要求，原始版权声明与许可声明予以保留。
