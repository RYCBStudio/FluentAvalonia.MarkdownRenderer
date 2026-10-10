using Avalonia.Controls;
using Markdig.Syntax;

namespace MarkdownRenderX.Avalonia;

public delegate void CodeToolRenderEventHandler(StackPanel headerPanel, StackPanel stackPanel,
    FencedCodeBlock fencedCodeBlock);