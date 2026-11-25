using NUnit.Framework.Interfaces;
using Scolita.Game.Saving;
using Scolita.Tests.PlayMode;
using UnityEngine;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(TemporarySaveRoot))]

namespace Scolita.Tests.PlayMode
{
    /// <summary>
    /// Gives the tests a save root of their own (<see cref="SaveRoot.UseTemporaryFolder"/>) for the length of a run, so no
    /// test reads or writes the Editor's own save root, which every worktree and the human's Editor share. The folder is
    /// named in the process's environment (<see cref="SaveRoot.EditorVariable"/>), which survives entering Play mode. A
    /// PlayMode run's folder is deleted once the Editor is back in Edit mode (<see cref="TemporarySaveRootCleanup"/>), after
    /// the last scene has closed and saved; an EditMode run's at once.
    /// </summary>
    public sealed class TemporarySaveRoot : ITestRunCallback
    {
        /// <summary>Called by the Test Framework when a run starts: a new, empty save root.</summary>
        public void RunStarted(ITest testsToRun)
        {
            SaveRoot.UseTemporaryFolder();
        }

        /// <summary>Called by the Test Framework when a run ends: an EditMode run gives its save root up now; a PlayMode run when Play mode ends.</summary>
        public void RunFinished(ITestResult testResults)
        {
            if (!Application.isPlaying)
            {
                SaveRoot.StopUsingTemporaryFolder();
            }
        }

        /// <summary>Called by the Test Framework before each test; nothing to do.</summary>
        public void TestStarted(ITest test)
        {
        }

        /// <summary>Called by the Test Framework after each test; nothing to do.</summary>
        public void TestFinished(ITestResult result)
        {
        }
    }
}
