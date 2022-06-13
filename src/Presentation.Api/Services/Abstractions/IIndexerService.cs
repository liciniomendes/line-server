namespace Presentation.Api.Services.Abstractions;

/// <summary>
/// Contract that represents a service that
/// indexes the content of a file by lines
/// </summary>
public interface IIndexerService
{
    /// <summary>
    /// Gets the position that represents the line number
    /// </summary>
    /// <param name="lineNumber">Number of the line that should be searched</param>
    /// <returns>The position of the line in the file</returns>
    /// <exception cref="IndexOutOfRangeException">When the line doesn't exist in the index</exception>
    Position GetLinePosition(int lineNumber);
    
    /// <summary>
    /// Quantity of lines indexed, <c>0</c> means empty.
    /// </summary>
    int Size { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileService"></param>
    /// <returns></returns>
    bool Build(IFileService fileService);
}