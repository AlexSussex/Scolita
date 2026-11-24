using System.Collections.Generic;

namespace Scolita.Core.Content
{
    /// <summary>
    /// An option a kind of step takes (<c>form</c> of <see cref="ActivityKind.TraceLetter"/>): its
    /// key, the values it may have, and whether every step of the kind must give it.
    /// </summary>
    public sealed class ActivityOption
    {
        /// <summary>Creates an option; called by <see cref="ActivityKinds"/>.</summary>
        public ActivityOption(string key, IReadOnlyList<string> values, bool required)
        {
            Key = key;
            Values = values;
            Required = required;
        }

        /// <summary>The option's name, as <c>steps.csv</c> writes it.</summary>
        public string Key { get; }

        /// <summary>The values it may have.</summary>
        public IReadOnlyList<string> Values { get; }

        /// <summary>Whether every step of the kind must give it.</summary>
        public bool Required { get; }
    }
}
