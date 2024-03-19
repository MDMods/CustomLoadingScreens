using MelonLoader;

namespace CustomLoadingScreens
{
    internal class ModSettings
    {
        private static MelonPreferences_Entry<bool> _verboseLogging;
        private static MelonPreferences_Entry<string> _customImageFolder;
        private static MelonPreferences_Entry<string> _customQuoteFolder;
        private static MelonPreferences_Entry<double> _probabilityOfCustomImage;
        private static MelonPreferences_Entry<double> _probabilityOfCustomQuote;

        public static bool VerboseLogging => _verboseLogging.Value;
        public static string CustomImageFolder => _customImageFolder.Value;
        public static string CustomQuoteFolder => _customQuoteFolder.Value;
        public static double CustomImageProbability => _probabilityOfCustomImage.Value;
        public static double CustomQuoteProbability => _probabilityOfCustomQuote.Value;

        internal static void Register()
        {
            var category = MelonPreferences.CreateCategory("CustomLoadingScreens", "Custom Loading Screens");

            _verboseLogging = category.CreateEntry("VerboseLogging", false, "Enable Verbose Logging");
            _customImageFolder = category.CreateEntry("CustomImageFolder", "UserData/CustomLoadingScreens", "Custom Image Folder Path");
            _customQuoteFolder = category.CreateEntry("CustomQuoteFolder", "UserData/CustomLoadingQuotes", "Custom Quote Folder Path");
            _probabilityOfCustomImage = category.CreateEntry("ImageProbability", 0.5, "Probability of Loading Custom Image (between 0-1)");
            _probabilityOfCustomQuote = category.CreateEntry("QuoteProbability", 0.5, "Probability of Loading Custom Quote (between 0-1)");
        }
    }
}
