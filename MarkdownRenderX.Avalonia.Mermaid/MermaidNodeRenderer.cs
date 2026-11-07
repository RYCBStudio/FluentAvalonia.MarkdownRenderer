using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Svg.Skia;
using Avalonia.Styling;
using Markdig.Syntax;
using MarkdownRenderX.Avalonia.Extensibility;
using Mermaider;
using MermaidRenderOptions = Mermaider.Models.RenderOptions;

namespace MarkdownRenderX.Avalonia.Mermaid;

/// <summary>
///     把 <c>```mermaid</c> 围栏代码块渲染为原生 SVG 图片。
///     解析与布局由 Mermaider（纯 .NET）完成，绘制走 Svg.Skia / SkiaSharp，
///     全程不使用 WebView、浏览器或 JavaScript 运行时。
/// </summary>
public sealed class MermaidNodeRenderer : IMarkdownNodeRenderer
{
    private const string FenceLanguage = "mermaid";

    /// <summary>图表配色，默认跟随应用主题</summary>
    public MermaidTheme Theme { get; set; } = MermaidTheme.Auto;

    /// <summary>图片最大高度（0 表示不限制）</summary>
    public double MaxHeight { get; set; } = 520;

    /// <summary>图表四周外边距</summary>
    public double Margin { get; set; } = 6;

    /// <summary>最近一次渲染失败的原因；成功时清空，便于诊断而不打断文档</summary>
    public string? LastError { get; private set; }

    public bool CanRender(MarkdownObject node)
    {
        return node is FencedCodeBlock block &&
               string.Equals(block.Info?.Trim(), FenceLanguage, StringComparison.OrdinalIgnoreCase);
    }

    public Control? CreateControl(MarkdownObject node)
    {
        if (node is not FencedCodeBlock block)
        {
            return null;
        }

        var code = ExtractCode(block);
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        try
        {
            var options = BuildOptions();
            var svg = MermaidRenderer.RenderSvg(code, options);
            var flattened = SvgCssFlattener.Flatten(svg);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(flattened));
            var svgSource = SvgSource.LoadFromStream(stream);
            if (svgSource is null)
            {
                LastError = "Svg.Skia could not load the generated SVG.";
                return null;
            }

            LastError = null;
            var image = new Image
            {
                Source = new SvgImage { Source = svgSource },
                Stretch = Stretch.Uniform,
                StretchDirection = StretchDirection.Both,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            if (MaxHeight > 0)
            {
                image.MaxHeight = MaxHeight;
            }

            return new Border
            {
                Child = image,
                Margin = new Thickness(Margin),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
        }
        catch (Exception ex)
        {
            // 返回 null 让注册表回退到内置代码块渲染，坏图表不会毁掉整篇文档
            LastError = $"{ex.GetType().Name}: {ex.Message}";
            return null;
        }
    }

    private MermaidRenderOptions BuildOptions()
    {
        var dark = Theme == MermaidTheme.Dark ||
                   (Theme == MermaidTheme.Auto &&
                    Application.Current?.ActualThemeVariant == ThemeVariant.Dark);

        return new MermaidRenderOptions
        {
            Bg = dark ? "#1E1E1E" : "#FFFFFF",
            Fg = dark ? "#ECECEC" : "#27272A",
            Line = dark ? "#8A8F98" : "#546072",
            Accent = dark ? "#60CDFF" : "#3B82F6",
            Font = "Segoe UI, Microsoft YaHei, sans-serif"
        };
    }

    private static string ExtractCode(FencedCodeBlock block)
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
}
