using System;
using System.Collections.Concurrent;
using System.IO;

namespace IlluminatedComments
{
    internal class ImageCache
    {
        private static readonly Lazy<ImageCache> _instance = new Lazy<ImageCache>(() => new ImageCache());
        private readonly ConcurrentDictionary<string, string> _imageMap = new ConcurrentDictionary<string, string>();
        private ImageCache() { }

        public static ImageCache Instance => _instance.Value;

        ~ImageCache()
        {
            foreach (var path in _imageMap.Values)
            {
                try
                {
                    File.Delete(path);
                }
                catch { }
            }
        }

        public void Add(string uri, string local)
        {
            _imageMap.GetOrAdd(uri, local);
        }

        public bool TryGetValue(string uri, out string local) => _imageMap.TryGetValue(uri, out local);
    }
}