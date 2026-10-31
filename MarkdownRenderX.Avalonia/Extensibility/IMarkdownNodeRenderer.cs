using Avalonia.Controls;
using Markdig.Syntax;

namespace MarkdownRenderX.Avalonia.Extensibility;

/// <summary>
///     Markdown 节点渲染器扩展点。
///     实现该接口并在 <see cref="MarkdownRendererRegistry" /> 注册后，
///     可接管指定 Markdig 节点（如公式、自定义容器）的渲染。
/// </summary>
public interface IMarkdownNodeRenderer
{
    /// <summary>
    ///     是否由本渲染器处理该节点
    /// </summary>
    bool CanRender(MarkdownObject node);

    /// <summary>
    ///     创建对应的 Avalonia 控件；返回 null 表示放弃，交回内置渲染逻辑
    /// </summary>
    Control? CreateControl(MarkdownObject node);
}
