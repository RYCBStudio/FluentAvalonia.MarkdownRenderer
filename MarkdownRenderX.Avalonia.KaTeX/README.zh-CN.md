# MarkdownRenderX.Avalonia.KaTeX

[English](README.md) | [简体中文](README.zh-CN.md)

为 [MarkdownRenderX.Avalonia](https://www.nuget.org/packages/MarkdownRenderX.Avalonia) 提供 KaTeX 风格的数学公式渲染——**SkiaSharp 原生排版，不使用 WebView，也不需要 JS 运行时**。

## 工作方式

1. 核心包已启用 Markdig 的 Mathematics 扩展，`$...$` 与 `$$...$$` 会被解析成 `MathInline` / `MathBlock` 节点；
2. 本包向 `MarkdownRendererRegistry` 注册一个 `IMarkdownNodeRenderer` 接管这些节点；
3. LaTeX 由 [CSharpMath](https://github.com/verybadcat/CSharpMath) 排版，再经 Avalonia 的 Skia 后端绘制；
4. **Latin Modern Math 字体已嵌入包内**（GUST 字体许可），Windows / Linux / macOS 均无需安装系统数学字体。

## 安装

```bash
dotnet add package MarkdownRenderX.Avalonia.KaTeX
```

## 快速开始

**1. 启动时注册一次**（如 `App.OnFrameworkInitializationCompleted` 或 `Program.Main`）：

```csharp
using MarkdownRenderX.Avalonia.KaTeX;

KaTeXExtension.Register();
```

**2. 在 `App.axaml` 中引入两个样式文件：**

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://MarkdownRenderX.Avalonia/Index.axaml" />
    <StyleInclude Source="avares://MarkdownRenderX.Avalonia.KaTeX/Index.axaml" />
</Application.Styles>
```

**3. 正常书写公式即可：**

```markdown
行内公式：$E=mc^2$，写在段落里。

$$
\frac{1}{2} + \sqrt{x^2 + y^2} = \int_0^\infty e^{-t}\,dt
$$
```

已有的 `MarkdownRender` 实例会自动生效，无需改动控件代码。

## 语法与渲染对照

| 输入 | Markdig 节点 | 渲染方式 |
|---|---|---|
| `$ ... $` | `MathInline` | 行内，16px |
| 独占多行的 `$$ ... $$` | `MathBlock` | 居中，18px |
| 单行 `$$ ... $$` | `MathInline`（Markdig 行为） | 仍按块级字号渲染 |

LaTeX 语法错误会由 CSharpMath 就地在原位显示为红色错误文本，不会中断整篇文档（注册表会吞掉渲染器异常并回退到内置逻辑）。

## 自定义

```csharp
// 字号对之后渲染的内容生效
KaTeXExtension.Instance.InlineFontSize = 15;
KaTeXExtension.Instance.DisplayFontSize = 20;
KaTeXExtension.Instance.DisplayMargin = 10;   // 块级公式上下间距
```

公式颜色通过 `MdMathTextColor`（`Color` 资源）跟随主题，可在自己的主题字典中覆盖：

```xml
<ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Light">
        <Color x:Key="MdMathTextColor">#202030</Color>
    </ResourceDictionary>
    <ResourceDictionary x:Key="Dark">
        <Color x:Key="MdMathTextColor">#F0F0F5</Color>
    </ResourceDictionary>
</ResourceDictionary.ThemeDictionaries>
```

## 限制

- CSharpMath 实现的是 LaTeX 的**数学子集**，不是完整 KaTeX 引擎：矩阵、分数、根号、积分、求和、极限、重音符号、希腊字母均支持；`\begin{align}`、`\textcolor`、自定义宏、任意 CSS 不支持。
- 输出是原生矢量文本，字距与 KaTeX 的 HTML/CSS 渲染可能略有差异。
- 若需要与 KaTeX 像素级一致（完整宏、颜色、`\htmlClass`），通常得上 WebView 方案——本包有意**不采用**。

## 开发

```bash
dotnet run --project tools/MrxSmoke -c Release   # 断言行内 + 块级公式的真实渲染
```

## 许可

- 本包：MIT
- Latin Modern Math 字体：GUST 字体许可（见 [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)）
- CSharpMath：许可证条款见其[仓库](https://github.com/verybadcat/CSharpMath)
