namespace Scottz0r.MarkdownSite.Services
{
    public interface IFileFetcherService
    {
        FileFetchResult GetFileContent(string fileName);

        string[] GetFileNames();
    }
}
