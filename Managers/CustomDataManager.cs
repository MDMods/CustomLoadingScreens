using CustomLoadingScreens.Data;
using CustomLoadingScreens.Utilities;

namespace CustomLoadingScreens.Managers
{
    internal class CustomDataManager
    {
        private const string JpgSearchPattern = "*.jpg";
        private const string PngSearchPattern = "*.png";
        private const string TxtSearchPattern = "*.txt";
        private const string GifSearchPattern = "*.gif";

        internal static readonly List<CustomImage> CustomImages = new();
        internal static readonly List<string> CustomQuotes = new();
        internal static readonly Dictionary<string, List<string>> BoundQuotes = new();
        internal static readonly Logger Logger = new(nameof(CustomDataManager));

        /// <summary>
        ///     Adds a bound quote to the BoundQuotes data structure.
        /// </summary>
        /// <param name="imageName">The file name of the image.</param>
        /// <param name="quote">The quote to bind to said image.</param>
        private static void AddBoundQuote(string imageName, string quote)
        {
            if (!BoundQuotes.ContainsKey(imageName)) BoundQuotes.Add(imageName, new List<string>());
            BoundQuotes[imageName].Add(quote);
        }

        /// <summary>
        ///     Adds an image to the CustomImages data structure.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        private static void AddImage(string path)
        {
            CustomImages.Add(new CustomImage(path));
        }

        /// <summary>
        ///     Loads the custom loading images into its respective data structure.
        /// </summary>
        internal static void LoadImages()
        {
            var imageFiles = Directory.EnumerateFiles(ModSettings.CustomImageFolder, JpgSearchPattern)
                .Concat(Directory.EnumerateFiles(ModSettings.CustomImageFolder, PngSearchPattern))
                .Concat(Directory.EnumerateFiles(ModSettings.CustomImageFolder, GifSearchPattern));
            foreach (var image in imageFiles)
            {
                AddImage(image);
            }
        }

        /// <summary>
        ///     Loads the custom loading quotes into its respective data structure.
        /// </summary>
        internal static void LoadQuotes()
        {
            var quoteFiles = Directory.EnumerateFiles(ModSettings.CustomQuoteFolder, TxtSearchPattern);
            foreach (var quote in quoteFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(quote);

                // Bound quote (the quotes in the file that
                if (CustomImages.Any(img => img.Name == fileName))
                {
                    Logger.Msg($"Loading bound quotes for {fileName}");
                    using var streamReader = new StreamReader(File.OpenRead(quote));
                    while (streamReader.ReadLine()?.Trim() is { } line)
                    {
                        AddBoundQuote(fileName, line);
                    }
                }
                else
                {
                    Logger.Msg($"Loading non-bound quotes for {fileName}");
                    using var streamReader = new StreamReader(File.OpenRead(quote));
                    while (streamReader.ReadLine()?.Trim() is { } line)
                    {
                        CustomQuotes.Add(line);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the number of custom loading quotes.
        /// </summary>
        /// <returns>The number of custom loading quotes.</returns>
        internal static int GetNumberOfQuotes() =>
            BoundQuotes.Select(kv => kv.Value.Count).Sum() + CustomQuotes.Count;
    }
}
