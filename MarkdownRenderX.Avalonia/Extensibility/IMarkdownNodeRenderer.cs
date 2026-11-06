using Avalonia.Controls;
using Avalonia.Controls.Documents;
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

    /// <summary>
    ///     创建行内文本元素（可选）。用于需要保持"文本可选中、可复制"的扩展，
    ///     例如把 <c>:beer:</c> 替换成 emoji 字符。默认返回 null。
    ///     仅在行内节点分派时调用，且优先于 <see cref="CreateControl" />；
    ///     带默认实现，已有渲染器无需改动即可兼容。
    /// </summary>
    Inline? CreateInline(MarkdownObject node) => null;
}
