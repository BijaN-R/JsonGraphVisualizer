using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace JsonVisualizer.Helpers
{
    /// <summary>
    /// 🎨 Syntax Highlighter برای JSON
    /// </summary>
    public static class JsonSyntaxHighlighter
    {
        // 🎨 رنگ‌ها
        public static Brush PropertyNameBrush { get; set; } = new SolidColorBrush(Color.FromRgb(86, 156, 214));   // آبی
        public static Brush StringValueBrush { get; set; } = new SolidColorBrush(Color.FromRgb(206, 145, 120));  // نارنجی
        public static Brush BracketBrush { get; set; } = new SolidColorBrush(Color.FromRgb(180, 120, 200));      // بنفش
        public static Brush CommaBrush { get; set; } = new SolidColorBrush(Color.FromRgb(80, 200, 120));         // سبز
        public static Brush SpecialCharBrush { get; set; } = new SolidColorBrush(Color.FromRgb(220, 80, 80));    // قرمز
        public static Brush NumberBrush { get; set; } = new SolidColorBrush(Color.FromRgb(181, 206, 168));       // سبز روشن
        public static Brush BoolNullBrush { get; set; } = new SolidColorBrush(Color.FromRgb(86, 156, 214));      // آبی
        public static Brush DefaultBrush { get; set; } = Brushes.White;

        /// <summary>
        /// 🔄 اعمال Syntax Highlighting روی RichTextBox
        /// </summary>
        public static void ApplyHighlighting(System.Windows.Controls.RichTextBox richTextBox)
        {
            if (richTextBox == null) return;

            // 📝 گرفتن متن فعلی
            var textRange = new TextRange(
                richTextBox.Document.ContentStart,
                richTextBox.Document.ContentEnd);
            string text = textRange.Text;

            if (string.IsNullOrWhiteSpace(text)) return;

            // 🔒 ذخیره موقعیت کرسر
            var caretPosition = richTextBox.CaretPosition;
            int caretOffset = GetCaretOffset(richTextBox);

            // 🧹 پاک کردن و ساختن دوباره
            var paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0);
            paragraph.LineHeight = 1;

            // 🎨 توکنایز و رنگ‌آمیزی
            var tokens = Tokenize(text);
            foreach (var token in tokens)
            {
                var run = new Run(token.Text)
                {
                    Foreground = GetBrushForTokenType(token.Type)
                };
                paragraph.Inlines.Add(run);
            }

            // 🔄 جایگزینی محتوا
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(paragraph);

            // 🔙 بازگرداندن موقعیت کرسر
            RestoreCaretPosition(richTextBox, caretOffset);
        }

        /// <summary>
        /// 📍 گرفتن آفست کرسر
        /// </summary>
        private static int GetCaretOffset(System.Windows.Controls.RichTextBox richTextBox)
        {
            var start = richTextBox.Document.ContentStart;
            var caret = richTextBox.CaretPosition;
            var range = new TextRange(start, caret);
            return range.Text.Length;
        }

        /// <summary>
        /// 📍 بازگرداندن موقعیت کرسر
        /// </summary>
        private static void RestoreCaretPosition(System.Windows.Controls.RichTextBox richTextBox, int offset)
        {
            try
            {
                var position = richTextBox.Document.ContentStart;
                int count = 0;

                while (position != null && count < offset)
                {
                    if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        int textLength = position.GetTextRunLength(LogicalDirection.Forward);
                        if (count + textLength >= offset)
                        {
                            position = position.GetPositionAtOffset(offset - count);
                            break;
                        }
                        count += textLength;
                    }
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                }

                if (position != null)
                {
                    richTextBox.CaretPosition = position;
                }
            }
            catch
            {
                // اگه مشکلی بود، کرسر رو آخر بذار
                richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
            }
        }

        /// <summary>
        /// 🔍 توکنایز کردن متن JSON
        /// </summary>
        private static List<JsonToken> Tokenize(string text)
        {
            var tokens = new List<JsonToken>();
            int i = 0;

            while (i < text.Length)
            {
                char c = text[i];

                // 🔲 Brackets و Braces
                if (c == '{' || c == '}' || c == '[' || c == ']' || c == '(' || c == ')')
                {
                    tokens.Add(new JsonToken(c.ToString(), TokenType.Bracket));
                    i++;
                }
                // ✅ کاما
                else if (c == ',')
                {
                    tokens.Add(new JsonToken(",", TokenType.Comma));
                    i++;
                }
                // ➡️ Colon
                else if (c == ':')
                {
                    tokens.Add(new JsonToken(":", TokenType.Colon));
                    i++;
                }
                // 📝 String (رشته)
                else if (c == '"')
                {
                    var (str, newIndex) = ReadString(text, i);

                    // تشخیص اینکه Property Name هست یا Value
                    var tokenType = IsPropertyName(text, newIndex)
                        ? TokenType.PropertyName
                        : TokenType.StringValue;

                    tokens.Add(new JsonToken(str, tokenType));
                    i = newIndex;
                }
                // 🔢 عدد
                else if (char.IsDigit(c) || (c == '-' && i + 1 < text.Length && char.IsDigit(text[i + 1])))
                {
                    var (num, newIndex) = ReadNumber(text, i);
                    tokens.Add(new JsonToken(num, TokenType.Number));
                    i = newIndex;
                }
                // 🔤 کلمات کلیدی (true, false, null)
                else if (char.IsLetter(c))
                {
                    var (word, newIndex) = ReadWord(text, i);
                    var type = (word == "true" || word == "false" || word == "null")
                        ? TokenType.BoolNull
                        : TokenType.Other;
                    tokens.Add(new JsonToken(word, type));
                    i = newIndex;
                }
                // ⬜ Whitespace
                else if (char.IsWhiteSpace(c))
                {
                    var (ws, newIndex) = ReadWhitespace(text, i);
                    tokens.Add(new JsonToken(ws, TokenType.Whitespace));
                    i = newIndex;
                }
                // ❓ سایر کاراکترها
                else
                {
                    tokens.Add(new JsonToken(c.ToString(), TokenType.Other));
                    i++;
                }
            }

            return tokens;
        }

        /// <summary>
        /// 📖 خواندن رشته
        /// </summary>
        private static (string text, int newIndex) ReadString(string input, int start)
        {
            int i = start + 1; // از بعد " شروع
            var sb = new System.Text.StringBuilder();
            sb.Append('"');

            while (i < input.Length)
            {
                char c = input[i];
                sb.Append(c);

                if (c == '"' && (i == start + 1 || input[i - 1] != '\\'))
                {
                    i++;
                    break;
                }
                i++;
            }

            return (sb.ToString(), i);
        }

        /// <summary>
        /// 🔢 خواندن عدد
        /// </summary>
        private static (string text, int newIndex) ReadNumber(string input, int start)
        {
            int i = start;
            var sb = new System.Text.StringBuilder();

            while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.' || input[i] == '-' ||
                   input[i] == '+' || input[i] == 'e' || input[i] == 'E'))
            {
                sb.Append(input[i]);
                i++;
            }

            return (sb.ToString(), i);
        }

        /// <summary>
        /// 🔤 خواندن کلمه
        /// </summary>
        private static (string text, int newIndex) ReadWord(string input, int start)
        {
            int i = start;
            var sb = new System.Text.StringBuilder();

            while (i < input.Length && char.IsLetter(input[i]))
            {
                sb.Append(input[i]);
                i++;
            }

            return (sb.ToString(), i);
        }

        /// <summary>
        /// ⬜ خواندن فضای خالی
        /// </summary>
        private static (string text, int newIndex) ReadWhitespace(string input, int start)
        {
            int i = start;
            var sb = new System.Text.StringBuilder();

            while (i < input.Length && char.IsWhiteSpace(input[i]))
            {
                sb.Append(input[i]);
                i++;
            }

            return (sb.ToString(), i);
        }

        /// <summary>
        /// ❓ آیا این رشته Property Name هست؟
        /// </summary>
        private static bool IsPropertyName(string text, int afterStringIndex)
        {
            // بعد از رشته، فضای خالی رو رد کن و ببین : هست یا نه
            int i = afterStringIndex;
            while (i < text.Length && char.IsWhiteSpace(text[i]))
            {
                i++;
            }
            return i < text.Length && text[i] == ':';
        }

        /// <summary>
        /// 🎨 گرفتن رنگ برای نوع توکن
        /// </summary>
        private static Brush GetBrushForTokenType(TokenType type)
        {
            return type switch
            {
                TokenType.PropertyName => PropertyNameBrush,
                TokenType.StringValue => StringValueBrush,
                TokenType.Bracket => BracketBrush,
                TokenType.Comma => CommaBrush,
                TokenType.Number => NumberBrush,
                TokenType.BoolNull => BoolNullBrush,
                TokenType.Colon => DefaultBrush,
                TokenType.Whitespace => DefaultBrush,
                _ => DefaultBrush
            };
        }
    }

    /// <summary>
    /// 🏷️ نوع توکن
    /// </summary>
    public enum TokenType
    {
        PropertyName,   // 🔵 آبی - نام پراپرتی
        StringValue,    // 🟠 نارنجی - مقدار رشته‌ای
        Bracket,        // 🟣 بنفش - {}[]()
        Comma,          // 🟢 سبز - ,
        Number,         // 🟢 سبز روشن - اعداد
        BoolNull,       // 🔵 آبی - true/false/null
        Colon,          // ⬜ سفید - :
        Whitespace,     // ⬜ سفید - فضای خالی
        SpecialChar,    // 🔴 قرمز - کاراکترهای خاص
        Other           // ⬜ سفید - سایر
    }

    /// <summary>
    /// 📦 توکن JSON
    /// </summary>
    public class JsonToken
    {
        public string Text { get; }
        public TokenType Type { get; }

        public JsonToken(string text, TokenType type)
        {
            Text = text;
            Type = type;
        }
    }
}
