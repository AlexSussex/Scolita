using Scolita.Core.Parent;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>The parent gate's and the daily limit's numbers as the learning rules asset has them, for the parent tests.</summary>
    public static class TestParentRules
    {
        /// <summary>A 3-second hold, the numbers 11 to 19, three choices, and limits of 10, 15 and 20 minutes; called by the tests.</summary>
        public static ParentRules Default()
        {
            return new ParentRules(3f, 11, 19, 3, new[] { 10, 15, 20 });
        }
    }
}
