var builder = WebApplication.CreateBuilder(args);

// registers the services in the DI
builder.Services
    .AddSingleton<IIndexerService, LineFeedIndexerService>()
    .AddSingleton<IFileService, FileService>()
    .AddSingleton<IFileSystem, FileSystem>()
    .AddSingleton<IFileInfo>(provider =>
    {
        // This will only be executed after the arguments are validated
        var fileSystem = provider.GetRequiredService<IFileSystem>();
        return fileSystem.FileInfo.FromFileName(args[0]);
    });
    
var app = builder.Build();

// Defines the endpoints
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
    
    // validates arguments
    if (args.Length < 1)
    {
        app.Logger.LogCritical("No arguments were supplied");
        
        return 1;
    }

    // validates the file
    var fileService = app.Services.GetRequiredService<IFileService>();
    if (fileService.IsValid() is false)
    {
        app.Logger.LogCritical("File is not valid");

        return 3;
    }   

    // builds the file index
    var indexer = app.Services.GetRequiredService<IIndexerService>();
    var result = indexer.Build(fileService);
    if (result is false)
    {
        app.Logger.LogCritical("Failed to build the index. Terminating.");

        return 4;
    }
    
    // starts the endpoint listening
    await app.RunAsync();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Application ended abnormally: {message}", ex.Message);

    return 2;
}

return 0;