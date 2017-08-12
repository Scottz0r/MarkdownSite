using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scottz0r.MarkdownSite.Services
{
    public interface IIOProxy
    {
        string[] GetFiles(string path);

        DateTime GetLastWriteTimeUtc(string path);

        string ReadAllText(string path);

        bool Exists(string path);
    }
}
