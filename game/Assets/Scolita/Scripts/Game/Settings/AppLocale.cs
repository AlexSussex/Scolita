using Scolita.Core.Languages;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Scolita.Game.Settings
{
    /// <summary>
    /// The language of the app's own words (the string tables under <c>Assets/Scolita/Localization/</c>): the
    /// Localization package's locale chosen from the tablet's settings (<see cref="TabletSettingsData.AppLanguage"/>),
    /// at boot and when the parent changes it. Romanian is <c>ro</c>, English is British English, <c>en-GB</c>.
    /// </summary>
    public static class AppLocale
    {
        /// <summary>The app's one string table collection.</summary>
        public const string TableName = "App";

        private const string RomanianCode = "ro";
        private const string EnglishCode = "en-GB";

        /// <summary>
        /// Waits for the Localization package to be ready (its tables are local), then selects the locale of
        /// <paramref name="language"/>; logs an error and keeps the current one when the tables can't be loaded or the
        /// project has no such locale.
        /// Called by <c>BootLoader</c>, the map, the parent area and the tests.
        /// </summary>
        public static void Select(Language language)
        {
            AsyncOperationHandle<LocalizationSettings> ready = LocalizationSettings.InitializationOperation;
            ready.WaitForCompletion();
            if (ready.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Localization: the string tables could not be loaded (" + ready.OperationException?.Message
                    + "); the app's words stay as they are.");
                return;
            }

            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(CodeOf(language));
            if (locale == null)
            {
                Debug.LogError("Localization: the project has no locale " + CodeOf(language) + "; the app's words stay in "
                    + LocalizationSettings.SelectedLocale + ".");
                return;
            }

            if (LocalizationSettings.SelectedLocale != locale)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
        }

        /// <summary>
        /// The app's words for <paramref name="key"/> in the selected locale (the key itself, with a warning from the
        /// package, when the table has no such entry). Called by the screens and flows that build their words in code.
        /// </summary>
        public static string Text(string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(TableName, key);
        }

        /// <summary>
        /// The app's words for <paramref name="key"/> in the selected locale, its <c>{0}</c>, <c>{1}</c>… filled with
        /// <paramref name="arguments"/>. Called by the screens that show numbers and names in their words.
        /// </summary>
        public static string Text(string key, params object[] arguments)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(TableName, key, null,
                FallbackBehavior.UseProjectSettings, arguments);
        }

        /// <summary>
        /// The app's words for <paramref name="key"/> in <paramref name="language"/>, whatever the selected locale: for
        /// words that belong to a lesson's language ("Bravo!" at the end of an English lesson is "Well done!").
        /// Called by the lesson's end.
        /// </summary>
        public static string TextIn(Language language, string key)
        {
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(CodeOf(language));
            return LocalizationSettings.StringDatabase.GetLocalizedString(TableName, key, locale);
        }

        /// <summary>The locale code of <paramref name="language"/>; called by <see cref="Select"/> and the tests.</summary>
        public static string CodeOf(Language language)
        {
            return language == Language.English ? EnglishCode : RomanianCode;
        }
    }
}
