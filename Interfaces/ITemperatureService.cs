using System;
using IndigoLabsAssignment.Enums;
using Models.IndigoLabsAssignment;

namespace IndigoLabsAssignment.Interfaces;

public interface ITemperatureService
{
    Task<List<CityTemperature>> CalculateCityAverages();
    Task<List<CityTemperature>> GetCityAverages();
    Task<CityTemperature> GetAverageTemperatureForCity(string cityName);
    Task<List<CityTemperature>> FilterCitiesByAverageTemperature(double threshold, FilterType filterType);
}
