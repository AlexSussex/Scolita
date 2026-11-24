using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Json;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>Tests <see cref="JsonReader"/>: what it reads, and what it refuses with its file and line.</summary>
    public sealed class JsonReaderTests
    {
        /// <summary>Tests that <see cref="JsonReader.Read"/> reads objects, arrays, numbers, strings, booleans and null, with their lines.</summary>
        [Test]
        public void TestRead_EveryKindOfValue()
        {
            ContentProblems problems = new ContentProblems();
            string text = "﻿{\n  \"a\": [1, -2.5, 3e2],\n  \"b\": \"x\\\"y\\u0103\",\n  \"c\": true,\n  \"d\": null\n}";

            JsonValue root = JsonReader.Read("test.json", text, problems);

            Assert.That(problems.All, Is.Empty);
            Assert.That(root.Kind, Is.EqualTo(JsonKind.Object));
            Assert.That(root.Members.Select(member => member.Key), Is.EqualTo(new[] { "a", "b", "c", "d" }));
            Assert.That(root.TryGet("a", out JsonValue numbers), Is.True);
            Assert.That(numbers.Line, Is.EqualTo(2));
            Assert.That(numbers.Items.Select(item => item.Number), Is.EqualTo(new[] { 1.0, -2.5, 300.0 }));
            Assert.That(root.TryGet("b", out JsonValue text2), Is.True);
            Assert.That(text2.Text, Is.EqualTo("x\"yă"));
            Assert.That(root.TryGet("c", out JsonValue yes) && yes.Boolean, Is.True);
            Assert.That(root.TryGet("d", out JsonValue none) && none.Kind == JsonKind.Null, Is.True);
            Assert.That(root.TryGet("e", out _), Is.False);
        }

        /// <summary>Tests that <see cref="JsonReader.Read"/> refuses what isn't JSON, naming the line.</summary>
        [TestCase("{\n  \"a\": [1, 2,]\n}", 2)]
        [TestCase("{\n  \"a\": 1\n  \"b\": 2\n}", 3)]
        [TestCase("{ \"a\": 1, \"a\": 2 }", 1)]
        [TestCase("{ \"a\": \"never closed }", 1)]
        [TestCase("[1]\n[2]", 2)]
        [TestCase("{ a: 1 }", 1)]
        [TestCase("[True]", 1)]
        [TestCase("[01]", 1)]
        [TestCase("", 1)]
        public void TestRead_RefusesWhatIsNotJson(string text, int line)
        {
            ContentProblems problems = new ContentProblems();

            JsonValue root = JsonReader.Read("test.json", text, problems);

            Assert.That(root, Is.Null);
            Assert.That(problems.All.Count, Is.EqualTo(1));
            Assert.That(problems.All[0].Source.File, Is.EqualTo("test.json"));
            Assert.That(problems.All[0].Source.Row, Is.EqualTo(line));
            Assert.That(problems.All[0].Reason, Does.StartWith("this is not JSON"));
        }
    }
}
