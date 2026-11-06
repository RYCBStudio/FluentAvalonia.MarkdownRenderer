using System.Threading;
using MarkdownRenderX.Avalonia.Extensibility;

namespace MarkdownRenderX.Avalonia.Emoji;

/// <summary>
///     Emoji 短码支持的注册入口。在应用启动时调用一次 <see cref="Register" />，
///     之后所有 <c>MarkdownRender</c> 实例都会把 <c>:beer:</c> 之类的短码显示为 emoji。
///     无需引入任何样式文件、图片或 WebView。
/// </summary>
public static class EmojiExtension
{
    private static readonly EmojiNodeRenderer Renderer = new();
    private static int _registered;

    /// <summary>内置短码数量（首次访问时加载短码表）</summary>
    public static int ShortcodeCount => EmojiCatalog.Count;

    /// <summary>
    ///     注册短码渲染器；重复调用无副作用
    /// </summary>
    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
        {
            return;
        }

        MarkdownRendererRegistry.Register(Renderer);
    }

    /// <summary>
    ///     注销短码渲染器（主要供测试或需要关闭替换的场景使用）
    /// </summary>
    public static void Unregister()
    {
        MarkdownRendererRegistry.Unregister(Renderer);
        Interlocked.Exchange(ref _registered, 0);
    }
}
