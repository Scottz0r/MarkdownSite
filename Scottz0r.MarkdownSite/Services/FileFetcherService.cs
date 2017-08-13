using System;
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
        private readonly IFileCacheService _fileCacheService;

        public FileFetcherService(IOptions<MdFileOptions> options, ILogger<FileFetcherService> logger, IIOProxy ioProxy, IFileCacheService fileCacheService)
        {
            _logger = logger;
            _options = options.Value;
            _ioProxy = ioProxy;
            _fileCacheService = fileCacheService;
        }

        public FileFetchResult GetFileContent(string fileName)
        {
            _logger.LogTrace($"Getting markdown content for \"{fileName}\".");

            // If source directory missing, not found.
            if(_options.MDSSourceDirectory == null || !_ioProxy.DirectoryExists(_options.MDSSourceDirectory))
            {
                _logger.LogWarning("Markdown Source directory is not configured or does not exist!");
                return FileFetchResult.Notfound();
            }

            string fileNameWithExtension = fileName + _options.MDSFileExtension;
            string filePath = Path.Combine(_options.MDSSourceDirectory, fileNameWithExtension);

            // Ensure file exists
            if (!_ioProxy.FileExists(filePath))
            {
                return FileFetchResult.Notfound();
            }

            DateTime lastModified = _ioProxy.GetLastWriteTimeUtc(filePath);

            // Cached and valid.
            if (_fileCacheService.HasKey(fileName))
            {
                _logger.LogTrace("File is cached. Comparing modified dates.");
                FileCacheResult cacheResult = _fileCacheService.Get(fileName);

                if(cacheResult.HasValue && cacheResult.FileData.LastModifiedUtc == lastModified)
                {
                    _logger.LogTrace("File has not been modified. Reading content from cache.");

                    return new FileFetchResult(cacheResult.FileData);
                }
            }

            // Cache for first time or refresh.
            _logger.LogTrace($"Caching file from '{filePath}'.");
            string content = _ioProxy.ReadAllText(filePath);
            _fileCacheService.Put(fileName, content, lastModified);

            var fileData = new FileData
            {
                Content = content,
                LastModifiedUtc = lastModified
            };
            return new FileFetchResult(fileData);
        }

        public string[] GetFileNames()
        {
            _logger.LogTrace("Getting file list.");

            // If source directory missing, not found.
            if (_options.MDSSourceDirectory == null || !_ioProxy.DirectoryExists(_options.MDSSourceDirectory))
            {
                _logger.LogWarning("Markdown Source directory is not configured or does not exist!");
                return new string[0];
            }

            var files = _ioProxy.GetFiles(_options.MDSSourceDirectory);
            for(int idx = 0; idx < files.Length; ++idx)
            {
                files[idx] = Path.GetFileNameWithoutExtension(files[idx]);
            }

            return files;
        }
    }
}
