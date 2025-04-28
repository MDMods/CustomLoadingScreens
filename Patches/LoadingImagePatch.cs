using CustomAlbums.Managers;
using CustomLoadingScreens.Data;
using CustomLoadingScreens.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Database;
using UnityEngine;
using UnityEngine.UI;
using Logger = CustomLoadingScreens.Utilities.Logger;

namespace CustomLoadingScreens.Patches
{
    internal class LoadingImagePatch
    {
        /// <summary>
        ///     This HarmonyPatch class does all the work for setting the custom backgrounds and text.
        /// </summary>
        [HarmonyPatch(typeof(LoadingImg), nameof(LoadingImg.OnEnable))]
        internal class LoadingImgPatch
        {
            private static readonly Logger Logger = new(nameof(LoadingImgPatch));
            private static bool Is43AspectRatio(double width, double height) => width % height / height <= 1.7;
            private static bool TryGetBoundImage(out string albumName)
            {
                albumName = default;
                var uid = GlobalDataBase.dbMusicTag?.CurMusicInfo()?.uid;
                if (!uid?.StartsWith($"{AlbumManager.Uid}-") ?? true) return false;
                var album = AlbumManager.GetByUid(uid);
                albumName = album?.AlbumName ?? string.Empty;
                return CustomDataManager.AlbumBoundImages.ContainsKey(albumName);
            }

            private static bool TryGetBoundQuote(out string albumName)
            {
                albumName = default;
                var uid = GlobalDataBase.dbMusicTag?.CurMusicInfo()?.uid;
                if (!uid?.StartsWith($"{AlbumManager.Uid}-") ?? true) return false;
                var album = AlbumManager.GetByUid(uid);
                albumName = album?.AlbumName ?? string.Empty;
                return CustomDataManager.AlbumBoundQuotes.ContainsKey(albumName);
            }
            private static void Postfix(ref LoadingImg __instance)
            {
                Logger.Msg("Loading screen is active!");
                var isImageBound = TryGetBoundImage(out var albumName);
                var isQuoteBound = TryGetBoundQuote(out var quoteAlbumName);
                // Rolled a custom image and we have a custom image
                if (ProbabilityManager.UseCustomImage || isImageBound)
                {
                    var customImage = isImageBound ? CustomDataManager.AlbumBoundImages[albumName] : ProbabilityManager.GetRandomImage();
                    if (customImage is null) return;

                    __instance.simpleIllus.SetActive(false);
                    __instance.specialIllus.SetActive(false);
                    __instance.verySpecialIllus.SetActive(false);
                    __instance.specialIllusfor43.SetActive(false);

                    if (customImage.IsVideo)
                    {
                        CustomVideo.Start(__instance.gameObject, customImage.VideoPath);
                    } 
                    else
                    {
                        Main.CurrentCustomImage = customImage;

                        // Deal with the specialIllus variables, respective of aspect ratio
                        if (Is43AspectRatio(customImage.Sprites[0].texture.width, customImage.Sprites[0].texture.height))
                        {
                            __instance.specialIllusfor43.SetActive(true);
                            Main.Image = __instance.specialIllusfor43.GetComponent<Image>();
                        }
                        else
                        {
                            __instance.specialIllus.SetActive(true);
                            Main.Image = __instance.simpleIllus.GetComponent<Image>();
                        }

                        // Set image component in the Illus to the custom image
                        Main.Image.sprite = customImage.Sprites[0];
                    }

                    // We can only use bound quotes if that image exists, bound quotes will always be displayed
                    if (customImage.HasBoundQuote)
                    {
                        var randomBound = ProbabilityManager.GetRandomBoundQuote(customImage.Name);
                        __instance.GetComponentInChildren<Text>().text = randomBound;
                        Logger.Msg("Custom loading screen with bound text has been loaded.");
                        return;
                    }
                    Logger.Msg("Custom random loading screen has been loaded.");
                }
                else
                {
                    Logger.Msg("Official loading screen has been loaded.");
                }

                if (isQuoteBound)
                {
                    Logger.Error("Quote bound!");
                    __instance.GetComponentInChildren<Text>().text =
                        ProbabilityManager.GetRandomAlbumBoundQuote(quoteAlbumName);
                    return;
                }

                // Did not roll a custom quote
                if (!ProbabilityManager.UseCustomQuote)
                {
                    Logger.Msg("Official loading text has been loaded.");
                    return;
                }

                var quote = ProbabilityManager.GetRandomQuote();
                if (quote is null) return;

                // Rolled a random quote
                __instance.GetComponentInChildren<Text>().text = quote;
                Logger.Msg("Custom random loading text has been loaded.");
            }

            /// <summary>
            ///     Adds support for GIF images.
            /// </summary>
            internal static void Update()
            {
                if (Main.CurrentCustomImage is null || Main.Image == null || Main.CurrentCustomImage.FramesPerSecond is 0 || Main.CurrentCustomImage.IsVideo) return;
                
                var frame = (int)Mathf.Floor(Time.time * 1000) %
                    (Main.CurrentCustomImage.FramesPerSecond * Main.CurrentCustomImage.FrameCount) / Main.CurrentCustomImage.FramesPerSecond;

                // Updates animated loading screens
                if (Main.Image == null) return;
                Main.Image.sprite = Main.CurrentCustomImage.Sprites[frame];
            }
        }
    }
}
