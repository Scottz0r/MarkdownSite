using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Scottz0r.MarkdownSite.Options;

namespace Scottz0r.MarkdownSite.Services
{
    public class FileFetcherService : IFileFetcherService
    {
        private readonly ILogger _logger;
        private readonly MdFileOptions _options;
        private readonly IIOProxy _ioProxy;
        private readonly IDictionary<string, FileCacheEntry> _fileCache;

        public FileFetcherService(IOptions<MdFileOptions> options, ILogger<FileFetcherService> logger, IIOProxy ioProxy)
        {
            _logger = logger;
            _options = options.Value;
            _ioProxy = ioProxy;
            _fileCache = new Dictionary<string, FileCacheEntry>();
        }

        public FileFetchResult GetFileContent(string fileName)
        {
            string fileNameWithExtension = fileName + _options.MDSFileExtension;
            string filePath = Path.Combine(_options.MDSSourceDirectory, fileNameWithExtension);

            // Ensure file exists
            if (!_ioProxy.Exists(filePath))
            {
                return FileFetchResult.Notfound();
            }

            DateTime lastModified = _ioProxy.GetLastWriteTimeUtc(filePath);

            // Cached and valid.
            if (_fileCache.ContainsKey(fileName))
            {
                _logger.LogTrace("File is cached. Comparing modified dates.");
                FileCacheEntry entry = _fileCache[fileName];

                if(entry.LastModified == lastModified)
                {
                    _logger.LogTrace("File has not been modified. Reading content from cache.");
                    string cachedContent = _fileCache[fileName].Content;
                    return FileFetchResult.Successful(cachedContent, lastModified);
                }
            }

            // Cache for first time or refresh.
            _logger.LogTrace($"Caching file from '{filePath}'.");
            string content = _ioProxy.ReadAllText(filePath);
            FileCacheEntry newCacheEntry = new FileCacheEntry
            {
                Content = content,
                LastModified = lastModified,
                Name = fileName
            };
            _fileCache[fileName] = newCacheEntry;
            return FileFetchResult.Successful(content, lastModified);
        }

        public IEnumerable<string> GetFileNames()
        {
            _logger.LogTrace("Getting file list.");
            var files = _ioProxy.GetFiles(_options.MDSSourceDirectory);
            foreach(var f in files)
            {
                yield return Path.GetFileNameWithoutExtension(f);
            }
        }
    }
}
