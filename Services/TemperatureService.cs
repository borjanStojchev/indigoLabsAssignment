using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IndigoLabsAssignment.Enums;
using IndigoLabsAssignment.Interfaces;
using IndigoLabsAssignment.Models;
using Microsoft.Extensions.Caching.Memory;
using Models.IndigoLabsAssignment;

namespace IndigoLabsAssignment.Services;

public class TemperatureService : ITemperatureService
{
    private readonly Settings _settings;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "cityAverages";

    public TemperatureService(IMemoryCache cache, Settings settings)
    {
        _cache = cache;
        _settings = settings;
    }

    public async Task<List<CityTemperature>> GetCityAverages()
    {
        return GetCachedData();
    }

    public async Task<List<CityTemperature>> CalculateCityAverages()
    {
        var cityData = new ConcurrentDictionary<string, (double Total, int Count)>();
        int batchSize = 1000;

        using (var reader = new StreamReader(_settings.DataPath))
        {
            while (!reader.EndOfStream)
            {
                var lines = new List<string>();
                for (int i = 0; i < batchSize && !reader.EndOfStream; i++)
                {
                    lines.Add(await reader.ReadLineAsync());
                }

                Parallel.ForEach(lines, line =>
                {
                    var fields = line.Split(';');
                    var city = fields[0];
                    var temperature = double.Parse(fields[1]);

                    cityData.AddOrUpdate(city,
                        (temperature, 1),
                        (key, existing) => (existing.Total + temperature, existing.Count + 1));
                });
            }

            var averages = cityData
                    .Select(city => new CityTemperature() { CityName = city.Key, Temperature = city.Value.Total / city.Value.Count })
                    .ToList();

            _cache.Set(CacheKey, averages);
            return averages;
        }
    }

    public async Task<CityTemperature> GetAverageTemperatureForCity(string cityName)
    {
        return GetCachedData().FirstOrDefault(x => x.CityName == cityName) ?? throw new KeyNotFoundException($"City '{cityName}' not found in the dataset.");
    }

    public async Task<List<CityTemperature>> FilterCitiesByAverageTemperature(double threshold, FilterType filterType)
    {
        var averages = GetCachedData();
        return filterType switch
        {
            FilterType.Larger => averages.Where(ct => ct.Temperature >= threshold).ToList(),
            FilterType.Smaller => averages.Where(ct => ct.Temperature <= threshold).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(filterType), $"Unhandled filter type: {filterType}")
        };
    }

    private List<CityTemperature> GetCachedData()
    {
        if (!_cache.TryGetValue(CacheKey, out List<CityTemperature> averages))
        {
            return new List<CityTemperature>();
        }
        return averages;
    }
}
