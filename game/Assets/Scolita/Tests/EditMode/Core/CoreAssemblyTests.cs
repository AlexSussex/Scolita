using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Scolita.Core.Randomness;

namespace Scolita.Tests.EditMode.Core
{
    /// <summary>
    /// Tests the compiled <c>Scolita.Core</c> assembly: the seam that keeps the learning rules apart
    /// from Unity (AGENTS.md → "The five seams").
    /// </summary>
    public sealed class CoreAssemblyTests
    {
        /// <summary>
        /// Tests that <see cref="Assembly.GetReferencedAssemblies"/> of Core names no Unity assembly.
        /// </summary>
        [Test]
        public void TestGetReferencedAssemblies_CoreReferencesNoUnityAssembly()
        {
            Assembly core = typeof(SeededRandom).Assembly;

            string[] unityReferences = core.GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .Where(name => name.StartsWith("Unity", StringComparison.Ordinal))
                .ToArray();

            Assert.That(core.GetName().Name, Is.EqualTo("Scolita.Core"));
            Assert.That(unityReferences, Is.Empty);
        }
    }
}
