using CustomLoadingScreens.Data;
using CustomLoadingScreens.Managers;
using CustomLoadingScreens.Patches;
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
        internal static bool FrameRead = false;

        private static void AlbumHandler(object s, CustomAlbums.ModExtensions.AlbumEventArgs e)
        {
            var album = e.Album;

            using var png = album.OpenNullableStream("loading.png");
            using var gif = album.OpenNullableStream("loading.gif");
            using var text = album.OpenNullableStream("quotes.txt");

            if (text is not null)
            {
                CustomDataManager.AddAlbumBoundQuotes(album.AlbumName, text);
                Logger.Msg("Added text!");
            }

            switch (gif)
            {
                // Both are null, return
                case null when png is null:
                    return;
                // Gif is null, use png
                case null:
                    CustomDataManager.AlbumBoundImages.Add(album.AlbumName, new CustomImage(album.Path, "loading.png"));
                    Logger.Msg("Added PNG!");
                    break;
                // Gif is non-null, use gif
                default:
                    CustomDataManager.AlbumBoundImages.Add(album.AlbumName, new CustomImage(album.Path, "loading.gif"));
                    Logger.Msg("Added GIF!");
                    break;
            }

        }
        public override void OnInitializeMelon()
        {
            ModSettings.Register();
            CustomAlbums.ModExtensions.Events.OnAlbumLoaded += AlbumHandler;

            if (!Directory.Exists(ModSettings.CustomImageFolder)) Directory.CreateDirectory(ModSettings.CustomImageFolder);
            if (!Directory.Exists(ModSettings.CustomQuoteFolder)) Directory.CreateDirectory(ModSettings.CustomQuoteFolder);

            CustomDataManager.LoadImages();
            CustomDataManager.LoadQuotes();
        }

        public override void OnLateInitializeMelon()
        {
            Logger.Msg($"Loaded {CustomDataManager.CustomImages.Count} custom loading image" +
                       $"{(CustomDataManager.CustomImages.Count != 1 ? "s" : "")} and {CustomDataManager.GetNumberOfQuotes} custom loading quote" +
                       $"{(CustomDataManager.GetNumberOfQuotes != 1 ? "s" : "")}.", false);
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
