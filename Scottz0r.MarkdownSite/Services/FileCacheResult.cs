using System;

namespace Scottz0r.MarkdownSite.Services
{
    public struct FileCacheResult
    {
        private FileData _cacheEntry;
        private bool _hasValue;

        public bool HasValue
        {
            get { return _hasValue; }
        }

        public FileData FileData
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException($"{nameof(FileData)} has not been assigned.");
                }

                return _cacheEntry;
            }
        }

        public FileCacheResult(FileData cacheEntry)
        {
            _cacheEntry = cacheEntry;
            _hasValue = cacheEntry != null;
        }

        public static implicit operator FileCacheResult(FileData val)
        {
            return new FileCacheResult(val);
        }

        public static FileCacheResult Empty()
        {
            return new FileCacheResult(null);
        }
    }
}
