using CustomLoadingScreens.Data;

namespace CustomLoadingScreens.Managers
{
    internal class ProbabilityManager
    {
        private static readonly Random Rng = new();

        /// <summary>
        ///     Calculates whether the loading image should be custom. 
        /// </summary>
        /// <returns><c>true</c> if the loading image should be custom, <c>false</c> otherwise.</returns>
        internal static bool UseCustomImage => ModSettings.CustomImageProbability > Rng.NextDouble();

        /// <summary>
        ///     Calculates whether the loading quote should be custom. 
        /// </summary>
        /// <returns><c>true</c> if the loading quote should be custom, <c>false</c> otherwise.</returns>
        internal static bool UseCustomQuote() =>
            ModSettings.CustomQuoteProbability > Rng.NextDouble();

        /// <summary>
        ///     Gets a random custom loading screen image.
        /// </summary>
        /// <returns>A random custom loading screen image.</returns>
        internal static CustomImage GetRandomImage() =>
            CustomDataManager.CustomImages[Rng.Next(CustomDataManager.CustomImages.Count)];
        
        /// <summary>
        ///     Gets a random custom loading screen quote.
        /// </summary>
        /// <returns>A random custom loading screen quote.</returns>
        internal static string GetRandomQuote() =>
            CustomDataManager.CustomQuotes[Rng.Next(CustomDataManager.CustomQuotes.Count)];

        /// <summary>
        ///     Gets a random custom bound loading screen quote.
        /// </summary>
        /// <param name="fileName">The file name of the bound image/quote.</param>
        /// <returns>A random custom bound loading screen quote.</returns>
        internal static string GetRandomBoundQuote(string fileName) =>
            CustomDataManager.BoundQuotes[fileName][Rng.Next(CustomDataManager.BoundQuotes[fileName].Count)];
    }
}
