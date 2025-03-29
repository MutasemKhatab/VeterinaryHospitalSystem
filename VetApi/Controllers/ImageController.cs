using Microsoft.AspNetCore.Mvc;

namespace VetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly string _imageDirectory;
        private readonly ILogger<ImageController> _logger;
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long FileSizeLimit = 5 * 1024 * 1024; // 5 MB

        public ImageController(IConfiguration configuration, ILogger<ImageController> logger)
        {
            _imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), configuration["ImageUploadPath"] ?? "wwwroot/images");
            _logger = logger;

            if (!Directory.Exists(_imageDirectory))
            {
                Directory.CreateDirectory(_imageDirectory);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile? file, [FromForm] string imageType)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            if (file.Length > FileSizeLimit)
            {
                _logger.LogWarning("File size exceeds the limit.");
                return BadRequest($"File size exceeds the limit of {FileSizeLimit / (1024 * 1024)} MB.");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_permittedExtensions.Contains(extension))
            {
                _logger.LogWarning("Invalid file type.");
                return BadRequest("Invalid file type. Only .jpg, .jpeg, .png, and .gif are allowed.");
            }

            if (string.IsNullOrEmpty(imageType) || (imageType != "vet" && imageType != "pfp"))
            {
                _logger.LogWarning("Invalid image type.");
                return BadRequest("Invalid image type. Only 'vet' and 'pfp' are allowed.");
            }

            var filePath = Path.Combine(_imageDirectory, imageType, Guid.NewGuid() + extension);

            try
            {
                if (!Directory.Exists(Path.Combine(_imageDirectory, imageType)))
                {
                    Directory.CreateDirectory(Path.Combine(_imageDirectory, imageType));
                }

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("images", imageType, Path.GetFileName(filePath));
                _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
                return Ok(relativePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}