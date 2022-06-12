namespace Presentation.Api.Services.Abstractions;

public interface IFileService
{
    bool IsValid();

    Stream OpenRead();

    string GetText(Position position);
}