namespace Presentation.Api.Services.Abstractions;

/// <summary>
/// Contract that represents a service that
/// handles operations with the file
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Validates if the file exist
    /// </summary>
    /// <returns><c>true</c> if exists, <c>false</c> otherwise</returns>
    bool IsValid();

    /// <summary>
    /// Creates a stream to the file read-only.
    /// </summary>
    /// <remarks>
    /// The calling code is responsible for disposing the stream,
    /// otherwise the handle to the file won't be released and will
    /// deplete the system resources faster 
    /// </remarks>
    /// <returns>Read-only stream</returns>
    Stream OpenRead();

    /// <summary>
    /// Get the text that is the the specified position in the file.
    /// </summary>
    /// <param name="position">The position in the file where the text is</param>
    /// <remarks>
    /// We are not validating the position, we assume that the data is correct because
    /// was validated by other parts of the application.
    /// </remarks>
    /// <returns>The ASCII text from that position</returns>
    /// <exception cref="IOException">When the position doesn't exist in the file</exception>
    string GetText(Position position);
}