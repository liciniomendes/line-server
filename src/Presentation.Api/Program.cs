var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IIndexerService, IndexerService>()
    .AddSingleton<IFileService, FileService>()
    .AddSingleton<IFileSystem, FileSystem>()
    .AddSingleton<IFileInfo>(provider =>
    {
        var fileSystem = provider.GetRequiredService<IFileSystem>();
        return fileSystem.FileInfo.FromFileName(args[0]);
    });
    
var app = builder.Build();

app.MapGet("/", () => args[0]);
app.MapGet("/lines/{lineNumber:int}", (int lineNumber, IIndexerService indexer, IFileService fileService) =>
{
    if (lineNumber < 0 || lineNumber > indexer.Size)
    {
        return Results.StatusCode(413);
    }

    var linePosition = indexer.GetLinePosition(lineNumber);
    return Results.Ok(fileService.GetText(linePosition));
});

try
{
    app.Logger.LogInformation("Application starting");
    app.Logger.LogDebug("Supplied arguments are [{arguments}]", args);
    
    if (args.Length < 1)
    {
        app.Logger.LogCritical("No arguments were supplied");
        
        return 1;
    }

    var fileService = app.Services.GetRequiredService<IFileService>();
    if (fileService.IsValid() is false)
    {
        app.Logger.LogCritical("File is not valid");

        return 3;
    }   

    var indexer = app.Services.GetRequiredService<IIndexerService>();
    indexer.Build(fileService);
    
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application ended abnormally: {message}", ex.Message);

    return 2;
}

return 0;