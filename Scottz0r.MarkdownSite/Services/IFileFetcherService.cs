using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scottz0r.MarkdownSite.Services
{
    public interface IFileFetcherService
    {
        FileFetchResult GetFileContent(string fileName);

        IEnumerable<string> GetFileNames();
    }
}
