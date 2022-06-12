namespace Presentation.Api.Services.Abstractions;

public interface IIndexerService
{
    Position GetLinePosition(int lineNumber);
    
    int Size { get; }
    
    bool Build(IFileService fileService);
}