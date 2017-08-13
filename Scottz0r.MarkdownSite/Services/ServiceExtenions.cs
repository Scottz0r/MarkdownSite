using Microsoft.Extensions.DependencyInjection;
using Scottz0r.MarkdownSite.Services;

namespace Scottz0r.MarkdownSite
{
    public static class ServiceExtenions
    {
        public static void AddMarkdownSite(this IServiceCollection svc)
        {
            svc.AddSingleton<IIOProxy, IOProxy>();
            svc.AddSingleton<IFileCacheService, FileCacheService>();
            svc.AddSingleton<IFileFetcherService, FileFetcherService>();
        }
    }
}
