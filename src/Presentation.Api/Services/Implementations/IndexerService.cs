namespace Presentation.Api.Services.Implementations;

public class IndexerService : IIndexerService
{
    private readonly ILogger<IndexerService> _logger;
    private Memory<long> _index = Memory<long>.Empty;

    public IndexerService(ILogger<IndexerService> logger) => _logger = logger;

    public Position GetLinePosition(int lineNumber)
    {
        var index = _index.Span;
        
        var start = lineNumber == 0 ? 0 : index[lineNumber - 1] + 1;
        var end = index[lineNumber];
        
        return new Position(start, end);
    }

    public int Size => _index.Length;
    
    public bool Build(IFileService fileService)
    {
        var numberOfLines = 0L;
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Start building the index");
        
        using var stream = fileService.OpenRead();
        _logger.LogDebug("Stream size is [{size}]", ConvertToReadableSize(stream.Length));
        
        var index = new LinkedList<long>();
        for (var i = 0L; i < stream.Length; i++)
        {
            var value = stream.ReadByte();
            if (value is not '\n') continue;
            
            index.AddLast(i);
            numberOfLines += 1;

            if (numberOfLines <= int.MaxValue) continue;
            
            _logger.LogCritical(
                "File contains at least [{totalLines}] lines, max supported is [{maxLines}]",
                numberOfLines, 
                int.MaxValue);
                
            return false;
        }
        
        stopwatch.Stop();
        _logger.LogDebug(
            "File contains [{lines}] lines, took {elapsed} and is using {MemorySize}",
            index.Count,
            stopwatch.Elapsed,
            ConvertToReadableSize(System.Diagnostics.Process.GetCurrentProcess().WorkingSet64));

        _index = new Memory<long>(index.ToArray());

        return true;
    }

    private string ConvertToReadableSize(long size) => size switch
    {
        < 1000 => $"{size} bytes",
        >= 1000 and < 1000000 => $"{ size / 1000 } KB",
        >= 1000000 and < 1000000000 => $"{size / 1000 / 1000} MB",
        _ => $"{ size / 1000 / 1000 / 1000 } GB"
    };
}