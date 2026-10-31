using Avalonia.Controls;
using Markdig.Syntax;

namespace MarkdownRenderX.Avalonia.Extensibility;

/// <summary>
///     全局 Markdown 节点渲染器注册表。
///     扩展包在启动时注册渲染器，即可接管指定节点（如 <c>MathBlock</c> / <c>MathInline</c>）的渲染。
/// </summary>
public static class MarkdownRendererRegistry
{
    private static readonly object Gate = new();
    private static readonly List<IMarkdownNodeRenderer> Renderers = [];

    /// <summary>已注册的渲染器数量</summary>
    public static int Count
    {
        get
        {
            lock (Gate)
            {
                return Renderers.Count;
            }
        }
    }

    /// <summary>
    ///     注册一个节点渲染器；重复注册会被忽略
    /// </summary>
    public static void Register(IMarkdownNodeRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        lock (Gate)
        {
            if (!Renderers.Contains(renderer))
            {
                Renderers.Add(renderer);
            }
        }
    }

    /// <summary>
    ///     注销渲染器
    /// </summary>
    /// <returns>是否确实移除了该渲染器</returns>
    public static bool Unregister(IMarkdownNodeRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        lock (Gate)
        {
            return Renderers.Remove(renderer);
        }
    }

    /// <summary>
    ///     清空全部渲染器（主要供测试使用）
    /// </summary>
    public static void Clear()
    {
        lock (Gate)
        {
            Renderers.Clear();
        }
    }

    /// <summary>
    ///     依次询问已注册的渲染器，第一个返回非 null 控件者胜出
    /// </summary>
    internal static Control? TryRender(MarkdownObject node)
    {
        IMarkdownNodeRenderer[] snapshot;

        lock (Gate)
        {
            if (Renderers.Count == 0)
            {
                return null;
            }

            snapshot = Renderers.ToArray();
        }

        foreach (var renderer in snapshot)
        {
            bool canRender;
            try
            {
                canRender = renderer.CanRender(node);
            }
            catch
            {
                // 第三方渲染器异常不能影响整篇文档的渲染
                continue;
            }

            if (!canRender)
            {
                continue;
            }

            try
            {
                var control = renderer.CreateControl(node);
                if (control != null)
                {
                    return control;
                }
            }
            catch
            {
                // 同上：渲染失败时退回内置逻辑
            }
        }

        return null;
    }
}
