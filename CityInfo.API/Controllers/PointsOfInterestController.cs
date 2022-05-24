using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities/{cityId}/pointsofinterest")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PointsOfInterestController : ControllerBase
{
    private readonly CitiesDataStore _citiesDataStore;
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;

    public PointsOfInterestController(CitiesDataStore citiesDataStore, ILogger<PointsOfInterestController> logger, IMailService mailService)
    {
        _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        try
        {
            var cityDto = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

            if (cityDto is null)
            {
                _logger.LogInformation($"The city with ID of ({cityId}) was not found.");
                return NotFound();
            }

            return Ok(cityDto.PointsOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        try
        {
            var cityDto = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

            if (cityDto is null)
            {
                _logger.LogInformation($"The city with ID of ({cityId}) was not found.");
                return NotFound();
            }

            if (cityDto.PointsOfInterest is null)
            {
                _logger.LogInformation($"The city with ID of ({cityId}) was found, but the PointOfInterest is null.");
                return NotFound();
            }

            var pointOfInterest = cityDto.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterest is null)
            {
                _logger.LogInformation($"The city with ID of ({cityId}) was found, but the point of " +
                    $"interest with ID of ({pointOfInterestId}) was not found.");
                return NotFound();
            }

            return Ok(pointOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<PointOfInterestDto> AddPointOfInterest(int cityId, PointOfInterestToCreateDto dto)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city is null)
            {
                return NotFound();
            }

            var newId = CitiesDataStore.Current.Cities
                .SelectMany(c => c.PointsOfInterest!)
                .Max(p => p.Id) + 1;

            var pointOfInterest = new PointOfInterestDto
            {
                Id = newId,
                Name = dto.Name,
                Description = dto.Description
            };

            city.PointsOfInterest!.Add(pointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = pointOfInterest.Id
                }, pointOfInterest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{pointofinterestid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestToUpdateDto dto)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest!.FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterestFromStore is null)
            {
                return NotFound();
            }

            pointOfInterestFromStore.Name = dto.Name;
            pointOfInterestFromStore.Description = dto.Description;

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPatch("{pointofinterestid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestToUpdateDto> patchDocument)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest!.FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterestFromStore is null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestToUpdateDto
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{pointofinterestid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        try
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city is null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest!.FirstOrDefault(x => x.Id == pointOfInterestId);

            if (pointOfInterestFromStore is null)
            {
                return NotFound();
            }

            city.PointsOfInterest!.Remove(pointOfInterestFromStore);

            await _mailService.Send($"CityInfo Log: {nameof(DeletePointOfInterest)}",
                $"Point of interest deleted with name: {pointOfInterestFromStore.Name}" +
                $" and ID: {pointOfInterestFromStore.Id}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
