using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownRenderX.Avalonia.Extensibility;
using AvaloniaInline = Avalonia.Controls.Documents.Inline;

namespace MarkdownRenderX.Avalonia.Emoji;

/// <summary>
///     把 <c>LiteralInline</c> 中的 GitHub / GitMoji 短码替换成 Unicode emoji。
///     返回 <see cref="Run" /> 而非控件，因此文本仍然可选中、可复制、可参与换行。
///     代码行（<c>CodeInline</c>）与围栏代码块（<c>FencedCodeBlock</c>）不是 LiteralInline，天然不会被替换。
/// </summary>
public sealed class EmojiNodeRenderer : IMarkdownNodeRenderer
{
    public bool CanRender(MarkdownObject node) => node is LiteralInline;

    public Control? CreateControl(MarkdownObject node) => null;

    public AvaloniaInline? CreateInline(MarkdownObject node)
    {
        if (node is not LiteralInline literal)
        {
            return null;
        }

        var text = literal.Content.ToString();
        var replaced = EmojiCatalog.Replace(text);

        // 没有短码时交回内置渲染逻辑，保持原有行为完全一致
        return ReferenceEquals(text, replaced) ? null : new Run(replaced);
    }
}
