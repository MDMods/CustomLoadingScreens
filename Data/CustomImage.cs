using System.IO.Compression;
using System.Runtime.CompilerServices;
using CustomAlbums.Utilities;
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
        private readonly int _size = Unsafe.SizeOf<Rgba32>();

        internal readonly string Name;
        internal readonly List<Sprite> Sprites;
        internal readonly int FramesPerSecond;
        internal readonly int FrameCount;
        internal readonly bool IsVideo;
        internal readonly string VideoPath;
        internal static readonly Logger Logger = new(nameof(CustomImage));

        internal CustomImage(string zipPath, string loadingName)
        {
            Sprites = new List<Sprite>();
            _config.PreferContiguousImageBuffers = true;

            Name = Path.GetFileNameWithoutExtension(zipPath);
            var extension = Path.GetExtension(loadingName).ToLowerInvariant();
            if (extension is ".webm" or ".mp4")
            {
                IsVideo = true;
                VideoPath = $"{Application.dataPath}/{Path.GetFileName(zipPath)}{extension}";
                File.WriteAllBytes(VideoPath, ZipFile.OpenRead(zipPath).GetEntry(loadingName)!.Open().ToMemoryStream().ToArray());
                Logger.Msg("Loaded a video from an MDM!");
                return;
            }

            // Unity loads textures upside-down so flip it
            // We will have checked that this is non-null beforehand
            var image = Image.Load<Rgba32>(ZipFile.OpenRead(zipPath).GetEntry(loadingName)!.Open());
            image.Mutate(c => c.Flip(FlipMode.Vertical));

            FramesPerSecond = image.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay * 10;
            FrameCount = image.Frames.Count;

            foreach (var frame in image.Frames)
            {
                var texture = new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false);

                var getPixelDataResult = frame.DangerousTryGetSinglePixelMemory(out var memory);
                if (!getPixelDataResult)
                {
                    Logger.Error("Failed to get pixel data.");
                    return;
                }

                using var handle = memory.Pin();

                // Ugly unsafe block to save a hard copy of memory for performance
                unsafe { texture.LoadRawTextureData((IntPtr)handle.Pointer, memory.Length * _size); }

                texture.Apply(false);

                Sprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
                Sprites.Last().hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }

            Logger.Msg("Created sprite.");
        }

        internal CustomImage(string path)
        {

            Sprites = new List<Sprite>();
            _config.PreferContiguousImageBuffers = true;
            
            Name = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path).ToLowerInvariant();
            if (extension is ".webm" or ".mp4")
            {
                IsVideo = true;
                VideoPath = path;
                Logger.Msg("Loaded a video from a folder!");
                return;
            }

            // Unity loads textures upside-down so flip it
            using var image = Image.Load<Rgba32>(path);
            image.Mutate(c => c.Flip(FlipMode.Vertical));

            FramesPerSecond = image.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay * 10;
            FrameCount = image.Frames.Count;

            foreach (var frame in image.Frames)
            {
                var texture = new Texture2D(frame.Width, frame.Height, TextureFormat.RGBA32, false);

                var getPixelDataResult = frame.DangerousTryGetSinglePixelMemory(out var memory);
                if (!getPixelDataResult)
                {
                    Logger.Error("Failed to get pixel data.");
                    return;
                }

                using var handle = memory.Pin();

                // Ugly unsafe block to save a hard copy of memory for performance
                unsafe { texture.LoadRawTextureData((IntPtr)handle.Pointer, memory.Length * _size); }

                texture.Apply(false);

                Sprites.Add( Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
                Sprites.Last().hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }

            Logger.Msg("Created sprite.");
        }
        internal bool HasBoundQuote => CustomDataManager.BoundQuotes.ContainsKey(Name);
    }
}
