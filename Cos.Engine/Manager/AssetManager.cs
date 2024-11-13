using Cos.Audio;
using Cos.Engine.Sprite;
using Newtonsoft.Json;
using NTDLS.DelegateThreadPooling;
using NTDLS.Helpers;
using NTDLS.Semaphore;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Cos.Engine.Manager
{
    public class AssetManager : IDisposable
    {
#if DEBUG
        private const string _assetPackagePath = "../../../../Installer/Cos.Assets.rez";
#else
        private const string _assetPackagePath = "Cos.Assets.rez";
#endif

        private readonly EngineCore _engine;
        private readonly OptimisticCriticalResource<Dictionary<string, object>> _collection = new();
        private readonly IArchive? _archive = null;
        private readonly Dictionary<string, IArchiveEntry> _entryHashes;
        public Dictionary<string, IArchiveEntry> Entries => _entryHashes;

        public AssetManager(EngineCore engine)
        {
            _engine = engine;

            _archive = ArchiveFactory.Open(_assetPackagePath, new SharpCompress.Readers.ReaderOptions()
            {
                ArchiveEncoding = new ArchiveEncoding()
                {
                    Default = System.Text.Encoding.Default
                }
            });

            _entryHashes = _archive.Entries.ToDictionary(item => item.Key.EnsureNotNull().ToLower(), item => item);
        }

        public void Dispose()
        {
            _archive?.Dispose();
        }

        public T GetMetaData<T>(string spriteImagePath)
        {
            string metadataFile = $"{spriteImagePath}.json".Replace('\\', '/'); ;
            string key = $"meta:{metadataFile.ToLower()}";

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(key, out var value);
                return value;
            });

            if (cached != null)
            {
                return (T)cached;
            }

            var metadata = JsonConvert.DeserializeObject<T>(GetText(metadataFile)).EnsureNotNull();
            _collection.Write(o => o.Add(key, metadata ?? throw new NullReferenceException()));
            return metadata;
        }

        /// <summary>
        /// Gets and caches a text files content from the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public static string GetUserText(string assetRelativePath, string defaultText = "")
        {
            assetRelativePath = assetRelativePath.ToLower();

            var userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Strikeforce Infinite");
            if (Directory.Exists(userDataPath) == false)
            {
                Directory.CreateDirectory(userDataPath);
            }
            string assetAbsolutePath = Path.Combine(userDataPath, assetRelativePath).Trim().Replace("\\", "/");
            if (File.Exists(assetAbsolutePath) == false)
            {
                return defaultText;
            }

            return File.ReadAllText(assetAbsolutePath);
        }

        /// <summary>
        /// Saves and caches a text file into the asset path.
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <param name="value"></param>
        public static void PutUserText(string assetRelativePath, string value)
        {
            assetRelativePath = assetRelativePath.ToLower();

            var userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Strikeforce Infinite");
            if (Directory.Exists(userDataPath) == false)
            {
                Directory.CreateDirectory(userDataPath);
            }
            string assetAbsolutePath = Path.Combine(userDataPath, assetRelativePath).Trim().Replace("\\", "/");
            File.WriteAllText(assetAbsolutePath, value);
        }

        public string GetText(string path, string defaultText = "")
        {
            path = path.ToLower().Replace('\\', '/');

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out object? value);
                return value as string;
            });
            if (cached != null)
            {
                return cached;
            }

            try
            {
                var text = GetCompressedText(path);
                _collection.Write(o => o.TryAdd(path, text));
                return text;
            }
            catch
            {
                return defaultText;
            }
        }

        public CosAudioClip GetAudio(string path, float initialVolume = 1, bool loopForever = false)
        {
            path = path.ToLower().Replace('\\', '/');

            var cacheKey = $"{path}:{initialVolume}:{loopForever}";
            var cached = _collection.Read(o =>
            {
                if (o.TryGetValue(cacheKey, out object? value))
                {
                    ((CosAudioClip)value).SetInitialVolume(initialVolume);
                    ((CosAudioClip)value).SetLoopForever(loopForever);
                    return (CosAudioClip)value;
                }
                return null;
            });

            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var result = new CosAudioClip(stream, initialVolume, loopForever);
            _collection.Write(o => o.TryAdd(cacheKey, result));
            stream.Close();
            return result;
        }

        public SharpDX.Direct2D1.Bitmap GetBitmap(string path)
        {
            path = path.ToLower().Replace('\\', '/');

            var cached = _collection.Read(o =>
            {
                o.TryGetValue(path, out object? value);
                return value as SharpDX.Direct2D1.Bitmap;
            });

            if (cached != null)
            {
                return cached;
            }

            using var stream = GetCompressedStream(path);
            var bitmap = _engine.Rendering.BitmapStreamToD2DBitmap(stream);
            _collection.Write(o => o.TryAdd(path, bitmap));
            return bitmap;
        }

        public void HydrateCache(SpriteTextBlock loadingHeader, SpriteTextBlock loadingDetail)
        {
            loadingHeader.SetTextAndCenterX("Hydrating asset cache...");

            using var archive = ArchiveFactory.Open(_assetPackagePath);
            using var dtp = new DelegateThreadPool(Environment.ProcessorCount);
            var threadPoolTracker = dtp.CreateChildQueue();

            int statusIndex = 0;
            float statusEntryCount = archive.Entries.Count();

            foreach (var entry in archive.Entries)
            {
                switch (Path.GetExtension(entry.Key.EnsureNotNull()).ToLower())
                {
                    case ".json":
                    case ".txt":
                        threadPoolTracker.Enqueue(() => GetText(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                        break;
                    case ".png":
                    case ".jpg":
                    case ".bmp":
                        threadPoolTracker.Enqueue(() => GetBitmap(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                        break;
                    case ".wav":
                        threadPoolTracker.Enqueue(() => GetAudio(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                        break;
                    default:
                        Interlocked.Increment(ref statusIndex);
                        break;
                }
            }

            threadPoolTracker.WaitForCompletion(TimeSpan.FromSeconds(60), () =>
            {
                loadingDetail.SetTextAndCenterX($"{statusIndex / statusEntryCount * 100.0:n0}%");
                return true;
            });

            loadingDetail.SetTextAndCenterX($"100%");
        }

        private string GetCompressedText(string path)
        {
            using var stream = GetCompressedStream(path);
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        private MemoryStream GetCompressedStream(string path)
        {
            path = path.Trim().Replace("\\", "/");

            if (_entryHashes.TryGetValue(path, out var entry))
            {
                lock (_entryHashes)
                {
                    using var stream = entry.OpenEntryStream();
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream;
                }
            }

            throw new FileNotFoundException(path);
        }
    }
}
