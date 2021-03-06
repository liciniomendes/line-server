namespace Presentation.Api.Services.Implementations;

/// <summary>
/// Implements a service that controls access to a file
/// using a <see cref="FileInfo"/> component
/// </summary>
public class FileService : IFileService
{
    private readonly IFileInfo _file;
    private readonly ILogger<FileService> _logger;

    public FileService(IFileInfo file, ILogger<FileService> logger)
    {
        _file = file;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsValid()
    {
        _logger.LogDebug("Validating file [{fullPath}]", _file.FullName);
        if (_file.Exists)
        {
            return true;
        }
        
        _logger.LogError("File {filename} can't be found", _file.FullName);
        
        return false;
    }

    /// <inheritdoc />
    public Stream OpenRead() => _file.OpenRead();
    
    /// <inheritdoc />
    public string GetText(Position position)
    {
        using var stream = _file.OpenRead();
        
        _ = stream.Seek(position.Start, SeekOrigin.Begin);
        
        var buffer = new byte[position.End - position.Start];
        _ = stream.Read(buffer);

        return Encoding.ASCII.GetString(buffer);
    }
}