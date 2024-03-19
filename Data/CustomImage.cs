using CustomLoadingScreens.Managers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using UnityEngine;
using Logger = CustomLoadingScreens.Utilities.Logger;

namespace CustomLoadingScreens.Data
{
    internal class CustomImage
    {
        private readonly Configuration _config = Configuration.Default;

        internal readonly string Name;
        internal readonly List<Sprite> Sprites;
        internal readonly int FramesPerSecond;
        internal readonly int FrameCount;

        internal CustomImage(string path)
        {
            Sprites = new List<Sprite>();
            var logger = new Logger(nameof(CustomImage));
            _config.PreferContiguousImageBuffers = true;
            
            Name = Path.GetFileNameWithoutExtension(path);

            // Unity loads textures upside-down so flip it
            var image = Image.Load<Rgba32>(path);
            image.Mutate(c => c.Flip(FlipMode.Vertical));

            FramesPerSecond = image.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay * 10;
            FrameCount = image.Frames.Count;

            foreach (var frame in image.Frames)
            {
                var texture = new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false);

                var getPixelDataResult = frame.DangerousTryGetSinglePixelMemory(out var memory);
                if (!getPixelDataResult)
                {
                    logger.Error("Failed to get pixel data.");
                    return;
                }

                using var handle = memory.Pin();

                // Ugly unsafe block to save a hard copy of memory for performance
                unsafe { texture.LoadRawTextureData((IntPtr)handle.Pointer, memory.Length * sizeof(IntPtr)); }

                texture.Apply(false);

                Sprites.Add( Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
                Sprites.Last().hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }

            logger.Msg("Created sprite.");
        }
        internal bool HasBoundQuote => CustomDataManager.BoundQuotes.ContainsKey(Name);
    }
}
