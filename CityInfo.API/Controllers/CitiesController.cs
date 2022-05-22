using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;


[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(ILogger<CitiesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<CityDto>> GetAllCities()
    {
        try
        {
            return Ok(CitiesDataStore.Current.Cities);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<CityDto> GetCity(int id)
    {
        try
        {
            var result = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}