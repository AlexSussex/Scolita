namespace Scolita.Core.Content.Json
{
    /// <summary>What a <see cref="JsonValue"/> is.</summary>
    public enum JsonKind
    {
        /// <summary>An object: named members, in the order written.</summary>
        Object,

        /// <summary>An array: items, in order.</summary>
        Array,

        /// <summary>A number.</summary>
        Number,

        /// <summary>A string.</summary>
        String,

        /// <summary><c>true</c> or <c>false</c>.</summary>
        Boolean,

        /// <summary><c>null</c>.</summary>
        Null
    }
}
