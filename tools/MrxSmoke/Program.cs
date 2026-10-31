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
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MarkdownRenderX.Avalonia.Controls.MarkdownRender;
using MarkdownRenderX.Avalonia.Extensibility;
using MarkdownRenderX.Avalonia.KaTeX;
using MathView = CSharpMath.Avalonia.MathView;

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
