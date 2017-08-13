using System;
using System.IO;

namespace Scottz0r.MarkdownSite.Services
{
    public class IOProxy : IIOProxy
    {
        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
