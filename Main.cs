using CustomLoadingScreens.Data;
using CustomLoadingScreens.Managers;
using CustomLoadingScreens.Patches;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine.UI;
using Logger = CustomLoadingScreens.Utilities.Logger;

namespace CustomLoadingScreens
{
    public class Main : MelonMod
    {
        private static readonly Logger Logger = new("CustomLoadingScreens");
        internal static Image Image;
        internal static CustomImage CurrentCustomImage;
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

        /// <summary>
        ///     Adds support for GIF images.
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
            LoadingImagePatch.LoadingImgPatch.Update();
        }
    }

}
