using System;
using System.Linq;
using System.Text;
using Scolita.Core.Languages;

namespace Scolita.Core.Content.Rules
{
    /// <summary>
    /// The rules on each word in use: its id starts with its language, its letters spell it, its
    /// syllables join back into it, its picture, and its twin and false friend in the other language.
    /// Part of <see cref="ContentRules"/>.
    /// </summary>
    public static class WordRules
    {
        /// <summary>Checks every word that isn't retired; called by <see cref="ContentRules.Check"/>.</summary>
        public static void Check(Curriculum curriculum, ContentProblems problems)
        {
            foreach (Word word in curriculum.Words.All.Where(word => !word.Retired))
            {
                CheckId(word, problems);
                CheckLetters(curriculum, word, problems);
                CheckSyllables(word, problems);
                CheckPicture(curriculum, word, problems);
                CheckPartner(curriculum, word, word.Twin, "twin", other => other.Twin, problems);
                CheckPartner(curriculum, word, word.FalseFriend, "false_friend", other => other.FalseFriend, problems);
            }
        }

        /// <summary>
        /// The letters of <paramref name="text"/> as the content compares them: lower case, without
        /// hyphens (<c>Tam-tam</c> → <c>tamtam</c>). Called by the letter and syllable checks.
        /// </summary>
        public static string Spelling(string text)
        {
            return text.Replace("-", string.Empty).ToLowerInvariant();
        }

        private static void CheckId(Word word, ContentProblems problems)
        {
            string prefix = LanguageCodes.CodeOf(word.Language) + "-";
            string id = word.Id.Value;
            if (!id.StartsWith(prefix, StringComparison.Ordinal) || id.Length == prefix.Length)
            {
                problems.Error(word.Source, "the word's id " + id + " must start with its language: " + prefix
                    + Spelling(word.Text) + ".");
            }
        }

        private static void CheckLetters(Curriculum curriculum, Word word, ContentProblems problems)
        {
            if (word.Letters.Count == 0)
            {
                if (!word.ListenOnly)
                {
                    problems.Error(word.Source, "the cell letters is empty: write the word's letters as letter ids ("
                        + string.Join(" ", Spelling(word.Text).ToCharArray()) + ").");
                }

                return;
            }

            StringBuilder spelled = new StringBuilder();
            bool allKnown = true;
            foreach (ContentId letterId in word.Letters)
            {
                Letter letter = References.InUse(curriculum.Letters, letterId, word.Source, "letter",
                    ContentFileNames.Letters, problems);
                if (letter == null)
                {
                    allKnown = false;
                    continue;
                }

                if (!letter.HasLanguage(word.Language))
                {
                    problems.Error(word.Source, "the letter " + letterId + " is not in the "
                        + LanguageCodes.CodeOf(word.Language) + " alphabet (its group is "
                        + ContentCodes.CodeOf(letter.Group) + ").");
                }

                spelled.Append(letter.Lower);
            }

            if (allKnown && !string.Equals(spelled.ToString().ToLowerInvariant(), Spelling(word.Text), StringComparison.Ordinal))
            {
                problems.Error(word.Source, "the letters " + string.Join(" ", word.Letters) + " spell '" + spelled
                    + "', not the word '" + word.Text + "'.");
            }
        }

        private static void CheckSyllables(Word word, ContentProblems problems)
        {
            string joined = Spelling(string.Concat(word.Syllables.Select(syllable => syllable.Trim())));
            if (word.Syllables.Count > 0 && !string.Equals(joined, Spelling(word.Text), StringComparison.Ordinal))
            {
                problems.Error(word.Source, "the syllables " + string.Join("-", word.Syllables) + " join into '"
                    + joined + "', not the word '" + word.Text + "'.");
            }
        }

        private static void CheckPicture(Curriculum curriculum, Word word, ContentProblems problems)
        {
            if (word.Picture.IsSet)
            {
                References.InUse(curriculum.Pictures, word.Picture, word.Source, "picture", ContentFileNames.Pictures,
                    problems);
            }
            else if (word.ListenOnly)
            {
                problems.Error(word.Source, "a listen-only word is shown as its picture, so it needs one.");
            }
        }

        private static void CheckPartner(Curriculum curriculum, Word word, ContentId partnerId, string column,
            Func<Word, ContentId> partnerOf, ContentProblems problems)
        {
            if (!partnerId.IsSet)
            {
                return;
            }

            Word partner = References.InUse(curriculum.Words, partnerId, word.Source, "word", ContentFileNames.Words,
                problems);
            if (partner == null)
            {
                return;
            }

            if (partner.Language == word.Language)
            {
                problems.Error(word.Source, "the " + column + " " + partnerId + " must be in the other language.");
            }

            if (partnerOf(partner) != word.Id)
            {
                problems.Error(word.Source, "the " + column + " " + partnerId + " must name " + word.Id
                    + " back in its own " + column + " cell.");
            }
        }
    }
}
