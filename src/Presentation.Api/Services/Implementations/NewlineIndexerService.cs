namespace Presentation.Api.Services.Implementations;

/// <summary>
/// Implements a indexer service that indexes a file by lines. 
/// A line will be created for each newline <c>\n</c>
/// </summary>
public class NewlineIndexerService : IIndexerService
{
    private readonly ILogger<NewlineIndexerService> _logger;
    private Memory<int> _index = Memory<int>.Empty;

    public NewlineIndexerService(ILogger<NewlineIndexerService> logger) => _logger = logger;

    /// <inheritdoc />
    public Position GetLinePosition(int lineNumber)
    {
        var index = _index.Span;
        
        var start = lineNumber == 0 ? 0 : index[lineNumber - 1] + 1;
        var end = index[lineNumber];
        
        return new Position(start, end);
    }

    /// <inheritdoc />
    public int Size => _index.Length;
    
    /// <inheritdoc />
    public bool Build(IFileService fileService)
    {
        var numberOfLines = 0L;
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Start building the index");
        
        using var stream = fileService.OpenRead();
        _logger.LogDebug("Stream size is [{size}]", ConvertToReadableSize(stream.Length));
        
        var index = new LinkedList<int>();
        var reader = new StreamReader(stream);
        var buffer = new char[4096].AsSpan();
        var streamProgress = 0;
        while (reader.EndOfStream is false)
        {
            var read = reader.Read(buffer);
            for (var i = 0; i < read; i++)
            {
                if (buffer[i] is not '\n')
                {
                    continue;
                }

                index.AddLast(streamProgress + i);
                numberOfLines += 1;
            }

            streamProgress += buffer.Length;
            
            // an array max size is int.MaxValue, we are accepting that 
            // this solution only works while the number of lines is lower
            // than that
            if (numberOfLines <= int.MaxValue)
            {
                continue;
            }
            
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
            ConvertToReadableSize(Process.GetCurrentProcess().WorkingSet64));

        _index = new Memory<int>(index.ToArray());

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