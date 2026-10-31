using System.Threading;
using MarkdownRenderX.Avalonia.Extensibility;

namespace MarkdownRenderX.Avalonia.KaTeX;

/// <summary>
///     KaTeX 公式支持的注册入口。
///     在 Avalonia 应用启动时调用一次 <see cref="Register" />，
///     之后所有 <c>MarkdownRender</c> 实例即会把 <c>$...$</c> / <c>$$...$$</c> 渲染成公式。
/// </summary>
public static class KaTeXExtension
{
    private static readonly KaTeXNodeRenderer Renderer = new();
    private static int _registered;

    /// <summary>
    ///     当前使用的渲染器实例（可调整字号等参数，调整后对新渲染的内容生效）
    /// </summary>
    public static KaTeXNodeRenderer Instance => Renderer;

    /// <summary>
    ///     注册公式渲染器并加载内置数学字体；重复调用无副作用
    /// </summary>
    public static void Register()
    {
        KaTeXFonts.EnsureLoaded();

        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        MarkdownRendererRegistry.Register(Renderer);
    }

    /// <summary>
    ///     注销公式渲染器（主要供测试或需要卸载扩展的场景使用）
    /// </summary>
    public static void Unregister()
    {
        MarkdownRendererRegistry.Unregister(Renderer);
        Interlocked.Exchange(ref _registered, 0);
    }
}
