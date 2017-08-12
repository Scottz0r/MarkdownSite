using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
