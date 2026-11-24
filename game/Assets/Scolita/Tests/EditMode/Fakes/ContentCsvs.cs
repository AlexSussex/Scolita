using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Scolita.Core.Content;
using Scolita.Core.Content.Rules;

namespace Scolita.Tests.EditMode.Fakes
{
    /// <summary>
    /// A small, valid set of the seven content CSVs (three letters, seven words, one planet in both
    /// languages) for the content tests, and helpers to break one thing in it and check it. Each
    /// test takes its own copy from <see cref="Valid"/>.
    /// </summary>
    public static class ContentCsvs
    {
        private const string Letters =
            "id,lower,upper,group,strokes,retired,notes\n"
            + "a,a,A,vowel-differs,a,,\n"
            + "m,m,M,shared-close,m,,\n"
            + "t,t,T,shared-close,,,\n";

        private const string Pictures =
            "id,art,retired,notes\n"
            + "mother,,,\n"
            + "plane,,,\n"
            + "apple,,,\n"
            + "mat,,,\n"
            + "planet,,,\n";

        private const string Words =
            "id,language,text,letters,syllables,picture,listen_only,twin,false_friend,retired,notes\n"
            + "ro-mama,ro,mama,m a m a,ma-ma,mother,,,,,\n"
            + "ro-avion,ro,avion,,a-vi-on,plane,yes,,,,\n"
            + "en-apple,en,apple,,ap-ple,apple,yes,,,,\n"
            + "en-mat,en,mat,m a t,mat,mat,,,,,\n"
            + "ro-am,ro,am,a m,am,,,,,,reading only\n"
            + "ro-planeta,ro,planetă,,pla-ne-tă,planet,yes,en-planet,,,\n"
            + "en-planet,en,planet,,plan-et,planet,yes,ro-planeta,,,\n";

