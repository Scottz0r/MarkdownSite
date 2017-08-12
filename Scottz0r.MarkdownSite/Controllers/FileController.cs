using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scottz0r.MarkdownSite.Services;
using Scottz0r.MarkdownSite.Models;

namespace Scottz0r.MarkdownSite.Controllers
{
    [Route("file")]
    public class FileController : Controller
    {
        private const string TEXT_PLAIN = "text/plain";
        private readonly ILogger _logger;
        private readonly IFileFetcherService _fileFetcherService;

        public FileController(ILogger<FileController> logger, IFileFetcherService fileFetcherService)
        {
            _logger = logger;
            _fileFetcherService = fileFetcherService;
        }

        [HttpGet("{fileName}")]
        public IActionResult Get([FromRoute]string fileName)
        {
            try
            {
                FileFetchResult result = _fileFetcherService.GetFileContent(fileName);
                if(result.State == FileFetchResult.ResultState.Successful)
                {
                    return Content(result.Content, TEXT_PLAIN);
                }
                else if(result.State == FileFetchResult.ResultState.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error while getting file: " + result.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1, ex, "Error while getting file.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("__list")]
        public IActionResult List()
        {
            try
            {
                string[] files = _fileFetcherService.GetFileNames().ToArray();
                ListDto listDto = new ListDto
                {
                    Files = files
                };
                return Ok(listDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(1, ex, "Error while listing files.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
