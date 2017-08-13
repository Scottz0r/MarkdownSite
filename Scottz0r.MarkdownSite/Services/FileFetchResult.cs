using System;

namespace Scottz0r.MarkdownSite.Services
{
    public struct FileFetchResult
    {
        private bool _isFound;
        private FileData _fileData;

        public bool IsFound { get { return _isFound; } }

        public FileData FileData
        {
            get
            {
                if (!_isFound)
                {
                    throw new InvalidOperationException($"{nameof(FileData)} has not been set.");
                }

                return _fileData;
            }
        }

        public FileFetchResult(FileData fileData)
        {
            _fileData = fileData;
            _isFound = fileData != null;
        }

        public static implicit operator FileFetchResult(FileData fileData)
        {
            return new FileFetchResult(fileData);
        }

        public static FileFetchResult Notfound()
        {
            return new FileFetchResult(null);
        }
    }
}
