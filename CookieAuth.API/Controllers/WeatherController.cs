using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CookieAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly List<Location> _locations = new()
        {
            new Location { Id = 1, Name = "New York", Country = "USA", Latitude = 40.7128, Longitude = -74.0060 },
            new Location { Id = 2, Name = "London", Country = "UK", Latitude = 51.5074, Longitude = -0.1278 },
            new Location { Id = 3, Name = "Tokyo", Country = "Japan", Latitude = 35.6762, Longitude = 139.6503 },
            new Location { Id = 4, Name = "Sydney", Country = "Australia", Latitude = -33.8688, Longitude = 151.2093 },
            new Location { Id = 5, Name = "Paris", Country = "France", Latitude = 48.8566, Longitude = 2.3522 }
        };

        // GET: api/Weather
        [HttpGet]
        public IActionResult GetForecast()
        {
            var forecast = GetRandomWeatherData();
            return Ok(forecast);
        }

        // GET: api/Weather/5/forecast
        [HttpGet("{id}/forecast")]
        public IActionResult GetForecastByLocation(int id)
        {
            var location = _locations.FirstOrDefault(l => l.Id == id);
            if (location == null)
            {
                return NotFound(new { Message = $"Location with ID {id} not found" });
            }

            var forecast = GetRandomWeatherData(location, 5);
            return Ok(new
            {
                Location = location,
                Forecast = forecast
            });
        }

        // GET: api/Weather/locations
        [HttpGet("locations")]
        public IActionResult GetLocations()
        {
            return Ok(_locations);
        }

        // GET: api/Weather/location/2
        [HttpGet("location/{id}")]
        public IActionResult GetLocation(int id)
        {
            var location = _locations.FirstOrDefault(l => l.Id == id);
            if (location == null)
            {
                return NotFound(new { Message = $"Location with ID {id} not found" });
            }

            return Ok(location);
        }

        // POST: api/Weather/search
        [HttpPost("search")]
        public IActionResult SearchWeather([FromBody] WeatherSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Filter locations based on search criteria
            var matchingLocations = _locations
                .Where(l => string.IsNullOrEmpty(request.LocationName) || 
                           l.Name.Contains(request.LocationName, StringComparison.OrdinalIgnoreCase) ||
                           l.Country.Contains(request.LocationName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!matchingLocations.Any())
            {
                return NotFound(new { Message = "No locations found matching your criteria" });
            }

            // Generate forecast data for matching locations
            var results = matchingLocations.Select(location => new
            {
                Location = location,
                Forecast = GetRandomWeatherData(location, request.Days ?? 3)
            }).ToList();

            return Ok(results);
        }

        // GET: api/Weather/secure-forecast
        [HttpGet("secure-forecast")]
        [Authorize]
        public IActionResult GetSecureForecast()
        {
            var userName = User.Identity?.Name ?? "Unknown User";
            var forecast = GetRandomWeatherData();
            
            return Ok(new
            {
                Message = $"Hello {userName}! Here's your personalized weather forecast.",
                UserSpecificForecast = forecast
            });
        }

        private List<WeatherForecast> GetRandomWeatherData(Location? location = null, int days = 5)
        {
            var rng = new Random();
            return Enumerable.Range(1, days).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                LocationName = location?.Name ?? "Default Location",
                Humidity = rng.Next(30, 95),
                WindSpeed = Math.Round(rng.NextDouble() * 30, 1)
            }).ToList();
        }
    }

    // Model classes for the WeatherController
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class WeatherSearchRequest
    {
        public string? LocationName { get; set; }
        public int? Days { get; set; }
    }
}