        private const string VoiceLines =
            "id,language,kind,text,clip,status,retired,notes\n"
            + "ro-sound-a,ro,sound,/a/,,,\n"
            + "ro-name-a,ro,name,a,,,\n"
            + "ro-sound-m,ro,sound,/m/,,,\n"
            + "ro-name-m,ro,name,me,,,\n"
            + "ro-sound-t,ro,sound,/t/,,,\n"
            + "ro-name-t,ro,name,te,,,\n"
            + "en-sound-a,en,sound,/\u00E6/,,,\n"
            + "en-name-a,en,name,ay,,,\n"
            + "en-sound-m,en,sound,/m/,,,\n"
            + "en-name-m,en,name,em,,,\n"
            + "en-sound-t,en,sound,/t/,,,\n"
            + "en-name-t,en,name,tee,,,\n"
            + "ro-word-mama,ro,word,mama,,,\n"
            + "ro-syllables-mama,ro,syllables,ma-ma,,,\n"
            + "ro-word-avion,ro,word,avion,,,\n"
            + "en-word-apple,en,word,apple,,,\n"
            + "en-word-mat,en,word,mat,,,\n"
            + "ro-word-am,ro,word,am,,,\n"
            + "ro-word-planeta,ro,word,planetă,,,\n"
            + "en-word-planet,en,word,planet,,,\n"
            + "ro-line-meet-new,ro,line,Hai s\u0103 cunoa\u0219tem o liter\u0103 nou\u0103!,,,\n"
            + "en-line-meet-new,en,line,Let's meet a new letter!,,,\n"
            + "ro-line-meet-listen,ro,line,Ascult\u0103: {x}\u2026 {x}\u2026 {word}!,,,\n"
            + "en-line-meet-listen,en,line,Listen: {x}\u2026 {x}\u2026 {word}!,,,\n"
            + "ro-line-trace-start,ro,line,\u00CEncepe de la punct \u0219i urmeaz\u0103 s\u0103geata.,,,\n"
            + "ro-line-syll,ro,line,Ascult\u0103 cuv\u00E2ntul \u0219i bate toba pentru fiecare silab\u0103.,,,\n"
            + "ro-line-syll-tick,ro,line,\"C\u00E2nd ai terminat, atinge bifa verde.\",,,\n"
            + "ro-line-syll-hint-listen,ro,line,Ascult\u0103 cum spun cuv\u00E2ntul pe silabe.,,,\n"
            + "ro-line-syll-hint-count,ro,line,Mai ascult\u0103 o dat\u0103 \u0219i num\u0103r\u0103 silabele.,,,\n"
            + "ro-line-syll-hint-once,ro,line,Bate toba o dat\u0103 pentru fiecare silab\u0103.,,,\n"
            + "ro-line-pick-letter,ro,line,Ascult\u0103 \u0219i atinge litera potrivit\u0103.,,,\n"
            + "ro-line-pick-picture,ro,line,Ascult\u0103 \u0219i atinge poza potrivit\u0103.,,,\n"
            + "en-line-pick-letter,en,line,Listen and tap the right letter.,,,\n"
            + "en-line-pick-picture,en,line,Listen and tap the right picture.,,,\n"
            + "ro-line-twins,ro,line,\"Une\u0219te cuvintele care seam\u0103n\u0103: unul \u00EEn rom\u00E2n\u0103, unul \u00EEn englez\u0103.\",,,\n"
            + "en-line-twins,en,line,\"Join the words that are alike: one in Romanian, one in English.\",,,\n"
            + "ro-line-twins-done,ro,line,Sunt gemeni!,,,\n"
            + "en-line-twins-done,en,line,They are twins!,,,\n"
            + "ro-line-false-friends,ro,line,\"Se scriu la fel, dar înseamnă altceva!\",,,\n"
            + "en-line-false-friends,en,line,\"They are spelled the same, but they mean different things!\",,,\n"
            + "ro-line-twins-hint-pictures,ro,line,Uită-te la poze: arată același lucru!,,,\n"
            + "en-line-twins-hint-pictures,en,line,Look at the pictures: they show the same thing!,,,\n"
            + "ro-line-twins-hint-spelled,ro,line,Caută cuvântul care se scrie la fel.,,,\n"
            + "en-line-twins-hint-spelled,en,line,Look for the word that is spelled the same.,,,\n"
            + "ro-line-twins-hint-sound,ro,line,Caută cuvântul care sună aproape la fel.,,,\n"
            + "en-line-twins-hint-sound,en,line,Look for the word that sounds almost the same.,,,\n"
            + "ro-line-twins-hint-show,ro,line,\"Uite, acesta este cuvântul potrivit. Unește-le!\",,,\n"
            + "en-line-twins-hint-show,en,line,\"Look, this is the right word. Join them!\",,,\n"
            + "ro-line-meet-tap-letter,ro,line,Atinge litera ca s-o auzi din nou.,,,\n"
            + "en-line-meet-tap-letter,en,line,Tap the letter to hear it again.,,,\n"
            + "ro-line-meet-tap-picture,ro,line,Acum atinge poza.,,,\n"
            + "en-line-meet-tap-picture,en,line,Now tap the picture.,,,\n"
            + "ro-line-pick-letter-x,ro,line,Atinge litera {x}.,,,\n"
            + "en-line-pick-letter-x,en,line,Tap the letter {x}.,,,\n"
            + "ro-line-listen-again,ro,line,Ascult\u0103 \u00EEnc\u0103 o dat\u0103.,,,\n"
            + "en-line-listen-again,en,line,Listen again.,,,\n"
            + "ro-line-pick-hint-other,ro,line,Aproape! \u00CEncearc\u0103 alt\u0103 liter\u0103.,,,\n"
            + "en-line-pick-hint-other,en,line,Nearly! Try another letter.,,,\n"
            + "ro-line-pick-hint-shape,ro,line,Uit\u0103-te bine la forma literei.,,,\n"
            + "en-line-pick-hint-shape,en,line,Look closely at the letter's shape.,,,\n"
            + "ro-line-pick-hint-show,ro,line,\"Uite, aceasta este litera {x}. Atinge-o!\",,,\n"
            + "en-line-pick-hint-show,en,line,\"Look, this is the letter {x}. Tap it!\",,,\n"
            + "ro-line-show-answer,ro,line,Uite r\u0103spunsul. Acum f\u0103-o \u0219i tu.,,,\n"
            + "en-line-show-answer,en,line,Here is the answer. Now you do it.,,,\n"
            + "ro-line-lesson-done,ro,line,Ai terminat lec\u021Bia. Bravo!,,,\n"
            + "en-line-lesson-done,en,line,You finished the lesson. Well done!,,,\n"
            + "ro-line-praise-1,ro,line,\"Bravo, ai reu\u0219it!\",,,\n"
            + "en-line-praise-1,en,line,\"Well done, you did it!\",,,\n"
            + "ro-line-praise-2,ro,line,E\u0219ti o stea!,,,\n"
            + "en-line-praise-2,en,line,You're a star!,,,\n";

        private const string Units =
            "id,domain,retired,notes\n"
            + "planet-1,reading,,\n";

        private const string Lessons =
            "id,unit,language,introduces,retired,notes\n"
            + "ro-a,planet-1,ro,a,,\n"
            + "ro-m,planet-1,ro,m,,\n"
            + "en-a,planet-1,en,a,,\n"
            + "en-m,planet-1,en,m,,\n"
            + "en-t,planet-1,en,t,,\n"
            + "party,planet-1,both,,,\n";

