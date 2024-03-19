using CustomLoadingScreens.Managers;
using CustomLoadingScreens.Utilities;
using MelonLoader;

namespace CustomLoadingScreens
{
    public class Main : MelonMod
    {
        private static readonly Logger Logger = new("CustomLoadingScreens");
        public override void OnInitializeMelon()
        {
            ModSettings.Register();

            if (!Directory.Exists(ModSettings.CustomImageFolder)) Directory.CreateDirectory(ModSettings.CustomImageFolder);
            if (!Directory.Exists(ModSettings.CustomQuoteFolder)) Directory.CreateDirectory(ModSettings.CustomQuoteFolder);

            CustomDataManager.LoadImages();
            CustomDataManager.LoadQuotes();

            Logger.Msg($"Loaded {CustomDataManager.CustomImages.Count} custom loading image" +
                       $"{(CustomDataManager.CustomImages.Count != 1 ? "s" : "")} and {CustomDataManager.GetNumberOfQuotes()} custom loading quote" +
                       $"{(CustomDataManager.GetNumberOfQuotes() != 1 ? "s" : "")}.", false);
        }
    }
}
