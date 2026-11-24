using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Rules;
using Scolita.Tests.EditMode.Fakes;

namespace Scolita.Tests.EditMode.Core.Content
{
    /// <summary>
    /// Tests <see cref="ContentRules.Check"/>: each validator rule, fed a valid set with one thing
    /// broken, reports the file, the row and the reason.
    /// </summary>
    public sealed class ContentRulesTests
    {
        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails "mama" in a lesson before m is taught (the
        /// brief's acceptance), and passes it once it is listen-only.
        /// </summary>
        [Test]
        public void TestCheck_WordBeforeItsLettersAreTaught()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,Syllables,,,ro-mama,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "ro-mama uses m", "not taught yet", "ro track",
                "ro-a", "(taught: a)");
            Assert.That(errors.Length, Is.EqualTo(1));

            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-mama,", "ro-mama,ro,mama,m a m a,ma-ma,mother,yes,,,,");
            Assert.That(ContentCsvs.Errors(csvs).Select(error => error.ToString()), Is.Empty);
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> counts the letter a lesson introduces as taught in that lesson.</summary>
        [Test]
        public void TestCheck_TheLessonsOwnLetterIsTaught()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,TraceWord,,,ro-mama,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-line-word-write,ro,line,Scrie cuv\u00E2ntul de m\u00E2n\u0103.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            Assert.That(errors.Select(error => error.ToString()), Is.Empty);
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a listen-only word in a step that shows words to read.</summary>
        [Test]
        public void TestCheck_ListenOnlyWordToRead()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,TraceWord,,,ro-avion,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-line-word-write,ro,line,Scrie cuv\u00E2ntul de m\u00E2n\u0103.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "shows its words to read", "ro-avion is listen-only");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails letters that don't spell the word, and syllables that don't join into it.</summary>
        [Test]
        public void TestCheck_LettersAndSyllablesJoinIntoTheWord()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-mama,", "ro-mama,ro,mama,m a m,ma-m,mother,,,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Words, row, "spell 'mam'", "not the word 'mama'");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, row, "syllables ma-m", "not the word 'mama'");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> compares words without case or hyphens (<c>Tam-tam</c>).</summary>
        [Test]
        public void TestCheck_CaseAndHyphensDontCount()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Words, "ro-tam-tam,ro,Tam-tam,t a m t a m,tam-tam,,,,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-word-tam-tam,ro,word,Tam-tam,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            Assert.That(errors.Select(error => error.ToString()), Is.Empty);
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a <c>TwoSounds</c> step on a letter with one
        /// language (the brief's example of a step whose kind needs items it doesn't have).
        /// </summary>
        [Test]
        public void TestCheck_TwoSoundsOnALetterWithOneLanguage()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.AddRow(csvs, ContentFileNames.Letters, "a-breve,\u0103,\u0102,romanian-only,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-sound-a-breve,ro,sound,/\u0259/,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-name-a-breve,ro,name,\u0103,,,");
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps,
                "party,TwoSounds,,a-breve,ro-avion en-apple ro-mama en-mat,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "a-breve is not in the en alphabet", "TwoSounds");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a step with too many letters or too few words for its kind.</summary>
        [Test]
        public void TestCheck_ItemCounts()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int tooMany = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,MeetLetter,,m a,,,");
            int tooFew = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "party,WordTwins,,,ro-mama,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, tooMany, "MeetLetter step takes exactly 1 letters, not 2");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, tooFew, "WordTwins step takes 2 to 8 words, not 1");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails an unknown option and a required option left out.</summary>
        [Test]
        public void TestCheck_Options()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int unknown = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form=print;case=lower;speed=fast,");
            int missing = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,TraceLetter,,a,,form=cursive,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, unknown, "no option speed");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, missing, "form is print or handwritten, not cursive");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, missing, "needs its option case");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a word in a step of the other language.</summary>
        [Test]
        public void TestCheck_WordInAStepOfTheOtherLanguage()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "en-a,ListenAndPick,,,en-apple ro-avion,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "ro-avion is ro", "step is en");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a missing voice line: a letter's sound, a
        /// word's line, a <c>Syllables</c> word's syllables, a step's instruction in each language.
        /// </summary>
        [Test]
        public void TestCheck_MissingVoiceLines()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-sound-t,", "en-sound-q,en,sound,/k/,,,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-word-am,", "ro-word-ama,ro,word,am,,,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-syllables-mama,", "ro-syllables-tata,ro,syllables,ta-ta,,,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-line-twins,", "en-line-twin,en,line,Twins.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Letters, 4, "voice line en-sound-t is not in voice-lines.csv");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, 6, "voice line ro-word-am is not in voice-lines.csv");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, 5, "voice line ro-syllables-mama is not in voice-lines.csv");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, 10, "voice line en-line-twins is not in voice-lines.csv");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a word line that doesn't say the word, and an id that doesn't match its columns.</summary>
        [Test]
        public void TestCheck_VoiceLineTextAndId()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-word-mama,", "ro-word-mama,ro,word,mami,,,");
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-line-bravo,en,line,Well done!,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Words, 2, "ro-word-mama says 'mami'", "word is 'mama'");
            ContentCsvs.AssertError(errors, ContentFileNames.VoiceLines, row, "ro-line-bravo must be en-line-<key>");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> warns, and doesn't fail, on a named clip that isn't recorded yet.</summary>
        [Test]
        public void TestCheck_MissingClipIsAWarning()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-sound-m,", "ro-sound-m,ro,sound,/m/,ro-sound-m.ogg,,");

            IReadOnlyList<ContentProblem> missing = ContentCsvs.Check(csvs);
            IReadOnlyList<ContentProblem> recorded = ContentCsvs.Check(csvs, "ro-sound-m.ogg");

            ContentProblem warning = missing.Single(problem => problem.Source.File == ContentFileNames.VoiceLines);
            Assert.That(warning.IsError, Is.False);
            Assert.That(warning.Source.Row, Is.EqualTo(row));
            Assert.That(warning.ToString(), Does.StartWith("warning: voice-lines.csv, row " + row).And.Contain("system voice"));
            Assert.That(missing.Any(problem => problem.IsError), Is.False);
            Assert.That(recorded.Any(problem => problem.Source.File == ContentFileNames.VoiceLines), Is.False);
        }

        /// <summary>
        /// Tests the picture rules: a word without a picture fails in a step that shows pictures, a
        /// listen-only word needs one, named art must be there, and art left empty is only a warning.
        /// </summary>
        [Test]
        public void TestCheck_Pictures()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int stepRow = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,ListenAndPick,,,ro-mama ro-am,,");
            int wordRow = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "en-apple,", "en-apple,en,apple,,ap-ple,,yes,,,,");
            int artRow = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Pictures, "mother,", "mother,mother.png,,");

            IReadOnlyList<ContentProblem> problems = ContentCsvs.Check(csvs);
            ContentProblem[] errors = problems.Where(problem => problem.IsError).ToArray();

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, stepRow, "shows its words' pictures", "ro-am has none");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, wordRow, "listen-only word", "needs one");
            ContentCsvs.AssertError(errors, ContentFileNames.Pictures, artRow, "mother.png is not in content/pictures/");
            Assert.That(problems.Count(problem => !problem.IsError && problem.Reason.Contains("placeholder")), Is.EqualTo(3));
            Assert.That(ContentCsvs.Errors(csvs, "mother.png").Any(error => error.Source.File == ContentFileNames.Pictures), Is.False);
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a retired letter, word or picture still in use, and an unknown one.</summary>
        [Test]
        public void TestCheck_RetiredAndUnknownReferences()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-avion,", "ro-avion,ro,avion,,a-vi-on,plane,yes,,,yes,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.Pictures, "mat,", "mat,,yes,");
            int unknownRow = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,LetterHunt,,a z,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-line-hunt,ro,line,Atinge toate stelele care au litera {x}.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, 2, "word ro-avion is retired", "words.csv, row 3");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, 5, "picture mat is retired");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, unknownRow, "letter z is not in letters.csv");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a twin that is in the same language or doesn't name the word back.</summary>
        [Test]
        public void TestCheck_Twins()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int mamaRow = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-mama,", "ro-mama,ro,mama,m a m a,ma-ma,mother,,en-mat,,,");
            int amRow = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Words, "ro-am,", "ro-am,ro,am,a m,am,,,,ro-mama,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Words, mamaRow, "twin en-mat must name ro-mama back");
            ContentCsvs.AssertError(errors, ContentFileNames.Words, amRow, "false_friend ro-mama must be in the other language");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a word whose id doesn't start with its language.</summary>
        [Test]
        public void TestCheck_WordIdStartsWithItsLanguage()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Words, "tata,ro,tata,t a t a,ta-ta,,,,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "ro-word-tata,ro,word,tata,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Words, row, "must start with its language: ro-tata");
        }

        /// <summary>
        /// Tests the lesson rules: a lesson in both languages introduces no letter, a lesson introduces
        /// a letter of its language, and no unit or lesson is empty.
        /// </summary>
        [Test]
        public void TestCheck_Lessons()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int bothRow = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Lessons, "party,", "party,planet-1,both,t,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.Letters, "sh,sh,Sh,english-only,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "en-sound-sh,en,sound,/\u0283/,,,");
            ContentCsvs.AddRow(csvs, ContentFileNames.VoiceLines, "en-name-sh,en,name,ess aitch,,,");
            int wrongRow = ContentCsvs.AddRow(csvs, ContentFileNames.Lessons, "ro-sh,planet-1,ro,sh,,");
            int unitRow = ContentCsvs.AddRow(csvs, ContentFileNames.Units, "planet-2,reading,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Lessons, bothRow, "both languages introduces no letter");
            ContentCsvs.AssertError(errors, ContentFileNames.Lessons, wrongRow, "sh is not in the ro alphabet");
            ContentCsvs.AssertError(errors, ContentFileNames.Lessons, wrongRow, "ro-sh has no step");
            ContentCsvs.AssertError(errors, ContentFileNames.Units, unitRow, "planet-2 has no lesson");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a <c>MeetLetter</c> or <c>FirstSound</c> step
        /// whose word starts with none of its letters (it would have no right answer).
        /// </summary>
        [Test]
        public void TestCheck_WordStartsWithTheStepsLetter()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,MeetLetter,,m,ro-avion,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "ro-avion doesn't start with m", "no right answer");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a <c>ListenAndPick</c> step with letters and words both.</summary>
        [Test]
        public void TestCheck_ListenAndPickTakesLettersOrWords()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,ListenAndPick,,m a,ro-mama,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "letters or words", "not both");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a syllables line that doesn't say the word's syllables.</summary>
        [Test]
        public void TestCheck_SyllablesLineSaysTheSyllables()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "ro-syllables-mama,", "ro-syllables-mama,ro,syllables,mam-a,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Words, 2, "ro-syllables-mama says 'mam-a'", "syllables are 'ma-ma'");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> refuses art outside <c>content/pictures/</c>, even when the file exists.</summary>
        [Test]
        public void TestCheck_ArtStaysInThePicturesFolder()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.ReplaceRow(csvs, ContentFileNames.Pictures, "mother,", "mother,../secret.png,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs, "../secret.png");

            ContentCsvs.AssertError(errors, ContentFileNames.Pictures, row, "../secret.png must be a file name", "no folder");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a <c>ListenAndPick</c> step whose letters
        /// (its choices) include one not yet taught by its lesson, in its language's track.
        /// </summary>
        [Test]
        public void TestCheck_ChoicesAreTaughtLetters()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,ListenAndPick,,a m,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "letters are its choices",
                "not taught yet in the ro track by the lesson ro-a: m (taught: a)");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a <c>ListenAndPick</c> step that lists a choice twice.</summary>
        [Test]
        public void TestCheck_ChoicesOnce()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "en-t,ListenAndPick,,t a t,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "each once", "t is listed more than once");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a step whose kind's hint line is missing in its language.</summary>
        [Test]
        public void TestCheck_HintLinesExist()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-line-pick-hint-other,", "en-line-unused,en,line,Unused.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, 7, "en-line-pick-hint-other is not in voice-lines.csv");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a language with steps but without the lines
        /// every lesson says (its end, its first praise), at its first step.
        /// </summary>
        [Test]
        public void TestCheck_LessonLinesExist()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-line-lesson-done,", "en-line-unused,en,line,Unused.,,,");
            ContentCsvs.ReplaceRow(csvs, ContentFileNames.VoiceLines, "en-line-praise-1,", "en-line-unused-2,en,line,Unused.,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, 6, "en-line-lesson-done is not in voice-lines.csv");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, 6, "en-line-praise-1 is not in voice-lines.csv");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a <c>LetterHunt</c> or a <c>FirstSound</c> step with a
        /// letter not taught yet: a wrong choice is never a letter still to come (the human, 2026-11-24).
        /// </summary>
        [Test]
        public void TestCheck_HuntAndFirstSoundChoicesTaught()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int hunt = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,LetterHunt,,a m,,,");
            int first = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-a,FirstSound,,a m,ro-avion,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, hunt, "letters are its choices",
                "not taught yet in the ro track by the lesson ro-a: m (taught: a)");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, first, "letters are its choices",
                "not taught yet in the ro track by the lesson ro-a: m (taught: a)");
        }

        /// <summary>Tests that <see cref="ContentRules.Check"/> fails a <c>LetterHunt</c> step with no letter to tell apart from its own.</summary>
        [Test]
        public void TestCheck_HuntNeedsAnotherLetter()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "ro-m,LetterHunt,,m,,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "a LetterHunt step takes", "letters, not 1");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a <c>TwoSounds</c> step on a letter whose sound is the
        /// same in both languages (the human, 2026-11-24).
        /// </summary>
        [Test]
        public void TestCheck_TwoSoundsOnALetterWhoseSoundsDiffer()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int row = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "en-m,TwoSounds,,m,ro-mama en-mat ro-mama en-mat,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, row, "a letter whose sound differs between Romanian and English",
                "m is shared-close");
        }

        /// <summary>
        /// Tests that <see cref="ContentRules.Check"/> fails a <c>TwoSounds</c> step whose pictures are all of one
        /// language, or whose word doesn't start with its letter.
        /// </summary>
        [Test]
        public void TestCheck_TwoSoundsSortsBothLanguagesByTheLetter()
        {
            Dictionary<string, string> csvs = ContentCsvs.Valid();
            int oneLanguage = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "en-a,TwoSounds,,a,ro-avion ro-avion ro-avion ro-avion,,");
            int otherStart = ContentCsvs.AddRow(csvs, ContentFileNames.Steps, "en-a,TwoSounds,,a,ro-avion en-apple en-mat ro-avion,,");

            ContentProblem[] errors = ContentCsvs.Errors(csvs);

            ContentCsvs.AssertError(errors, ContentFileNames.Steps, oneLanguage, "sorts pictures of both languages",
                "none of its words is en");
            ContentCsvs.AssertError(errors, ContentFileNames.Steps, otherStart, "the word en-mat doesn't start with a");
            Assert.That(errors.Where(error => error.Source.Row == otherStart).Select(error => error.Reason),
                Has.None.Contains("both languages"));
        }
    }
}
