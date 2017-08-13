using System;

namespace Scottz0r.MarkdownSite.Services
{
    public class FileFetchResult
    {
        public enum ResultState
        {
            Successful,
            NotFound,
            Exception
        }

        public ResultState State { get; set; }

        public string Content { get; set; }

        public DateTime LastModifiedUtc { get; set; }

        public string Message { get; set; }

        public static FileFetchResult Notfound()
        {
            return new FileFetchResult
            {
                State = ResultState.NotFound,
                Message = "Not Found"
            };
        }

        public static FileFetchResult Successful(string content, DateTime lastModifiedUtc)
        {
            return new FileFetchResult
            {
                State = ResultState.Successful,
                Content = content,
                LastModifiedUtc = lastModifiedUtc
            };
        }
    }
}
