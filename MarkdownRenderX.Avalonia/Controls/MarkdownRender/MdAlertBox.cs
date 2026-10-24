using Avalonia;
using Avalonia.Controls.Primitives;
using MarkdownRenderX.Avalonia.i18n;

namespace MarkdownRenderX.Avalonia.Controls.MarkdownRender;

/// <summary>
///     GitHub 风格警告框（Alert）控件。
///     自绘实现，不依赖 FluentAvalonia 等任何第三方 UI 控件库；
///     配色与外观集中定义在 <c>Themes/Controls/MdAlertBox.axaml</c>，随明暗主题切换。
/// </summary>
public class MdAlertBox : TemplatedControl
{
    /// <summary>警告类型：tip / note / warning / important / caution</summary>
    public static readonly StyledProperty<string> KindProperty =
        AvaloniaProperty.Register<MdAlertBox, string>(nameof(Kind), "note");

    /// <summary>标题文本</summary>
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<MdAlertBox, string>(nameof(Title), string.Empty);

    /// <summary>标题前的字形符号</summary>
    public static readonly StyledProperty<string> GlyphProperty =
        AvaloniaProperty.Register<MdAlertBox, string>(nameof(Glyph), "\u2139");

    /// <summary>正文内容</summary>
    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<MdAlertBox, string>(nameof(Message), string.Empty);

    public string Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Glyph
    {
        get => GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    /// <summary>
    ///     按 GitHub Alert 类型创建警告框
    /// </summary>
    /// <param name="type">类型标识，如 tip / note / warning / important / caution</param>
    /// <param name="content">已剥离 <c>&gt;</c> 引用标记的正文</param>
    public static MdAlertBox Create(string type, string content)
    {
        var kind = (type ?? string.Empty).ToLowerInvariant();

        return new MdAlertBox
        {
            Kind = kind,
            Title = GetTitle(kind),
            Glyph = GetGlyph(kind),
            Message = content,
            Margin = new Thickness(0, 5, 0, 5)
        };
    }

    /// <summary>各类型的标题（随 UI 区域性本地化）</summary>
    private static string GetTitle(string kind)
    {
        return kind switch
        {
            "tip" => MdStrings.AlertTip,
            "note" => MdStrings.AlertNote,
            "warning" => MdStrings.AlertWarning,
            "important" => MdStrings.AlertImportant,
            "caution" => MdStrings.AlertCaution,
            _ => MdStrings.AlertInfo
        };
    }

    /// <summary>各类型的字形符号，使用各平台常见的 Unicode 符号，避免引入图标字体依赖</summary>
    private static string GetGlyph(string kind)
    {
        return kind switch
        {
            "tip" => "\u2714", // ✔
            "note" => "\u2139", // ℹ
            "warning" or "important" => "\u26A0", // ⚠
            "caution" => "\u2716", // ✖
            _ => "\u2139"
        };
    }
}
