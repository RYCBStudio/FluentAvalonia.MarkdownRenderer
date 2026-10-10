# MarkdownRenderX.Avalonia

面向 [Avalonia](https://github.com/AvaloniaUI/Avalonia) 的 Markdown 渲染控件，**不依赖 FluentAvalonia**，可直接用于任何 Avalonia 应用（Fluent / Simple / 自定义主题均可），自动跟随明暗主题切换。

> 本包自 [FluentAvalonia.MarkdownRender](https://www.nuget.org/packages/FluentAvalonia.MarkdownRender) 派生，二者功能与外观对等；
> 区别仅在于本包移除了 FluentAvalonia 依赖（GitHub 风格 Alert 改为自绘实现，主题色改为组件自带资源键）。
> 若你的应用本来就使用 FluentAvalonia，可直接选用 `FluentAvalonia.MarkdownRender`。

## 特性

- 标准 Markdown：标题、段落、列表、引用、分割线、粗斜体、行内代码、链接
- 代码块：基于 AvaloniaEdit + TextMate 的语法高亮，覆盖 C/C++、C#、Java、Python、Go、Rust、TypeScript、JSON、YAML、Dockerfile、PowerShell 等数十种语言，附带一键复制
- GFM 表格渲染
- GitHub 风格警告框（Alert）：`> [!NOTE]` / `[!TIP]` / `[!IMPORTANT]` / `[!WARNING]` / `[!CAUTION]`，内置自绘实现，无需任何第三方 UI 库
- 图片渲染（含 SVG、动图）
- 内联 HTML 块渲染
- 5 套内置配色主题：Default、OrangeHeart、Inkiness、ColorfulPurple、TechnologyBlue
- 内置轻量多语言（zh-CN / zh-Hant / en-US），随 `CurrentUICulture` 自动切换

## 版本与依赖对照

版本号采用四段式：**前三位对齐所适配的 Avalonia 版本，末位是组件自身修订号**。

| 包版本 | 适配 Avalonia | 目标框架 | FluentAvalonia 依赖 |
|---|---|---|---|
| `12.1.3.x` | 12.1.3 | `net10.0` | 无 |

主要依赖：`Avalonia` 12.1.3、`Avalonia.AvaloniaEdit` 12.0.0、`AvaloniaEdit.TextMate` 12.0.0、`Svg.Controls.Skia.Avalonia` 12.0.0.17、`Markdig` 1.4.0、`SkiaSharp` 4.151.2、`TextMateSharp` 2.0.4。

## 安装

```bash
dotnet add package MarkdownRenderX.Avalonia
```

## 快速开始

**1. 在 `App.axaml` 中引入样式**（必须，否则控件无视觉外观）：

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://MarkdownRenderX.Avalonia/Index.axaml" />
</Application.Styles>
```

**2. 在页面中使用控件**：

```xml
<UserControl xmlns:mrx="https://github.com/RYCBStudio/MarkdownRenderX.Avalonia">

    <!-- 绑定 -->
    <mrx:MarkdownRender Value="{Binding MarkdownText}" />

    <!-- 或直接写字面量 -->
    <mrx:MarkdownRender>
        <mrx:MarkdownRender.Value>
            # 标题
            | 列 A | 列 B |
            | --- | --- |
            | 1 | 2 |
        </mrx:MarkdownRender.Value>
    </mrx:MarkdownRender>

</UserControl>
```

`Value` 为 `string` 类型的 Markdown 源文本。

## 主题

默认加载 `Default` 主题。若需切换，可在引入 `Index.axaml` 之后引用所需主题文件：

```
avares://MarkdownRenderX.Avalonia/Themes/Styles/Default.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/OrangeHeart.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/Inkiness.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/ColorfulPurple.axaml
avares://MarkdownRenderX.Avalonia/Themes/Styles/TechnologyBlue.axaml
```

明暗色由 `Themes/MarkdownThemes/Light.axaml` 与 `Dark.axaml` 提供，跟随应用主题变体自动切换。

### 自定义配色

组件不依赖外部主题资源，所有颜色键都由组件自带。若需微调，在你的应用资源里覆盖同名键即可：

| 资源键 | 用途 |
|---|---|
| `MdAccentTextFillColorPrimaryBrush` | 链接前景色、链接下划线 |
| `MdAccentTextFillColorTertiaryBrush` | 链接按下态 |
| `MdAlertBodyBrush` | 警告框正文 |
| `MdAlert{Kind}AccentBrush` / `MdAlert{Kind}BackgroundBrush` | 警告框各类型配色，`Kind` ∈ `Tip` / `Note` / `Warning` / `Important` / `Caution` |

## 本地化

控件内置字符串（警告框标题、复制提示等）位于 `i18n/MdStrings.cs`，依据 `CultureInfo.CurrentUICulture` 在 zh-CN / zh-Hant / en-US 之间自动选择，无需额外配置。

## 开发

```bash
dotnet build -c Release
dotnet pack -c Release -o ./artifacts
```

## 许可与致谢

- 原始项目：[AIDotNet/Markdown.AIRender](https://github.com/AIDotNet/Markdown.AIRender)，MIT 许可
- 本仓库：RYCBStudio，MIT 许可（见 [LICENSE](../LICENSE)）
