using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scottz0r.MarkdownSite
{
    public class FileCacheEntry
    {
        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime LastModified { get; set; }
    }
}
