using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Moq;
using Scottz0r.MarkdownSite.Services;
using Scottz0r.MarkdownSite.Options;
using System.IO;

namespace Scottz0r.MarkdownSite.Tests
{
    public class FileFetcherService_Tests
    {
        // These fields are for keeping these tests cross platform friendly, since
        // the path seperator will be different between *nix and windows.
        private static readonly char DSC = Path.DirectorySeparatorChar;
        private static readonly string SLASH_MD = $"{DSC}md";
        private static readonly string MD_SLASH_TEST_DOT_MD = Path.Combine(SLASH_MD, "test.md");

        Mock<IOptions<MdFileOptions>> _options;
        Mock<ILogger<FileFetcherService>> _logger;

        public FileFetcherService_Tests()
        {
            _options = new Mock<IOptions<MdFileOptions>>();
            _logger = new Mock<ILogger<FileFetcherService>>();

            _options.SetupGet(x => x.Value).Returns(new MdFileOptions { MDSSourceDirectory = SLASH_MD, MDSFileExtension = ".md" });
        }

        [Fact]
        public void It_should_get_files_names_without_extensions()
        {
            // Arrange
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.GetFiles(SLASH_MD)).Returns(new[] { "test.md", "other.md" });
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(true);
            var cacheService = new Mock<IFileCacheService>();
            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            string[] actual = service.GetFileNames().ToArray();

            // Assert
            Assert.Equal("test", actual[0]);
            Assert.Equal("other", actual[1]);
        }

        [Fact]
        public void It_should_return_empty_array_directory_null()
        {
            // Arrange
            var badOptions = new Mock<IOptions<MdFileOptions>>();
            badOptions.SetupGet(x => x.Value).Returns(new MdFileOptions { MDSSourceDirectory = null });
            var cacheService = new Mock<IFileCacheService>();
            var ioProxy = new Mock<IIOProxy>();
            var service = new FileFetcherService(badOptions.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            string[] actual = service.GetFileNames();

            // Assert
            Assert.Equal(0, actual.Length);
        }

        [Fact]
        public void It_should_empty_array_directory_not_found()
        {
            // Arrange
            var cacheService = new Mock<IFileCacheService>();
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(false);
            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            string[] actual = service.GetFileNames();

            // Assert
            Assert.Equal(0, actual.Length);
        }

        [Fact]
        public void It_should_load_and_cache_file()
        {
            // Arrange
            string filePath = MD_SLASH_TEST_DOT_MD;
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.FileExists(filePath)).Returns(true);
            ioProxy.Setup(x => x.GetLastWriteTimeUtc(filePath)).Returns(new DateTime(2017, 1, 1));
            ioProxy.Setup(x => x.ReadAllText(filePath)).Returns("# Test");
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(true);

            var cacheService = new Mock<IFileCacheService>();
            cacheService.Setup(x => x.HasKey("test")).Returns(false);

            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.True(actual.IsFound);
            Assert.Equal("# Test", actual.FileData.Content);
        }

        [Fact]
        public void It_should_read_from_cache()
        {
            // Arrange
            string filePath = MD_SLASH_TEST_DOT_MD;
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.FileExists(filePath)).Returns(true);
            ioProxy.Setup(x => x.GetLastWriteTimeUtc(filePath)).Returns(new DateTime(2017, 1, 1));
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(true);

            var cacheService = new Mock<IFileCacheService>();
            cacheService.Setup(x => x.HasKey("test")).Returns(true);
            cacheService.Setup(x => x.Get("test")).Returns(new FileData { Content = "# Test", LastModifiedUtc = new DateTime(2017, 1, 1) });

            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.True(actual.IsFound);
            Assert.Equal("# Test", actual.FileData.Content);
        }

        [Fact]
        public void It_should_recache_file_newer()
        {
            // Arrange
            string filePath = MD_SLASH_TEST_DOT_MD;
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.FileExists(filePath)).Returns(true);
            ioProxy.Setup(x => x.GetLastWriteTimeUtc(filePath)).Returns(new DateTime(2017, 2, 1));
            ioProxy.Setup(x => x.ReadAllText(filePath)).Returns("# New");
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(true);

            var cacheService = new Mock<IFileCacheService>();
            cacheService.Setup(x => x.HasKey("test")).Returns(true);
            cacheService.Setup(x => x.Get("test")).Returns(new FileData { Content = "# Test", LastModifiedUtc = new DateTime(2017, 1, 1) });

            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.True(actual.IsFound);
            Assert.Equal("# New", actual.FileData.Content);
        }

        [Fact]
        public void It_should_return_not_found_file_doesnt_exist()
        {
            // Arrange
            string filePath = MD_SLASH_TEST_DOT_MD;
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.FileExists(filePath)).Returns(false);
            ioProxy.Setup(x => x.GetLastWriteTimeUtc(filePath)).Throws(new FileNotFoundException());
            ioProxy.Setup(x => x.ReadAllText(filePath)).Throws(new FileNotFoundException());
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(true);

            var cacheService = new Mock<IFileCacheService>();
            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.False(actual.IsFound);
        }

        [Fact]
        public void It_should_return_not_found_directory_null()
        {
            // Arrange
            var badOptions = new Mock<IOptions<MdFileOptions>>();
            badOptions.SetupGet(x => x.Value).Returns(new MdFileOptions { MDSSourceDirectory = null });
            var cacheService = new Mock<IFileCacheService>();
            var ioProxy = new Mock<IIOProxy>();
            var service = new FileFetcherService(badOptions.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.False(actual.IsFound);
        }

        [Fact]
        public void It_should_return_not_found_directory_not_found()
        {
            // Arrange
            var cacheService = new Mock<IFileCacheService>();
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(false);
            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.False(actual.IsFound);
        }

        [Fact]
        public void It_should_recache_cache_miss()
        {
            // Arrange
            string filePath = MD_SLASH_TEST_DOT_MD;
            var ioProxy = new Mock<IIOProxy>();
            ioProxy.Setup(x => x.FileExists(filePath)).Returns(true);
            ioProxy.Setup(x => x.GetLastWriteTimeUtc(filePath)).Returns(new DateTime(2017, 1, 1));
            ioProxy.Setup(x => x.ReadAllText(filePath)).Returns("# Test");
            ioProxy.Setup(x => x.DirectoryExists(SLASH_MD)).Returns(true);

            var cacheService = new Mock<IFileCacheService>();
            cacheService.Setup(x => x.HasKey("test")).Returns(true);
            cacheService.Setup(x => x.Get("test")).Returns(FileCacheResult.Empty());

            var service = new FileFetcherService(_options.Object, _logger.Object, ioProxy.Object, cacheService.Object);

            // Act
            FileFetchResult actual = service.GetFileContent("test");

            // Assert
            Assert.True(actual.IsFound);
            Assert.Equal("# Test", actual.FileData.Content);
        }
    }
}
