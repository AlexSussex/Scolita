namespace Scolita.Core.Content
{
    /// <summary>
    /// One option of a lesson step (<c>form=handwritten</c>): a key its kind takes
    /// (<see cref="ActivityKindRequirements.Options"/>) and its value.
    /// </summary>
    public readonly struct StepOption
    {
        /// <summary>Creates an option; called by <see cref="ContentTables"/>, the lesson asset and tests.</summary>
        public StepOption(string key, string value)
        {
            Key = key ?? string.Empty;
            Value = value ?? string.Empty;
        }

        /// <summary>The option's name (<c>form</c>).</summary>
        public string Key { get; }

        /// <summary>Its value (<c>handwritten</c>).</summary>
        public string Value { get; }

        /// <summary><c>key=value</c>, as <c>steps.csv</c> writes it.</summary>
        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }
}
