using System.Linq;
using Scolita.Core.Voice;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>The ids of a voice sequence's lines, in order, for assertions. Used by the activity and lesson tests.</summary>
    public static class SequenceLines
    {
        /// <summary>The ids of <paramref name="sequence"/>'s lines; called by the activity and lesson tests.</summary>
        public static string[] Of(VoiceSequence sequence)
        {
            return sequence.Items
                .Select(item => item.Line.ToString())
                .ToArray();
        }
    }
}
