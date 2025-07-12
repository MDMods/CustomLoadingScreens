using CustomAlbums.Managers;
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

        private static void LoadLoadingImages()
        {
            var albums = AlbumManager.LoadedAlbums;

            foreach (var (albumName, album) in albums)
            {
                Func<string, Stream> openStream;

                if (!album.IsPack)
                {
                    openStream = album.OpenNullableStream;
                }
                else
                {
                    var pack = PackManager.GetPackFromUid(album.Uid);
                    if (pack is null)
                    {
                        Logger.Error($"Pack with UID {album.Uid} not found.");
                        continue;
                    }
                    openStream = pack.OpenNullableStream;
                }

                using var png = openStream("loading.png");
                using var gif = openStream("loading.gif");
                using var mp4 = openStream("loading.mp4");
                using var text = openStream("quotes.txt");

                if (text is not null)
                {
                    CustomDataManager.AddAlbumBoundQuotes(album.AlbumName, text);
                    Logger.Msg($"Added bound quotes for album {album.AlbumName}.");
                }

                // Prioritize mp4, then gif, then png
                if (mp4 is not null)
                {
                    CustomDataManager.AlbumBoundImages.Add(album.AlbumName, new CustomImage(album.Path, "loading.mp4"));
                    Logger.Msg($"Added MP4 for album {album.AlbumName}!");
                    continue;
                }
                if (gif is not null)
                {
                    CustomDataManager.AlbumBoundImages.Add(album.AlbumName, new CustomImage(album.Path, "loading.gif"));
                    Logger.Msg($"Added GIF for album {album.AlbumName}!");
                    continue;
                }
                if (png is not null)
                {
                    CustomDataManager.AlbumBoundImages.Add(album.AlbumName, new CustomImage(album.Path, "loading.png"));
                    Logger.Msg($"Added PNG for album {album.AlbumName}!");
                    continue;
                }
            }
        }

        public override void OnInitializeMelon()
        {
            ModSettings.Register();

            if (!Directory.Exists(ModSettings.CustomImageFolder)) Directory.CreateDirectory(ModSettings.CustomImageFolder);
            if (!Directory.Exists(ModSettings.CustomQuoteFolder)) Directory.CreateDirectory(ModSettings.CustomQuoteFolder);

            CustomDataManager.LoadImages();
            CustomDataManager.LoadQuotes();

            LoadLoadingImages();
        }

        public override void OnLateInitializeMelon()
        {
            Logger.Msg($"Loaded {CustomDataManager.CustomImages.Count} custom loading image" +
                       $"{(CustomDataManager.CustomImages.Count != 1 ? "s" : "")} and {CustomDataManager.GetNumberOfQuotes} custom loading quote" +
                       $"{(CustomDataManager.GetNumberOfQuotes != 1 ? "s" : "")}.", false);

            Logger.Msg($"Loaded {CustomDataManager.AlbumBoundImages.Count} album-bound loading image" +
                       $"{(CustomDataManager.AlbumBoundImages.Count != 1 ? "s" : "")} and {CustomDataManager.BoundQuotes.Count} album-bound quote" +
                       $"{(CustomDataManager.BoundQuotes.Count != 1 ? "s" : "")}.", false);
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
