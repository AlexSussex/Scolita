using System.Collections.Generic;

namespace Scolita.Content
{
    /// <summary>
    /// A content asset (a letter, a word, a lesson, a catalog) that can check its own data. Every
    /// ScriptableObject under <c>Assets/Scolita/Content</c> implements it, and the content validator
    /// (<c>ContentValidatorTests</c>, an EditMode test) runs it on each one.
    /// </summary>
    public interface IValidatableContent
    {
        /// <summary>
        /// Returns what is wrong with this asset, in words a person can act on; empty when it is
        /// valid. Called by <c>ContentValidatorTests</c>.
        /// </summary>
        IReadOnlyList<string> Validate();
    }
}
