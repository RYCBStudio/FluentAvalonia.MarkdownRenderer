using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Headless;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Svg.Skia;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MarkdownRenderX.Avalonia.Controls.MarkdownRender;
using MarkdownRenderX.Avalonia.Extensibility;
using MarkdownRenderX.Avalonia.KaTeX;
using MarkdownRenderX.Avalonia.Emoji;
using MarkdownRenderX.Avalonia.Mermaid;
using MathView = CSharpMath.Avalonia.MathView;
using FamrMarkdownRender = FluentAvalonia.MarkdownRender.Controls.MarkdownRender.MarkdownRender;
using MermaidRenderOptions = Mermaider.Models.RenderOptions;

namespace MrxSmoke;

/// <summary>
///     MarkdownRenderX.Avalonia 无头冒烟探针：
///     验证组件样式字典可加载、自绘 Alert 的 ControlTheme 与配色资源可解析、Markdown 解析可产出控件。
/// </summary>
internal static class Program
{
    private static int _failures;

    [STAThread]
    public static int Main()
    {
        AppBuilder.Configure<SmokeApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        try
        {
            CheckStyleDictionaryLoaded();
            CheckAlertThemeAndColors();
            CheckMarkdownRendersAlert();
            CheckLinkAccentResolved();
            CheckLocalization();
            CheckKaTeX();
            CheckEmoji();
            CheckFamrExtensions();
            CheckMermaid();
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXCEPTION: " + ex);
            _failures++;
        }

        Console.WriteLine(_failures == 0 ? "SMOKE OK" : $"SMOKE FAILED ({_failures})");
        return _failures;
    }

    private static void CheckStyleDictionaryLoaded()
    {
        var hasIndex = SmokeApp.Instance.Styles.OfType<Styles>().Any();
        Assert(hasIndex, "Index.axaml 已作为 Styles 加载");
    }

    private static void CheckAlertThemeAndColors()
    {
        var alert = MdAlertBox.Create("warning", "这是一条警告");
        var window = new Window { Width = 600, Height = 400, Content = alert };
        window.Show();
        Pump();

        var accent = alert.BorderBrush as ISolidColorBrush;
        var background = alert.Background as ISolidColorBrush;

        Assert(alert.Title.Length > 0, "Alert 标题非空", alert.Title);
        Assert(accent != null, "Alert BorderBrush 已解析（ControlTheme 生效）");
        Assert(background != null, "Alert Background 已解析");
        Assert(accent != null && accent.Color == Color.Parse("#9D5D00"), "warning 强调色 = #9D5D00",
            accent?.Color.ToString());
        Assert(background != null && background.Color == Color.Parse("#FDF5EC"), "warning 背景色 = #FDF5EC",
            background?.Color.ToString());

        var tip = MdAlertBox.Create("tip", "提示");
        var caution = MdAlertBox.Create("caution", "注意");
        var w2 = new Window { Width = 600, Height = 400, Content = new StackPanel { Children = { tip, caution } } };
        w2.Show();
        Pump();
        Assert((tip.BorderBrush as ISolidColorBrush)?.Color == Color.Parse("#0F7B0F"), "tip 强调色 = #0F7B0F",
            (tip.BorderBrush as ISolidColorBrush)?.Color.ToString());
        Assert((caution.BorderBrush as ISolidColorBrush)?.Color == Color.Parse("#C42B1C"), "caution 强调色 = #C42B1C",
            (caution.BorderBrush as ISolidColorBrush)?.Color.ToString());
    }

