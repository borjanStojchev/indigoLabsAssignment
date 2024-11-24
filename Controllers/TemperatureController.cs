using IndigoLabsAssignment.Authentication;
using IndigoLabsAssignment.Enums;
using IndigoLabsAssignment.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IndigoLabsAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    public class TemperatureController : ControllerBase
    {
        private readonly ITemperatureService _temperatureService;

        public TemperatureController(ITemperatureService temperatureService)
        {
            _temperatureService = temperatureService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetCityAverages()
        {
            var cityAverages = await _temperatureService.GetCityAverages();
            return Ok(cityAverages);
        }

        [HttpGet("{cityName}")]
        public async Task<IActionResult> GetAverageTemperatureForCity(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return BadRequest("City name must be provided.");
            }
            try
            {
                var cityTemperature = await _temperatureService.GetAverageTemperatureForCity(cityName);
                return Ok(cityTemperature);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterCitiesByAverageTemperature([FromQuery] double threshold, [FromQuery] FilterType filterType)
        {
            try
            {
                var filteredCities = await _temperatureService.FilterCitiesByAverageTemperature(threshold, filterType);
                return Ok(filteredCities);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
