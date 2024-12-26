namespace shortUrlAOT;

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Dictionary<string, List<UrlMap>>))]
internal partial class DictionaryJsonContext : JsonSerializerContext
{
}


public class DbBackgroundService : IHostedService, IDisposable
{
    private readonly ILogger<DbBackgroundService> _logger;
    private readonly Db _db;
    private Timer _timer;
    private const string FilePath = "users.json";

    public DbBackgroundService(ILogger<DbBackgroundService> logger, Db db)
    {
        _logger = logger;
        _db = db;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Users Background Service is starting.");
        LoadUsersFromFile();
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        SaveUsersToFile();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Users Background Service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    private void SaveUsersToFile()
    {
        var json = JsonSerializer.Serialize(_db.Users,DictionaryJsonContext.Default.DictionaryStringListUrlMap);
        File.WriteAllText(FilePath, json);
        _logger.LogInformation("Users data has been saved to file.");
    }

    private void LoadUsersFromFile()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            _db.Users = JsonSerializer.Deserialize<Dictionary<string, List<UrlMap>>>(json,DictionaryJsonContext.Default.DictionaryStringListUrlMap) ?? new Dictionary<string, List<UrlMap>>();
            _logger.LogInformation("Users data has been loaded from file.");
        }
    }
}
