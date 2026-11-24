using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Scolita.Core.Content.Json
{
    /// <summary>
    /// Reads the text of a JSON file (RFC 8259: objects, arrays, numbers, strings, <c>true</c>,
    /// <c>false</c>, <c>null</c>) into a <see cref="JsonValue"/>, strictly: the first thing that isn't
    /// JSON is reported with its file and line, and nothing is guessed. A byte order mark at the start
    /// is allowed. Core can't use Unity's <c>JsonUtility</c> (and it can't read arrays of arrays), so
    /// the content's JSON files (the letters' shapes) are read with this.
    /// </summary>
    public sealed class JsonReader
    {
        private const char ByteOrderMark = '﻿';
        private const int DeepestNesting = 64;

        private readonly string text;
        private int position;
        private int line = 1;
        private string error;
        private int errorLine;

        private JsonReader(string text)
        {
            this.text = text ?? string.Empty;
            position = this.text.Length > 0 && this.text[0] == ByteOrderMark ? 1 : 0;
        }

        /// <summary>
        /// Reads <paramref name="text"/>, the contents of <paramref name="fileName"/>; on anything that
        /// isn't JSON (or more than one value), reports an error into <paramref name="problems"/> naming
        /// the line and returns null. Called by <c>Scolita.Core.Tracing.LetterShapeReader</c> and
        /// <c>JsonReaderTests</c>.
        /// </summary>
        public static JsonValue Read(string fileName, string text, ContentProblems problems)
        {
            JsonReader reader = new JsonReader(text);
            JsonValue value = reader.ReadDocument();
            if (value == null)
            {
                problems.Error(new ContentSource(fileName, reader.errorLine), "this is not JSON: " + reader.error);
            }

            return value;
        }

        private JsonValue ReadDocument()
        {
            JsonValue value = ReadValue(0);
            if (value == null)
            {
                return null;
            }

            SkipSpace();
            return position < text.Length ? Fail("there is more after the end of the first value.") : value;
        }

        private JsonValue ReadValue(int depth)
        {
            if (depth > DeepestNesting)
            {
                return Fail("it is nested more than " + DeepestNesting + " deep.");
            }

            SkipSpace();
            if (position >= text.Length)
            {
                return Fail("it ends where a value should be.");
            }

            char next = text[position];
            switch (next)
            {
                case '{':
                    return ReadObject(depth);
                case '[':
                    return ReadArray(depth);
                case '"':
                    string read = ReadString();
                    return read == null ? null : JsonValue.OfString(line, read);
                case 't':
                    return ReadWord("true", JsonValue.OfBoolean(line, true));
                case 'f':
                    return ReadWord("false", JsonValue.OfBoolean(line, false));
                case 'n':
                    return ReadWord("null", JsonValue.OfNull(line));
                default:
                    return next == '-' || (next >= '0' && next <= '9')
                        ? ReadNumber()
                        : Fail("'" + next + "' can't start a value.");
            }
        }

        private JsonValue ReadObject(int depth)
        {
            int startLine = line;
            position++;
            List<KeyValuePair<string, JsonValue>> members = new List<KeyValuePair<string, JsonValue>>();
            HashSet<string> names = new HashSet<string>();
            SkipSpace();
            if (Peek('}'))
            {
                position++;
                return JsonValue.OfObject(startLine, members);
            }

            while (true)
            {
                SkipSpace();
                if (!Peek('"'))
                {
                    return Fail("a member's name (in double quotes) should come here.");
                }

                string name = ReadString();
                if (name == null)
                {
                    return null;
                }

                if (!names.Add(name))
                {
                    return Fail("the member \"" + name + "\" is written twice.");
                }

                SkipSpace();
                if (!Peek(':'))
                {
                    return Fail("a ':' should follow the member \"" + name + "\".");
                }

                position++;
                JsonValue value = ReadValue(depth + 1);
                if (value == null)
                {
                    return null;
                }

                members.Add(new KeyValuePair<string, JsonValue>(name, value));
                SkipSpace();
                if (Peek(','))
                {
                    position++;
                    continue;
                }

                if (Peek('}'))
                {
                    position++;
                    return JsonValue.OfObject(startLine, members);
                }

                return Fail("a ',' or a '}' should come here.");
            }
        }

        private JsonValue ReadArray(int depth)
        {
            int startLine = line;
            position++;
            List<JsonValue> items = new List<JsonValue>();
            SkipSpace();
            if (Peek(']'))
            {
                position++;
                return JsonValue.OfArray(startLine, items);
            }

            while (true)
            {
                JsonValue item = ReadValue(depth + 1);
                if (item == null)
                {
                    return null;
                }

                items.Add(item);
                SkipSpace();
                if (Peek(','))
                {
                    position++;
                    continue;
                }

                if (Peek(']'))
                {
                    position++;
                    return JsonValue.OfArray(startLine, items);
                }

                return Fail("a ',' or a ']' should come here.");
            }
        }

        private string ReadString()
        {
            position++;
            StringBuilder read = new StringBuilder();
            while (position < text.Length)
            {
                char next = text[position++];
                if (next == '"')
                {
                    return read.ToString();
                }

                if (next == '\\')
                {
                    if (!ReadEscape(read))
                    {
                        return null;
                    }

                    continue;
                }

                if (next < ' ')
                {
                    position--;
                    FailText("a string can't hold a line break or a control character; close it with '\"'.");
                    return null;
                }

                read.Append(next);
            }

            FailText("a string is never closed.");
            return null;
        }

        private bool ReadEscape(StringBuilder read)
        {
            if (position >= text.Length)
            {
                FailText("a string ends after '\\'.");
                return false;
            }

            char escaped = text[position++];
            switch (escaped)
            {
                case '"':
                case '\\':
                case '/':
                    read.Append(escaped);
                    return true;
                case 'b':
                    read.Append('\b');
                    return true;
                case 'f':
                    read.Append('\f');
                    return true;
                case 'n':
                    read.Append('\n');
                    return true;
                case 'r':
                    read.Append('\r');
                    return true;
                case 't':
                    read.Append('\t');
                    return true;
                case 'u':
                    if (position + 4 <= text.Length && int.TryParse(text.Substring(position, 4), NumberStyles.AllowHexSpecifier,
                            CultureInfo.InvariantCulture, out int code))
                    {
                        read.Append((char)code);
                        position += 4;
                        return true;
                    }

                    FailText("'\\u' must be followed by four hexadecimal digits.");
                    return false;
                default:
                    FailText("'\\" + escaped + "' is not an escape JSON knows.");
                    return false;
            }
        }

        private JsonValue ReadNumber()
        {
            int start = position;
            if (Peek('-'))
            {
                position++;
            }

            if (Peek('0'))
            {
                position++;
            }
            else if (!SkipDigits())
            {
                return Fail("a number needs a digit after its sign.");
            }

            if (Peek('.'))
            {
                position++;
                if (!SkipDigits())
                {
                    return Fail("a number needs a digit after its '.'.");
                }
            }

            if (Peek('e') || Peek('E'))
            {
                position++;
                if (Peek('+') || Peek('-'))
                {
                    position++;
                }

                if (!SkipDigits())
                {
                    return Fail("a number needs a digit in its exponent.");
                }
            }

            string written = text.Substring(start, position - start);
            double number = double.Parse(written, NumberStyles.Float, CultureInfo.InvariantCulture);
            return double.IsInfinity(number) ? Fail("the number " + written + " is too large.") : JsonValue.OfNumber(line, number);
        }

        private bool SkipDigits()
        {
            int start = position;
            while (position < text.Length && text[position] >= '0' && text[position] <= '9')
            {
                position++;
            }

            return position > start;
        }

        private JsonValue ReadWord(string word, JsonValue value)
        {
            if (string.CompareOrdinal(text, position, word, 0, word.Length) != 0)
            {
                return Fail("'" + text[position] + "' can't start a value (true, false and null are written in small letters).");
            }

            position += word.Length;
            return value;
        }

        private void SkipSpace()
        {
            while (position < text.Length)
            {
                char next = text[position];
                if (next == '\n')
                {
                    line++;
                }
                else if (next != ' ' && next != '\t' && next != '\r')
                {
                    return;
                }

                position++;
            }
        }

        private bool Peek(char expected)
        {
            return position < text.Length && text[position] == expected;
        }

        private JsonValue Fail(string reason)
        {
            FailText(reason);
            return null;
        }

        private void FailText(string reason)
        {
            error = reason;
            errorLine = line;
        }
    }
}
