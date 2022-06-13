namespace Presentation.Api.Models;

/// <summary>
/// Represents a line position in a file
/// </summary>
/// <param name="Start">Byte in the file where the line starts</param>
/// <param name="End">Byte in the file where the line ends</param>
public record Position(long Start, long End);