using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly ILogger<FilesController> _logger;
    private readonly FileExtensionContentTypeProvider _typeProvider;

    public FilesController(ILogger<FilesController> logger, FileExtensionContentTypeProvider typeProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
    }

    [HttpGet("{extension}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetFile(string extension)
    {
        try
        {
            var file = FilesDataStore.Files.FirstOrDefault(x => x.Extension == extension || x.Extension == "." + extension);

            if (file is null)
            {
                return NotFound();
            }

            if (!_typeProvider.TryGetContentType(file.FullName, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(file.FullName);

            return File(fileBytes, contentType, file.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
