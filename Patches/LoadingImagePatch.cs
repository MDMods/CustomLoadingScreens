using CustomLoadingScreens.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
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
            private static void Postfix(ref LoadingImg __instance)
            {

                // Rolled a custom image and we have a custom image
                if (ProbabilityManager.UseCustomImage && CustomDataManager.CustomImages.Any())
                {
                    var customImage = ProbabilityManager.GetRandomImage();
                    Il2CppArrayBase<Image> components;

                    __instance.simpleIllus.SetActive(false);
                    __instance.specialIllus.SetActive(false);
                    __instance.verySpecialIllus.SetActive(false);
                    __instance.specialIllusfor43.SetActive(false);

                    // Deal with the specialIllus variables, respective of aspect ratio
                    if (Is43AspectRatio(customImage.Sprite.texture.width, customImage.Sprite.texture.height))
                    {
                        __instance.specialIllusfor43.SetActive(true);
                        components = __instance.specialIllusfor43.GetComponents<Image>();
                    }
                    else
                    {
                        __instance.specialIllus.SetActive(true);
                        components = __instance.simpleIllus.GetComponents<Image>();
                    }

                    // Sets all image components in the Illus to the custom image
                    foreach (var image in components)
                    {
                        image.sprite = customImage.Sprite;
                    }

                    // We can only use bound quotes if that image exists, bound quotes will always be displayed
                    if (customImage.HasBoundQuote)
                    {
                        var randomBound = ProbabilityManager.GetRandomBoundQuote(customImage.Name);
                        foreach (var text in __instance.GetComponentsInChildren<Text>())
                        {
                            text.text = randomBound;
                        }
                        Logger.Msg("Custom loading screen with bound text has been loaded.");
                        return;
                    }
                    Logger.Msg("Custom random loading screen has been loaded.");
                }
                else
                {
                    Logger.Msg("Official loading screen has been loaded.");
                }

                // Did not roll a custom quote
                if (!ProbabilityManager.UseCustomQuote())
                {
                    Logger.Msg("Official loading text has been loaded.");
                    return;
                }

                // Rolled a random quote
                foreach (var text in __instance.GetComponentsInChildren<Text>())
                {
                    text.text = ProbabilityManager.GetRandomQuote();
                }

                Logger.Msg("Custom random loading text has been loaded.");
            }
        }
    }
}
