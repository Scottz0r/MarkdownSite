using System;

namespace Scottz0r.MarkdownSite.Services
{
    public interface IFileCacheService
    {
        bool HasKey(string key);

        FileCacheResult Put(string key, string content, DateTime lastModifiedUtc);

        FileCacheResult Get(string key);
    }
}