        private const string Steps =
            "lesson,kind,language,letters,words,options,notes\n"
            + "ro-a,MeetLetter,,a,ro-avion,,\n"
            + "ro-a,TraceLetter,,a,,form=print;case=lower,\n"
            + "ro-m,MeetLetter,,m,ro-mama,,\n"
            + "ro-m,Syllables,,,ro-mama,,\n"
            + "en-a,MeetLetter,,a,en-apple,,\n"
            + "en-m,ListenAndPick,,m a,,,\n"
            + "en-t,ListenAndPick,,t m a,,,\n"
            + "party,ListenAndPick,ro,a m,,,\n"
            + "party,WordTwins,,,ro-planeta en-planet,,\n";

        /// <summary>A fresh copy of the valid set, by file name; called by the content tests.</summary>
        public static Dictionary<string, string> Valid()
        {
            return new Dictionary<string, string>
            {
                { ContentFileNames.Letters, Letters },
                { ContentFileNames.Words, Words },
                { ContentFileNames.Pictures, Pictures },
                { ContentFileNames.VoiceLines, VoiceLines },
                { ContentFileNames.Units, Units },
                { ContentFileNames.Lessons, Lessons },
                { ContentFileNames.Steps, Steps }
            };
        }

        /// <summary>
        /// Adds <paramref name="row"/> at the end of <paramref name="file"/> in <paramref name="csvs"/>
        /// and returns its row number as a spreadsheet shows it. Called by the content tests.
        /// </summary>
        public static int AddRow(Dictionary<string, string> csvs, string file, string row)
        {
            csvs[file] = csvs[file] + row + "\n";
            return csvs[file].Count(character => character == '\n');
        }

        /// <summary>
        /// Replaces the one row of <paramref name="file"/> that starts with <paramref name="rowStart"/>
        /// by <paramref name="row"/>, and returns its row number; fails the test when no single row
        /// starts so. Called by the content tests.
        /// </summary>
        public static int ReplaceRow(Dictionary<string, string> csvs, string file, string rowStart, string row)
        {
            List<string> rows = csvs[file].TrimEnd('\n').Split('\n').ToList();
            int[] matches = Enumerable.Range(0, rows.Count)
                .Where(index => rows[index].StartsWith(rowStart, System.StringComparison.Ordinal))
                .ToArray();
            Assert.That(matches.Length, Is.EqualTo(1), file + " must have one row starting with " + rowStart);
            rows[matches[0]] = row;
            csvs[file] = string.Join("\n", rows) + "\n";
            return matches[0] + 1;
        }

        /// <summary>
        /// Reads and checks <paramref name="csvs"/> (<see cref="ContentTables.Read"/>, then
        /// <see cref="ContentRules.Check"/>) with only <paramref name="existingFiles"/> on disk, and
        /// returns every problem. Called by the content tests.
        /// </summary>
        public static IReadOnlyList<ContentProblem> Check(Dictionary<string, string> csvs, params string[] existingFiles)
        {
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(csvs, problems);
            ContentRules.Check(curriculum, new FakeContentFiles(existingFiles), problems);
            return problems.All;
        }

        /// <summary>The errors among <see cref="Check"/>'s problems; called by the content tests.</summary>
        public static ContentProblem[] Errors(Dictionary<string, string> csvs, params string[] existingFiles)
        {
            return Check(csvs, existingFiles)
                .Where(problem => problem.IsError)
                .ToArray();
        }

        /// <summary>Reads <paramref name="csvs"/> into a curriculum, asserting there was no error; called by the content tests.</summary>
        public static Curriculum Read(Dictionary<string, string> csvs)
        {
            ContentProblems problems = new ContentProblems();
            Curriculum curriculum = ContentTables.Read(csvs, problems);
            Assert.That(problems.All.Where(problem => problem.IsError).Select(problem => problem.ToString()), Is.Empty);
            return curriculum;
        }

        /// <summary>
        /// Asserts that <paramref name="errors"/> hold one at <paramref name="file"/>'s row
        /// <paramref name="row"/> whose reason holds every one of <paramref name="reasonParts"/>.
        /// Called by the content tests.
        /// </summary>
        public static void AssertError(IReadOnlyList<ContentProblem> errors, string file, int row, params string[] reasonParts)
        {
            string[] messages = errors.Select(error => error.ToString()).ToArray();
            bool found = errors.Any(error => error.Source.File == file
                && error.Source.Row == row
                && reasonParts.All(part => error.Reason.Contains(part)));
            Assert.That(found, Is.True, "expected an error at " + file + ", row " + row + " saying "
                + string.Join(" \u2026 ", reasonParts) + "; got:\n" + string.Join("\n", messages));
        }
    }
}
