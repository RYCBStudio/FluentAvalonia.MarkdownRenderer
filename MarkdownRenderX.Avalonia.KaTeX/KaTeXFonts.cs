using System.Reflection;
using System.Threading;
using CSharpMath.Rendering.BackEnd;
using Typography.OpenFont;

namespace MarkdownRenderX.Avalonia.KaTeX;

/// <summary>
///     KaTeX 扩展使用的数学字体。
///     包内自带 Latin Modern Math（GUST Font License），因此不依赖系统是否安装数学字体。
/// </summary>
internal static class KaTeXFonts
{
    private const string FontResourceName = "MarkdownRenderX.Avalonia.KaTeX.Fonts.LatinModernMath.otf";

    private static int _state;

    /// <summary>
    ///     幂等地把内置字体注册到 CSharpMath 的全局字体集合
    /// </summary>
    internal static void EnsureLoaded()
    {
        if (Interlocked.Exchange(ref _state, 1) == 1)
        {
            return;
        }

        try
        {
            using var stream = typeof(KaTeXFonts).Assembly.GetManifestResourceStream(FontResourceName);
            if (stream is null)
            {
                return;
            }

            var typeface = new OpenFontReader().Read(stream);
            Fonts.GlobalTypefaces.AddSupplement(typeface);
        }
        catch
        {
            // 字体加载失败时退回 CSharpMath 的默认行为（可能依赖系统数学字体），不影响其它内容渲染
        }
    }
}
