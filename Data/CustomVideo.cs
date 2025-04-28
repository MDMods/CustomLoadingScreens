using UnityEngine.Video;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLoadingScreens.Data
{
    internal class CustomVideo
    {
        private static VideoPlayer Player = null;
        private static RenderTexture Texture = null;
        private static readonly Vector3 Vector = new(20, 11.25f);
        private static readonly Utilities.Logger Logger = new(nameof(CustomVideo));

        internal static void Start(GameObject canvas, string videoPath)
        {
            // Create texture
            Texture = new RenderTexture(1920, 1080, 0);
            Texture.Create();

            Logger.Error("Going to play " + videoPath);

            // Create VideoPlayer
            Player = canvas.AddComponent<VideoPlayer>();
            Player.targetTexture = Texture;

            // Set up and play video
            Player.playOnAwake = true;
            Player.skipOnDrop = false;
            Player.audioOutputMode = VideoAudioOutputMode.None;
            Player.aspectRatio = VideoAspectRatio.FitOutside;
            Player.url = videoPath;
            Player.isLooping = true;
            Player.Prepare();
            Player.Play();

            // Set RawImage as a child of transform
            var newObject = new GameObject();
            newObject.transform.SetParent(canvas.transform);
            var rawImage = newObject.AddComponent<RawImage>();
            rawImage.texture = Texture;
            newObject.transform.localScale = Vector;
            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.SetSiblingIndex(2);
        }
    }
}
