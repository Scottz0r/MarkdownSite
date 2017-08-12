using Microsoft.Extensions.DependencyInjection;
using Scottz0r.MarkdownSite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scottz0r.MarkdownSite
{
    public static class ServiceExtenions
    {
        public static void AddMarkdownSite(this IServiceCollection svc)
        {
            svc.AddSingleton<IIOProxy, IOProxy>();
            svc.AddSingleton<IFileFetcherService, FileFetcherService>();
        }
    }
}
