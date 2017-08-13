using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Scottz0r.MarkdownSite
{
    public class SpaMiddleware
    {
        private readonly RequestDelegate _next;

        public SpaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value) && !context.Request.Path.StartsWithSegments("/file"))
            {
                // Set path to index and re-process request (call _next again).
                context.Request.Path = "/index.html";
                await _next(context);
            }
        }
    }
}
