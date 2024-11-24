using System;
using IndigoLabsAssignment.Interfaces;
using IndigoLabsAssignment.Models;

namespace IndigoLabsAssignment.Services;

public class FileMonitorService : BackgroundService
{
    private readonly FileSystemWatcher _fileWatcher;
    private readonly ITemperatureService _temperatureService;
    private readonly Settings _settings;

    public FileMonitorService(ITemperatureService temperatureService, Settings settings)
    {
        _temperatureService = temperatureService;
        _settings = settings;

        _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_settings.DataPath))
        {
            Filter = Path.GetFileName(_settings.DataPath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        _fileWatcher.Changed += OnFileChanged;
        _fileWatcher.Created += OnFileChanged;
        _fileWatcher.Renamed += OnFileChanged;
        _fileWatcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            _temperatureService.CalculateCityAverages();
            Console.WriteLine("File changed. City averages recalculated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error recalculating averages: {ex.Message}");
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _fileWatcher.EnableRaisingEvents = true;
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _fileWatcher.Dispose();
        base.Dispose();
    }
}