    private static void CheckMarkdownRendersAlert()
    {
        const string markdown = """
                                # 标题

                                > [!TIP]
                                > 提示内容

                                > [!CAUTION]
                                > 危险内容

                                | 列A | 列B |
                                | --- | --- |
                                | 1 | 2 |
                                """;

        var render = new MarkdownRender { Value = markdown };
        var window = new Window { Width = 900, Height = 700, Content = render };
        window.Show();
        Pump(2000);

        var alerts = render.GetVisualDescendants().OfType<MdAlertBox>().ToList();
        var descendants = render.GetVisualDescendants().Count();
        Assert(descendants > 3, "Markdown 解析产出可视子控件", descendants.ToString());
        Assert(alerts.Count == 2, "警报框渲染数量 = 2", alerts.Count.ToString());
        if (alerts.Count == 2)
        {
            Assert((alerts[0].BorderBrush as ISolidColorBrush)?.Color == Color.Parse("#0F7B0F"),
                "TIP 警报使用 tip 配色", (alerts[0].BorderBrush as ISolidColorBrush)?.Color.ToString());
            Assert((alerts[1].BorderBrush as ISolidColorBrush)?.Color == Color.Parse("#C42B1C"),
                "CAUTION 警报使用 caution 配色", (alerts[1].BorderBrush as ISolidColorBrush)?.Color.ToString());
        }
    }

    private static void CheckLinkAccentResolved()
    {
        var render = new MarkdownRender { Value = "访问 [链接](https://example.com) 了解详情。" };
        var window = new Window { Width = 900, Height = 400, Content = render };
        window.Show();
        Pump(1500);

        var link = render.GetVisualDescendants().OfType<MarkdownLink>().FirstOrDefault();
        Assert(link != null, "链接控件已渲染");
        var accent = link?.GetValue(TextElement.ForegroundProperty) as ISolidColorBrush;
        Assert(accent != null, "链接强调色资源键已解析（MdAccentTextFillColorPrimaryBrush）",
            accent?.Color.ToString());
    }

    /// <summary>
    ///     验证运行时本地化：英文 / 简体 / 繁体三套 Alert 标题，且 IMPORTANT 与 CAUTION 文案不重复
    /// </summary>
    private static void CheckLocalization()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            Check("en-US", "note", "Note");
            Check("en-US", "tip", "Tip");
            Check("en-US", "warning", "Warning");
            Check("en-US", "important", "Important");
            Check("en-US", "caution", "Caution");

            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
            Check("zh-CN", "note", "注意");
            Check("zh-CN", "warning", "警告");
            Check("zh-CN", "important", "重要");
            Check("zh-CN", "caution", "谨慎");

