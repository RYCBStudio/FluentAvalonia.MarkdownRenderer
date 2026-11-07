using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownRenderX.Avalonia.Mermaid;

/// <summary>
///     把 Mermaider 输出的 SVG（大量使用 CSS 自定义属性与 <c>color-mix()</c>）
///     展开为纯静态 SVG。
///     <para>
///         Svg.Skia 不解析 CSS 变量、<c>color-mix()</c>、<c>rem</c> 与 CSS filter，
///         未展开时颜色会被当成黑色或整块忽略（甘特图背景尤其明显）。
///         本类在内存中做字符串级求值，不引入 HTML/CSS 引擎，也不需要 WebView。
///     </para>
/// </summary>
internal static partial class SvgCssFlattener
{
    [GeneratedRegex(@"<style\b[^>]*>.*?</style>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex StyleBlockRegex();

    [GeneratedRegex(@"(?<name>--[\w-]+)\s*:\s*(?<value>[^;]+)")]
    private static partial Regex VariableDeclRegex();

    [GeneratedRegex(@"<svg\b[^>]*?\sstyle=""(?<style>[^""]*)""", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex RootStyleRegex();

    [GeneratedRegex(@"(?<value>-?[\d.]+)\s*%")]
    private static partial Regex PercentRegex();

    [GeneratedRegex(@"(?<value>[\d.]+)\s*rem\b")]
    private static partial Regex RemRegex();

    /// <summary>展开全部 CSS 变量 / color-mix，并移除无法解析的 style 块</summary>
    public static string Flatten(string svg)
    {
        if (string.IsNullOrEmpty(svg))
        {
            return svg;
        }

        var variables = CollectVariables(svg);
        var result = FlattenFunctions(svg, variables);

        // style 块里剩余的是字体、drop-shadow filter 等 Svg.Skia 不支持的内容
        result = StyleBlockRegex().Replace(result, string.Empty);
        // 根节点的 style 只是变量声明容器，展开后不再需要
        result = RootStyleRegex().Replace(result, m =>
            m.Value.Replace($" style=\"{m.Groups["style"].Value}\"", string.Empty, StringComparison.Ordinal), 1);

        return result;
    }

    private static Dictionary<string, string> CollectVariables(string svg)
    {
        var vars = new Dictionary<string, string>(StringComparer.Ordinal);

        var root = RootStyleRegex().Match(svg);
        if (root.Success)
        {
            AddDeclarations(vars, root.Groups["style"].Value);
        }

        foreach (Match block in StyleBlockRegex().Matches(svg))
        {
            // 只取大括号里的声明（svg { ... }），忽略选择器本身
            var body = block.Value;
            var open = body.IndexOf('{');
            var close = body.LastIndexOf('}');
            if (open >= 0 && close > open)
            {
                AddDeclarations(vars, body.Substring(open + 1, close - open - 1));
            }
        }

        return vars;
    }

    private static void AddDeclarations(Dictionary<string, string> vars, string declarations)
    {
        foreach (Match m in VariableDeclRegex().Matches(declarations))
        {
            vars[m.Groups["name"].Value] = m.Groups["value"].Value.Trim();
        }
    }

    /// <summary>
    ///     从左到右找出最外层 <c>var(...)</c> / <c>color-mix(...)</c> 函数并替换为具体值。
    ///     函数内部由 <see cref="ExpandInnermost" /> 逐层展开，因此支持嵌套
    ///     （例如 <c>color-mix(in srgb, var(--accent, var(--fg)) 15%, var(--bg))</c>）。
    /// </summary>
    private static string FlattenFunctions(string svg, Dictionary<string, string> variables)
    {
        var sb = new StringBuilder(svg.Length + 256);
        var i = 0;

        while (i < svg.Length)
        {
            var varAt = svg.IndexOf("var(", i, StringComparison.Ordinal);
            var mixAt = svg.IndexOf("color-mix(", i, StringComparison.Ordinal);
            var start = MinPositive(varAt, mixAt);
            if (start < 0)
            {
                sb.Append(svg, i, svg.Length - i);
                break;
            }

            sb.Append(svg, i, start - i);
            var open = svg.IndexOf('(', start);
            var close = FindMatchingParen(svg, open);
            if (close < 0)
            {
                sb.Append(svg, start, svg.Length - start);
                break;
            }

            var expression = svg.Substring(start, close - start + 1);
            var expanded = ExpandInnermost(expression, variables);

            if (TryParseColor(expanded, out var color))
            {
                sb.Append(FormatColor(color));
            }
            else
            {
                sb.Append(ConvertNonColor(expanded));
            }

            i = close + 1;
        }

        return sb.ToString();
    }

    /// <summary>反复展开最内层的函数，直到表达式里不再有括号</summary>
    private static string ExpandInnermost(string expr, Dictionary<string, string> variables)
    {
        for (var iteration = 0; iteration < 48; iteration++)
        {
            var open = expr.LastIndexOf('(');
            if (open < 0)
            {
                break;
            }

            var close = FindMatchingParen(expr, open);
            if (close < 0)
            {
                break;
            }

            var nameStart = open - 1;
            while (nameStart >= 0 && (char.IsLetterOrDigit(expr[nameStart]) || expr[nameStart] == '-'))
            {
                nameStart--;
            }

            var name = expr.Substring(nameStart + 1, open - nameStart - 1);
            var inner = expr.Substring(open + 1, close - open - 1);
            string replacement;

            switch (name)
            {
                case "var":
                {
                    var args = SplitTopLevel(inner);
                    if (args.Count == 0)
                    {
                        replacement = "#000000";
                    }
                    else if (variables.TryGetValue(args[0], out var defined))
                    {
                        replacement = defined;
                    }
                    else if (args.Count > 1)
                    {
                        // fallback 本身可能含嵌套函数，重新拼回去让下一轮继续展开
                        replacement = string.Join(",", args.GetRange(1, args.Count - 1));
                    }
                    else
                    {
                        replacement = "#000000";
                    }

                    break;
                }
                case "color-mix":
                    replacement = EvaluateColorMix(inner);
                    break;
                default:
                    // 未知函数（Svg.Skia 同样不认识），保留文本但去掉括号以免死循环
                    replacement = inner;
                    break;
            }

            expr = expr.Substring(0, nameStart + 1) + replacement + expr.Substring(close + 1);
        }

        return expr.Trim();
    }

    private static string EvaluateColorMix(string inner)
    {
        var args = SplitTopLevel(inner);
        if (args.Count < 3)
        {
            return "#000000";
        }

        var first = args[1];
        var percent = 50.0;
        var pm = PercentRegex().Match(first);
        if (pm.Success &&
            double.TryParse(pm.Groups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
        {
            percent = parsed;
        }

        var colorText = PercentRegex().Replace(first, string.Empty).Trim();
        if (!TryParseColor(colorText, out var c1) || !TryParseColor(args[2], out var c2))
        {
            return "#000000";
        }

        var p = Math.Clamp(percent / 100.0, 0.0, 1.0);
        return FormatColor((
            (byte)Math.Round(c1.R * p + c2.R * (1 - p)),
            (byte)Math.Round(c1.G * p + c2.G * (1 - p)),
            (byte)Math.Round(c1.B * p + c2.B * (1 - p)),
            c1.A * p + c2.A * (1 - p)));
    }

    /// <summary>非颜色变量：目前只有 rem 字号（1rem = 16px）</summary>
    private static string ConvertNonColor(string expr)
    {
        var rem = RemRegex().Match(expr);
        if (rem.Success &&
            double.TryParse(rem.Groups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
        {
            return (value * 16).ToString("0.##", CultureInfo.InvariantCulture);
        }

        var px = expr.TrimEnd('p', 'x').Trim();
        return string.IsNullOrEmpty(px) ? expr.Trim() : px;
    }

    private static int FindMatchingParen(string s, int open)
    {
        if (open < 0)
        {
            return -1;
        }

        var depth = 0;
        for (var i = open; i < s.Length; i++)
        {
            if (s[i] == '(')
            {
                depth++;
            }
            else if (s[i] == ')' && --depth == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private static List<string> SplitTopLevel(string s)
    {
        var parts = new List<string>();
        var depth = 0;
        var sb = new StringBuilder();
        foreach (var ch in s)
        {
            if (ch == '(')
            {
                depth++;
            }
            else if (ch == ')')
            {
                depth--;
            }

            if (ch == ',' && depth == 0)
            {
                parts.Add(sb.ToString().Trim());
                sb.Clear();
            }
            else
            {
                sb.Append(ch);
            }
        }

        parts.Add(sb.ToString().Trim());
        return parts;
    }

    private static int MinPositive(int a, int b) => a < 0 ? b : b < 0 ? a : Math.Min(a, b);

    private static bool TryParseColor(string text, out (byte R, byte G, byte B, double A) color)
    {
        color = default;
        var s = text.Trim();
        switch (s.ToLowerInvariant())
        {
            case "transparent":
                color = (0, 0, 0, 0);
                return true;
            case "white":
                color = (255, 255, 255, 1);
                return true;
            case "black":
                color = (0, 0, 0, 1);
                return true;
        }

        if (!s.StartsWith('#'))
        {
            return false;
        }

        var hex = s[1..];
        if (hex.Length == 3)
        {
            hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
        }

        if (hex.Length is not (6 or 8))
        {
            return false;
        }

        if (!byte.TryParse(hex.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) ||
            !byte.TryParse(hex.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) ||
            !byte.TryParse(hex.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
        {
            return false;
        }

        var a = 1.0;
        if (hex.Length == 8 &&
            byte.TryParse(hex.AsSpan(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var alpha))
        {
            a = alpha / 255.0;
        }

        color = (r, g, b, a);
        return true;
    }

    private static string FormatColor((byte R, byte G, byte B, double A) c)
    {
        return c.A >= 0.999
            ? $"#{c.R:X2}{c.G:X2}{c.B:X2}"
            : $"#{c.R:X2}{c.G:X2}{c.B:X2}{(byte)Math.Round(c.A * 255):X2}";
    }
}
