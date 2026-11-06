using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace MarkdownRenderX.Avalonia.Emoji;

/// <summary>
///     源生成 JSON 上下文：保证在 AOT / trimming 构建下也能反序列化短码表。
/// </summary>
[JsonSerializable(typeof(Dictionary<string, string>))]
internal partial class EmojiJsonContext : JsonSerializerContext;

/// <summary>
///     内置短码表（GitHub gemoji + GitMoji），懒加载一次。
/// </summary>
internal static class EmojiCatalog
{
    private const string ResourceName = "MarkdownRenderX.Avalonia.Emoji.Resources.emoji-map.json";

    private static readonly Lazy<Dictionary<string, string>> Map =
        new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    // GitHub 短码允许字母、数字、下划线、加减号（:+1: / :-1: / :8ball:）
    private static readonly Regex Shortcode = new(
        @":([a-z0-9_+\-]+):",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>已加载的短码数量</summary>
    public static int Count => Map.Value.Count;

    /// <summary>
    ///     把文本中的短码替换为 Unicode emoji；未知短码原样保留。
    ///     没有变化时返回原字符串引用，便于调用方快速判断是否接管渲染。
    /// </summary>
    public static string Replace(string text)
    {
        if (string.IsNullOrEmpty(text) || text.IndexOf(':') < 0)
        {
            return text;
        }

        return Shortcode.Replace(text, static match =>
            Map.Value.TryGetValue(match.Groups[1].Value, out var emoji) ? emoji : match.Value);
    }

    private static Dictionary<string, string> Load()
    {
        try
        {
            using var stream = typeof(EmojiCatalog).Assembly.GetManifestResourceStream(ResourceName);
            if (stream is null)
            {
                return Empty();
            }

            var raw = JsonSerializer.Deserialize(stream, EmojiJsonContext.Default.DictionaryStringString);
            return raw is null
                ? Empty()
                : new Dictionary<string, string>(raw, StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            // 短码表损坏时降级为"不替换"，不影响 Markdown 正常渲染
            return Empty();
        }
    }

    private static Dictionary<string, string> Empty() => new(StringComparer.OrdinalIgnoreCase);
}