            CultureInfo.CurrentUICulture = new CultureInfo("zh-TW");
            Check("zh-TW", "note", "注意");
            Check("zh-TW", "info", "資訊");
            Check("zh-TW", "caution", "謹慎");
            Check("zh-TW", "important", "重要");
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }

    private static void Check(string culture, string kind, string expected)
    {
        var actual = MdAlertBox.Create(kind, "x").Title;
        Assert(actual == expected, $"{culture}: {kind.ToUpperInvariant()} 标题", actual);
    }

    /// <summary>
    ///     验证 KaTeX 扩展：注册后行内与块级公式都应渲染成 MathView，且排版无错误
    /// </summary>
    private static void CheckKaTeX()
    {
        KaTeXExtension.Register();
        Assert(MarkdownRendererRegistry.Count >= 1, "KaTeX 渲染器已注册",
            MarkdownRendererRegistry.Count.ToString());

        const string markdown = """
                                # 公式

                                质能方程 $E=mc^2$ 是相对论的核心。

                                $$\frac{1}{2}$$

                                $$
                                \sum_{i=1}^{n} i
                                $$
                                """;

        var render = new MarkdownRender { Value = markdown };
        var window = new Window { Width = 900, Height = 700, Content = render };
        window.Show();
        Pump(2500);

        object greyValue = null;
        var grey9 = Application.Current?.TryFindResource("Grey9", out greyValue) == true;
        Console.WriteLine($"  [diag] Grey9 found={grey9} value={greyValue}");

        var views = render.GetVisualDescendants().OfType<MathView>().ToList();
        Assert(views.Count >= 3, "公式控件数量 >= 3", views.Count.ToString());
        if (views.Count < 3)
        {
            return;
        }

        for (var i = 0; i < views.Count; i++)
        {
            Console.WriteLine($"  [diag] view{i}: latex='{views[i].LaTeX}' size={views[i].FontSize} color={views[i].TextColor} parent={views[i].Parent?.GetType().Name}");
        }

        Assert(views[0].LaTeX == "E=mc^2", "行内公式 LaTeX 提取", views[0].LaTeX);

        var allOk = views.All(v => string.IsNullOrEmpty(v.ErrorMessage));
        Assert(allOk, "全部公式无排版错误", string.Join(" | ", views.Select(v => v.ErrorMessage ?? "-")));

        var blockViews = views.Where(v => v.Parent is not InlineUIContainer).ToList();
        Assert(blockViews.Count >= 1, "存在块级（MathBlock）公式", blockViews.Count.ToString());
        if (blockViews.Count > 0)
        {
            Assert(blockViews[0].FontSize > views[0].FontSize, "块级公式字号大于行内",
                $"{blockViews[0].FontSize} > {views[0].FontSize}");
        }

        Assert(views[0].TextColor != default, "公式文字颜色已应用", views[0].TextColor.ToString());
    }

    /// <summary>
    ///     验证 Emoji 扩展：正文短码换成 Unicode（仍是可选中的 Run），代码块与未知短码原样保留
    /// </summary>
    private static void CheckEmoji()
    {
        EmojiExtension.Register();
        Assert(EmojiExtension.ShortcodeCount > 1500, "短码表加载",
            EmojiExtension.ShortcodeCount.ToString());

        var body = RenderPlainText("Cheers :beer: and :tada: :+1:! Unknown :definitely_not_an_emoji_xyz: stays.");
        Assert(body.Contains("🍺") && body.Contains("🎉") && body.Contains("👍"),
            "正文短码替换为 emoji（Run，文本可选中）", body);
        Assert(body.Contains(":definitely_not_an_emoji_xyz:"), "未知短码原样保留");

        // 只渲染代码块：不得出现任何 emoji，且原文必须保留
        const string codeOnly = """
                                Inline `:beer:` and

                                ```
                                echo :beer:
                                ```
                                """;
        var code = RenderPlainText(codeOnly);
        Assert(!code.Contains("🍺"), "代码块内短码未被替换", code);
        Assert(code.Contains(":beer:"), "代码块内短码原文保留");
    }

    /// <summary>渲染一段 Markdown，收集可视树里所有可见文本</summary>
    private static string RenderPlainText(string markdown)
    {
        var render = new MarkdownRender { Value = markdown };
        var window = new Window { Width = 900, Height = 700, Content = render };
        window.Show();
        Pump(1200);
        return CollectVisibleText(render);
    }

    private static string CollectVisibleText(Control root)
    {
        var parts = new List<string>();
        foreach (var v in root.GetVisualDescendants())
        {
            switch (v)
            {
                case SelectableTextBlock tb:
                    parts.Add(tb.Text ?? string.Empty);
                    parts.AddRange(tb.Inlines.OfType<Run>().Select(r => r.Text ?? string.Empty));
                    break;
                case TextBox box:
                    parts.Add(box.Text ?? string.Empty);
                    break;
                case TextBlock block:
                    parts.Add(block.Text ?? string.Empty);
                    break;
            }
        }

        return string.Join(" | ", parts);
    }

    /// <summary>
    ///     FAMR 作为 MRX 副包：扩展只注册一次，FAMR 控件也应能渲染公式与 emoji
    /// </summary>
    private static void CheckFamrExtensions()
    {
        var famr = new FamrMarkdownRender { Value = "Beer :beer: and $E=mc^2$" };
        var window = new Window { Width = 900, Height = 400, Content = famr };
        window.Show();
        Pump(1500);

        var mathViews = famr.GetVisualDescendants().OfType<MathView>().ToList();
        Assert(mathViews.Count == 1, "FAMR 公式扩展生效", mathViews.Count.ToString());

        var text = CollectVisibleText(famr);
        Assert(text.Contains("🍺"), "FAMR emoji 扩展生效", text);
    }

    /// <summary>
    ///     Mermaid 扩展：```mermaid 围栏块渲染为 SvgImage；
    ///     CSS 变量 / color-mix 必须完全展开，其他语言的围栏块不受影响。
    /// </summary>
    private static void CheckMermaid()
    {
        MermaidExtension.Register();

        const string markdown = """
                                # 图表

                                ```mermaid
                                flowchart LR
                                    A[开始] --> B{判断}
                                    B -- 是 --> C[完成]
                                ```

                                ```csharp
                                var x = 1;
                                ```
                                """;

        var render = new MarkdownRender { Value = markdown };
        var window = new Window { Width = 900, Height = 700, Content = render };
        window.Show();
        Pump(1800);

        var images = render.GetVisualDescendants().OfType<Image>()
            .Where(i => i.Source is SvgImage).ToList();
        Assert(images.Count == 1, "mermaid 围栏块渲染为 SvgImage", images.Count.ToString());
        Assert(MermaidExtension.Instance.LastError == null, "Mermaid 渲染无异常",
            MermaidExtension.Instance.LastError ?? "-");
        Assert((images.FirstOrDefault()?.Source as SvgImage)?.Source != null, "SvgSource 已加载");

        var text = CollectVisibleText(render);
        Assert(text.Contains("csharp"), "普通围栏块仍显示语言标签", text);

        var plain = new MarkdownRender { Value = "```csharp\nvar x = 1;\n```" };
        var plainWindow = new Window { Width = 900, Height = 400, Content = plain };
        plainWindow.Show();
        Pump(1200);
        var plainSvg = plain.GetVisualDescendants().OfType<Image>().Count(i => i.Source is SvgImage);
        Assert(plainSvg == 0, "普通围栏代码块未被 Mermaid 接管", plainSvg.ToString());

        // 直接验证 CSS 扁平化器（Mermaider 原生输出带大量 var()/color-mix()）
        var raw = Mermaider.MermaidRenderer.RenderSvg(
            "flowchart LR\n  A[Start] --> B[End]", new MermaidRenderOptions());
        Assert(raw.Contains("var(--") && raw.Contains("color-mix("),
            "Mermaider 原生 SVG 含 CSS 变量");
        var flat = SvgCssFlattener.Flatten(raw);
        Assert(!flat.Contains("var(") && !flat.Contains("color-mix("),
            "CSS 变量与 color-mix 已全部展开");
        Assert(!flat.Contains("<style"), "不可解析的 style 块已移除");
        Assert(flat.Contains("stroke=\"#"), "描边颜色已展开为十六进制");
    }

    private static void Pump(int totalMs = 300)
    {
        var steps = Math.Max(1, totalMs / 50);
        for (var i = 0; i < steps; i++)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(50);
        }

        Dispatcher.UIThread.RunJobs();
    }

    private static void Assert(bool condition, string what, string detail = null)
    {
        if (condition)
        {
            Console.WriteLine($"[PASS] {what}{(detail == null ? "" : $" ({detail})")}");
        }
        else
        {
            Console.WriteLine($"[FAIL] {what}{(detail == null ? "" : $" ({detail})")}");
            _failures++;
        }
    }
}

internal sealed class SmokeApp : Application
{
    public static SmokeApp Instance { get; private set; }

    public SmokeApp()
    {
        Instance = this;
    }

    public override void Initialize()
    {
        Styles.Add(new FluentTheme());
        var componentStyles = (Styles)AvaloniaXamlLoader.Load(
            new Uri("avares://MarkdownRenderX.Avalonia/Index.axaml"));
        Styles.Add(componentStyles);

        var kaTeXStyles = (Styles)AvaloniaXamlLoader.Load(
            new Uri("avares://MarkdownRenderX.Avalonia.KaTeX/Index.axaml"));
        Styles.Add(kaTeXStyles);
        RequestedThemeVariant = ThemeVariant.Light;
    }
}
