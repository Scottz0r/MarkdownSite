using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Scottz0r.MarkdownSite.Services
{
    public class FileCacheService : IFileCacheService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, FileData> _fileCache;

        public FileCacheService(ILogger<FileCacheService> logger)
        {
            _logger = logger;
            _fileCache = new ConcurrentDictionary<string, FileData>();
        }

        public bool HasKey(string key)
        {
            return _fileCache.ContainsKey(key);
        }

        public FileCacheResult Put(string key, string content, DateTime lastModifiedUtc)
        {
            _logger.LogTrace($"Adding/updating \"{key}\" in cache.");
            var cacheEntry = new FileData
            {
                Content = content,
                LastModifiedUtc = lastModifiedUtc
            };

            return _fileCache.AddOrUpdate(key, cacheEntry, (k, existing) =>
            {
                if (existing.LastModifiedUtc > cacheEntry.LastModifiedUtc)
                {
                    _logger.LogTrace($"Existing cache entry for \"{key}\" has more recent modified date. Keeping existing.");
                    return existing;
                }

                return cacheEntry;
            });
        }

        public FileCacheResult Get(string key)
        {
            _logger.LogTrace($"Fetching key \"{key}\" from cache.");
            if(_fileCache.TryGetValue(key, out FileData val))
            {
                return val;
            }

            _logger.LogWarning($"Cache miss on key \"{key}\".");
            return FileCacheResult.Empty();
        }
    }
}
