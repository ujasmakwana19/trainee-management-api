using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using TraineeManagement.Api.ErrorCodesUtils;
using TraineeManagement.Api.ExceptionUtils;
using TraineeManagement.Api.FileServices;

namespace TraineeManagement.Api.FileController
{
    [Authorize]
    [ApiController]
    [Route("api/submission-files")]
    public class FileController : ControllerBase
    {
        private int BufferSize;// 16 MB buffer size

        private readonly ILogger<FileController> _logger;
        private readonly FileManagerService _fileManager;
        private readonly IConfiguration _config;

        public FileController(
            ILogger<FileController> logger,
            IConfiguration config,
            FileManagerService fileManager)
        {
            
            _logger = logger;
            _config = config;
            _fileManager = fileManager;
            BufferSize = int.Parse(_config["StorageSettings:Buffer_Size"] ?? "16777216"); // Default 16 MB
        }

        [HttpPost]
        [Route("multipart")]
        public async Task<IActionResult> UploadMultipartReader()
        {
            if (!Request.ContentType?.StartsWith("multipart/form-data") ?? true)
            {
                throw new BadRequestException(ErrorCodes.INVALID_MODEL);
            }

            string? boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(Request.ContentType).Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new BadRequestException(ErrorCodes.INVALID_MODEL);
            }

            CancellationToken cancellationToken = HttpContext.RequestAborted;
            string filePath = await _fileManager.SaveViaMultipartReaderAsync(boundary, Request.Body, cancellationToken);
            return Ok("Saved file at " + filePath);
        }

        // [HttpPost]
        // [Route("pipe")]
        // public async Task<IActionResult> UploadPipeReader()
        // {
        //     if (!Request.HasFormContentType)
        //     {
        //         return BadRequest("The request does not contain a valid form.");
        //     }

        //     var cancellationToken = HttpContext.RequestAborted;
        //     var filePath = await _fileManager.SaveViaPipeReaderAsync(Request.BodyReader, cancellationToken);
        //     return Ok("Saved file at " + filePath);
        // }

        // [HttpPost]
        // [Route("form")]
        // public async Task<IActionResult> ReadForms()
        // {
        //     if (!Request.HasFormContentType)
        //     {
        //         return BadRequest("The request does not contain a valid form.");
        //     }

        //     var cancellationToken = HttpContext.RequestAborted;
        //     var formFeature = Request.HttpContext.Features.GetRequiredFeature<IFormFeature>();
        //     await formFeature.ReadFormAsync(cancellationToken);

        //     var filePath = Request.Form.Files.First().FileName;
        //     return Ok("Saved file at " + filePath);
        // }
    }
}