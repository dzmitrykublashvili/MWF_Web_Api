using Microsoft.AspNetCore.Mvc;
using MWF_Web_Api.Models;
using MWF_Web_Api.Services;
using MWF_Web_Api.Utility;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.VisualBasic;

namespace MWF_Web_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : Controller
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly ImagesService _imageService;

        public ImagesController(ILogger<ImagesController> logger, ImagesService imageService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm]IFormFile img, [FromQuery]int id, CancellationToken cancellationToken)
        {
            long length = img.Length;
            byte[] bytes;

            try
            {
                if (length > 0)
                {
                    await using var stream = img.OpenReadStream();
                    bytes = new byte[length];
                    _ = await stream.ReadAsync(bytes, 0, (int)img.Length, cancellationToken);

                    var extensionStr = Path.GetExtension(img.FileName);
                    var extension = _imageService.GetImageFileExtension(extensionStr);

                    var image = new Image(bytes, extension);

                    var result = await _imageService.SaveImage(image, id, cancellationToken);

                    return result.Value ? Ok() : BadRequest(result.Message);
                }

                return BadRequest("Empty message file.");
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }

        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery]int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _imageService.GetImage(id, cancellationToken);

                if (result.IsSuccessful && result.Value is not null)
                {
                    return File(result.Value, "image/jpg");
                }

                return NotFound();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
