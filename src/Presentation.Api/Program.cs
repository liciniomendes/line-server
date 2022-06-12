using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var index = Memory<long>.Empty;

app.Logger.LogInformation("Application starting");

app.MapGet("/", () => args[0]);
app.MapGet("/lines/{lineId:int}", (int lineId) =>
{
    if (lineId < 1)
        return Results.NotFound();

    if (lineId > index.Length)
        return Results.StatusCode(413);

    var span = index.Span;
    var start = lineId == 1 ? 0 : span[lineId - 2] + 1;
    var end = span[lineId - 1];

    using var stream = new FileInfo(args[0]).OpenRead();
    var p = stream.Seek(start, SeekOrigin.Begin);
    var buffer = new byte[end - start];
    var read = stream.Read(buffer);

    return Results.Ok(Encoding.ASCII.GetString(buffer));
});

try
{
    app.Logger.LogDebug("Supplied arguments are [{arguments}]", args);
    
    if (args.Length < 1)
    {
        app.Logger.LogCritical("No arguments were supplied");
        
        return 1;
    }

    var file = new FileInfo(args[0]);
    app.Logger.LogDebug("Validating file [{fullPath}]", file.FullName);
    if (file.Exists is false)
    {
        app.Logger.LogCritical("File {filename} can't be found", file.FullName);

        return 3;
    }

    var processor = new CacheFile(file, app.Services.GetService<ILogger<CacheFile>>());
    index = processor.Process();
    
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application ended abnormally: {message}", ex.Message);

    return 2;
}

return 0;

internal class CacheFile
{
    private readonly FileInfo _file;
    private readonly ILogger<CacheFile> _logger;

    public CacheFile(FileInfo file, ILogger<CacheFile> logger)
    {
        _file = file;
        _logger = logger;
    }

    internal Memory<long> Process()
    {
        var startMemory = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Start building the index");
        using var stream = _file.OpenRead();
        _logger.LogDebug("{l}", stream.Length);
        _logger.LogDebug("Stream size is [{size}]", ConvertToReadableSize(stream.Length));
        
        var cachedIndex = new LinkedList<long>();
        for (var i = 0L; i < stream.Length; i++)
        {
            var value = stream.ReadByte();
            if (value is not '\n') continue;
            //_logger.LogDebug("Found end of line at {i}", i);
            
            cachedIndex.AddLast(i);
        }
        
        stopwatch.Stop();
        _logger.LogDebug(
            "File contains [{lines}] lines, took {elapsed} and is using {MemorySize}",
            cachedIndex.Count,
            stopwatch.Elapsed,
            ConvertToReadableSize(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64));

        return new Memory<long>(cachedIndex.ToArray());
    }

    private string ConvertToReadableSize(long size) => size switch
    {
        < 1000 => $"{size} bytes",
        >= 1000 and < 1000000 => $"{ size / 1000 } KB",
        >= 1000000 and < 1000000000 => $"{size / 1000 / 1000} MB",
        _ => $"{ size / 1000 / 1000 / 1000 } GB"
    };

}
