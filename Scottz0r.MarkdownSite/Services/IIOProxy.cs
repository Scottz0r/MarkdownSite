using System;

namespace Scottz0r.MarkdownSite.Services
{
    public interface IIOProxy
    {
        string[] GetFiles(string path);

        DateTime GetLastWriteTimeUtc(string path);

        string ReadAllText(string path);

        bool FileExists(string path);

        bool DirectoryExists(string path);
    }
}
