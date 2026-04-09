using System.Text;
using Microsoft.Maui.Controls.Shapes;

namespace MauiAI.ChatUI.Controls;

public sealed class MarkdownView : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MarkdownView),
        string.Empty,
        propertyChanged: static (bindable, _, _) => ((MarkdownView)bindable).Render());

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(MarkdownView),
        Colors.Black,
        propertyChanged: static (bindable, _, _) => ((MarkdownView)bindable).Render());

    public static readonly BindableProperty InlineCodeBackgroundColorProperty = BindableProperty.Create(
        nameof(InlineCodeBackgroundColor),
        typeof(Color),
        typeof(MarkdownView),
        Color.FromArgb("#E5E7EB"),
        propertyChanged: static (bindable, _, _) => ((MarkdownView)bindable).Render());

    public static readonly BindableProperty CodeBackgroundColorProperty = BindableProperty.Create(
        nameof(CodeBackgroundColor),
        typeof(Color),
        typeof(MarkdownView),
        Color.FromArgb("#111827"),
        propertyChanged: static (bindable, _, _) => ((MarkdownView)bindable).Render());

    public static readonly BindableProperty CodeTextColorProperty = BindableProperty.Create(
        nameof(CodeTextColor),
        typeof(Color),
        typeof(MarkdownView),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((MarkdownView)bindable).Render());

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color InlineCodeBackgroundColor
    {
        get => (Color)GetValue(InlineCodeBackgroundColorProperty);
        set => SetValue(InlineCodeBackgroundColorProperty, value);
    }

    public Color CodeBackgroundColor
    {
        get => (Color)GetValue(CodeBackgroundColorProperty);
        set => SetValue(CodeBackgroundColorProperty, value);
    }

    public Color CodeTextColor
    {
        get => (Color)GetValue(CodeTextColorProperty);
        set => SetValue(CodeTextColorProperty, value);
    }

    public MarkdownView()
    {
        Render();
    }

    private void Render()
    {
        var layout = new VerticalStackLayout
        {
            Spacing = 8
        };

        foreach (var block in MarkdownParser.Parse(Text))
        {
            if (block.IsCodeBlock)
            {
                layout.Children.Add(new Border
                {
                    BackgroundColor = CodeBackgroundColor,
                    Padding = new Thickness(12),
                    StrokeThickness = 0,
                    StrokeShape = new RoundRectangle { CornerRadius = 12 },
                    Content = new Label
                    {
                        Text = block.Text,
                        TextColor = CodeTextColor,
                        LineBreakMode = LineBreakMode.WordWrap,
                        FontFamily = GetMonospaceFont(),
                        FontSize = 13
                    }
                });

                continue;
            }

            layout.Children.Add(new Label
            {
                FormattedText = MarkdownParser.ToFormattedString(block.Text, TextColor, InlineCodeBackgroundColor),
                LineBreakMode = LineBreakMode.WordWrap,
                TextColor = TextColor
            });
        }

        Content = layout;
    }

    private static string GetMonospaceFont()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            return "monospace";
        }

        if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            return "Menlo";
        }

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            return "Consolas";
        }

        return "Courier New";
    }

    private sealed record MarkdownBlock(string Text, bool IsCodeBlock);

    private static class MarkdownParser
    {
        public static IReadOnlyList<MarkdownBlock> Parse(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return [new MarkdownBlock(string.Empty, false)];
            }

            var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
            var lines = normalized.Split('\n');
            var blocks = new List<MarkdownBlock>();
            var currentBlock = new StringBuilder();
            var inCodeBlock = false;

            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
                {
                    if (currentBlock.Length > 0)
                    {
                        blocks.Add(new MarkdownBlock(currentBlock.ToString().TrimEnd('\n'), inCodeBlock));
                        currentBlock.Clear();
                    }

                    inCodeBlock = !inCodeBlock;
                    continue;
                }

                currentBlock.AppendLine(line);
            }

            if (currentBlock.Length > 0 || blocks.Count == 0)
            {
                blocks.Add(new MarkdownBlock(currentBlock.ToString().TrimEnd('\n'), inCodeBlock));
            }

            return blocks;
        }

        public static FormattedString ToFormattedString(string text, Color textColor, Color inlineCodeBackgroundColor)
        {
            var formatted = new FormattedString();
            var buffer = new StringBuilder();
            var isBold = false;
            var isInlineCode = false;

            void Flush()
            {
                if (buffer.Length == 0)
                {
                    return;
                }

                formatted.Spans.Add(new Span
                {
                    Text = buffer.ToString(),
                    TextColor = isInlineCode ? Colors.Black : textColor,
                    BackgroundColor = isInlineCode ? inlineCodeBackgroundColor : Colors.Transparent,
                    FontAttributes = isBold ? FontAttributes.Bold : FontAttributes.None,
                    FontFamily = isInlineCode ? GetMonospaceFont() : null
                });

                buffer.Clear();
            }

            for (var index = 0; index < text.Length; index++)
            {
                if (text[index] == '*' && index + 1 < text.Length && text[index + 1] == '*')
                {
                    Flush();
                    isBold = !isBold;
                    index++;
                    continue;
                }

                if (text[index] == '`')
                {
                    Flush();
                    isInlineCode = !isInlineCode;
                    continue;
                }

                buffer.Append(text[index]);
            }

            Flush();
            return formatted;
        }
    }
}
