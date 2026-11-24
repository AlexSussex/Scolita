using System.Collections.Generic;

namespace Scolita.Core.Content.Json
{
    /// <summary>
    /// One value read from a JSON file (<see cref="JsonReader"/>), with the line it starts on, so a
    /// content rule can name the line of what it refuses.
    /// </summary>
    public sealed class JsonValue
    {
        private static readonly IReadOnlyList<JsonValue> NoItems = new JsonValue[0];
        private static readonly IReadOnlyList<KeyValuePair<string, JsonValue>> NoMembers = new KeyValuePair<string, JsonValue>[0];

        private JsonValue(JsonKind kind, int line, double number, string text, bool boolean,
            IReadOnlyList<JsonValue> items, IReadOnlyList<KeyValuePair<string, JsonValue>> members)
        {
            Kind = kind;
            Line = line;
            Number = number;
            Text = text;
            Boolean = boolean;
            Items = items;
            Members = members;
        }

        /// <summary>What it is.</summary>
        public JsonKind Kind { get; }

        /// <summary>The line it starts on, from 1.</summary>
        public int Line { get; }

        /// <summary>Its number; 0 when it isn't a number.</summary>
        public double Number { get; }

        /// <summary>Its text; empty when it isn't a string.</summary>
        public string Text { get; }

        /// <summary>Its value when it is a boolean.</summary>
        public bool Boolean { get; }

        /// <summary>Its items when it is an array; empty otherwise.</summary>
        public IReadOnlyList<JsonValue> Items { get; }

        /// <summary>Its members, in the order written, when it is an object; empty otherwise.</summary>
        public IReadOnlyList<KeyValuePair<string, JsonValue>> Members { get; }

        /// <summary>
        /// The member <paramref name="name"/> of an object; false when there is none (or it isn't an
        /// object). Called by the readers of JSON content and the tests.
        /// </summary>
        public bool TryGet(string name, out JsonValue value)
        {
            foreach (KeyValuePair<string, JsonValue> member in Members)
            {
                if (member.Key == name)
                {
                    value = member.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>An object; called by <see cref="JsonReader"/>.</summary>
        internal static JsonValue OfObject(int line, IReadOnlyList<KeyValuePair<string, JsonValue>> members)
        {
            return new JsonValue(JsonKind.Object, line, 0, string.Empty, false, NoItems, members);
        }

        /// <summary>An array; called by <see cref="JsonReader"/>.</summary>
        internal static JsonValue OfArray(int line, IReadOnlyList<JsonValue> items)
        {
            return new JsonValue(JsonKind.Array, line, 0, string.Empty, false, items, NoMembers);
        }

        /// <summary>A number; called by <see cref="JsonReader"/>.</summary>
        internal static JsonValue OfNumber(int line, double number)
        {
            return new JsonValue(JsonKind.Number, line, number, string.Empty, false, NoItems, NoMembers);
        }

        /// <summary>A string; called by <see cref="JsonReader"/>.</summary>
        internal static JsonValue OfString(int line, string text)
        {
            return new JsonValue(JsonKind.String, line, 0, text, false, NoItems, NoMembers);
        }

        /// <summary>A boolean; called by <see cref="JsonReader"/>.</summary>
        internal static JsonValue OfBoolean(int line, bool value)
        {
            return new JsonValue(JsonKind.Boolean, line, 0, string.Empty, value, NoItems, NoMembers);
        }

        /// <summary><c>null</c>; called by <see cref="JsonReader"/>.</summary>
        internal static JsonValue OfNull(int line)
        {
            return new JsonValue(JsonKind.Null, line, 0, string.Empty, false, NoItems, NoMembers);
        }
    }
}
