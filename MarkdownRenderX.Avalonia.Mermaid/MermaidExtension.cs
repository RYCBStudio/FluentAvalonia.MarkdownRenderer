using System.Threading;
using MarkdownRenderX.Avalonia.Extensibility;

namespace MarkdownRenderX.Avalonia.Mermaid;

/// <summary>
///     Mermaid 图表支持的注册入口。
///     在 Avalonia 应用启动时调用一次 <see cref="Register" />，
///     之后所有 <c>MarkdownRender</c> 实例会把 <c>```mermaid</c> 围栏块渲染成图表。
/// </summary>
public static class MermaidExtension
{
    private static readonly MermaidNodeRenderer Renderer = new();
    private static int _registered;

    /// <summary>当前使用的渲染器实例（可调整主题、最大高度等参数）</summary>
    public static MermaidNodeRenderer Instance => Renderer;

    /// <summary>注册图表渲染器；重复调用无副作用</summary>
    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        MarkdownRendererRegistry.Register(Renderer);
    }

    /// <summary>注销图表渲染器（主要供测试使用）</summary>
    public static void Unregister()
    {
        MarkdownRendererRegistry.Unregister(Renderer);
        Interlocked.Exchange(ref _registered, 0);
    }
}
