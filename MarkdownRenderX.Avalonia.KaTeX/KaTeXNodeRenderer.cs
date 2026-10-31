using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Markdig.Extensions.Mathematics;
using Markdig.Syntax;
using MarkdownRenderX.Avalonia.Extensibility;
using MathView = CSharpMath.Avalonia.MathView;

namespace MarkdownRenderX.Avalonia.KaTeX;

/// <summary>
///     把 Markdig 的数学节点（<c>$...$</c> / <c>$$...$$</c>）渲染为原生公式控件。
///     排版由 CSharpMath（Skia 绘制）完成，不使用 WebView。
/// </summary>
public sealed class KaTeXNodeRenderer : IMarkdownNodeRenderer
{
    /// <summary>行内公式字号</summary>
    public float InlineFontSize { get; set; } = 16;

    /// <summary>块级（独占一行）公式字号</summary>
    public float DisplayFontSize { get; set; } = 18;

    /// <summary>块级公式上下外边距</summary>
    public double DisplayMargin { get; set; } = 8;

    public bool CanRender(MarkdownObject node) => node is MathBlock or MathInline;

    public Control? CreateControl(MarkdownObject node)
    {
        return node switch
        {
            MathBlock block => CreateDisplay(ExtractBlockLatex(block)),
            MathInline inline => CreateInlineOrDisplay(inline),
            _ => null
        };
    }

    /// <summary>
    ///     单行 <c>$$ ... $$</c> 会被 Markdig 解析为 <see cref="MathInline" />，
    ///     这里按定界符数量把它当作块级公式处理，以获得更大的字号。
    /// </summary>
    private Control CreateInlineOrDisplay(MathInline inline)
    {
        var latex = ExtractInlineLatex(inline);
        return inline.DelimiterCount >= 2 ? CreateDisplay(latex) : CreateInline(latex);
    }

    private Control CreateDisplay(string latex)
    {
        var view = new MathView
        {
            LaTeX = latex,
            FontSize = DisplayFontSize,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, DisplayMargin, 0, DisplayMargin)
        };

        ApplyThemeColor(view);
        return view;
    }

    private Control CreateInline(string latex)
    {
        var view = new MathView
        {
            LaTeX = latex,
            FontSize = InlineFontSize,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(2, 0, 2, 0)
        };

        ApplyThemeColor(view);
        return view;
    }

    /// <summary>
    ///     公式颜色绑定到主题资源（Light / Dark 各一套）。
    ///     MathView 在构造时写入了本地默认值，样式 Setter 无法覆盖，故在代码中直接绑定。
    /// </summary>
    private static void ApplyThemeColor(MathView view)
    {
        view.Bind(MathView.TextColorProperty, new DynamicResourceExtension("MdMathTextColor"));
    }

    /// <summary>
    ///     提取 <c>$$ ... $$</c> 块的 LaTeX 文本（Markdig 已剥离定界符）
    /// </summary>
    private static string ExtractBlockLatex(MathBlock block)
    {
        var lines = block.Lines;
        if (lines.Count == 0)
        {
            return string.Empty;
        }

        var parts = new List<string>(lines.Count);
        for (var i = 0; i < lines.Count; i++)
        {
            parts.Add(lines.Lines[i].ToString());
        }

        return string.Join("\n", parts).Trim();
    }

    /// <summary>
    ///     提取 <c>$ ... $</c> 行内公式的 LaTeX 文本
    /// </summary>
    private static string ExtractInlineLatex(MathInline inline)
    {
        return inline.Content.ToString().Trim();
    }
}
