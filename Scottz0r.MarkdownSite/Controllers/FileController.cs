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
        private const string GENERIC_ERROR_MSG = "Error while processing request.";
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
                    FileDto dto = new FileDto
                    {
                        LastModifiedUtc = result.LastModifiedUtc,
                        Content = result.Content
                    };
                    return CustomOk(dto);
                }
                else if(result.State == FileFetchResult.ResultState.NotFound)
                {
                    return CustomNotFound();
                }
                else
                {
                    _logger.LogError("Error while getting file: " + result.Message);
                    return CustomInternalServerError(result.Message);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(1, ex, GENERIC_ERROR_MSG);
                return CustomInternalServerError(GENERIC_ERROR_MSG);
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
                return CustomOk(listDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(1, ex, GENERIC_ERROR_MSG);
                return CustomInternalServerError(GENERIC_ERROR_MSG);
            }
        }

        private IActionResult CustomOk(object data)
        {
            DtoWrapper goodResult = new DtoWrapper
            {
                Id = HttpContext.TraceIdentifier,
                Data = data
            };
            return StatusCode(StatusCodes.Status200OK, goodResult);
        }

        private IActionResult CustomNotFound()
        {
            DtoWrapper errorResult = new DtoWrapper
            {
                Id = HttpContext.TraceIdentifier,
                Data = null,
                Error = new ErrorData
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Not found."
                }
            };
            return StatusCode(StatusCodes.Status404NotFound, errorResult);
        }

        private IActionResult CustomInternalServerError(string message)
        {
            DtoWrapper errorResult = new DtoWrapper
            {
                Id = HttpContext.TraceIdentifier,
                Data = null,
                Error = new ErrorData
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = message
                }
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
        }
    }
}
