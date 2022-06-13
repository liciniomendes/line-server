namespace Presentation.Api.Tests.Integrations.FileServiceTests;

public class GetTextTests
{
    [Fact]
    public void GetFirstLine_ShouldReturnWithoutTheLineFeedTest()
    {
        const string expectedText = "May the force be with you!";
        var fileServiceLoggerDummy = A.Dummy<ILogger<FileService>>();
        var indexerServiceLoggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes($"{expectedText}\n");
        var fileInfoMock = A.Fake<IFileInfo>();
        A.CallTo(() => fileInfoMock.OpenRead())
            .ReturnsNextFromSequence(new MemoryStream(fileContent), new MemoryStream(fileContent));
        var indexerService = new LineFeedIndexerService(indexerServiceLoggerDummy);
        var fileService = new FileService(fileInfoMock, fileServiceLoggerDummy);
        indexerService.Build(fileService);
        var position = indexerService.GetLinePosition(0);

        var actual = fileService.GetText(position);

        actual.Should().BeEquivalentTo(expectedText);
    }
    
    [Fact]
    public void GetOneLineFromTheMiddle_ShouldReturnWithoutTheLineFeedTest()
    {
        const string expectedText = "May the force be with you!";
        var fileServiceLoggerDummy = A.Dummy<ILogger<FileService>>();
        var indexerServiceLoggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes($"Trash\n{expectedText}\nTrash\n");
        var fileInfoMock = A.Fake<IFileInfo>();
        A.CallTo(() => fileInfoMock.OpenRead())
            .ReturnsNextFromSequence(new MemoryStream(fileContent), new MemoryStream(fileContent));
        var indexerService = new LineFeedIndexerService(indexerServiceLoggerDummy);
        var fileService = new FileService(fileInfoMock, fileServiceLoggerDummy);
        indexerService.Build(fileService);
        var position = indexerService.GetLinePosition(1);

        var actual = fileService.GetText(position);

        actual.Should().BeEquivalentTo(expectedText);
    }
    
    [Fact]
    public void GetLastLine_ShouldReturnWithoutTheLineFeedTest()
    {
        const string expectedText = "May the force be with you!";
        var fileServiceLoggerDummy = A.Dummy<ILogger<FileService>>();
        var indexerServiceLoggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes($"Trash\n{expectedText}\n");
        var fileInfoMock = A.Fake<IFileInfo>();
        A.CallTo(() => fileInfoMock.OpenRead())
            .ReturnsNextFromSequence(new MemoryStream(fileContent), new MemoryStream(fileContent));
        var indexerService = new LineFeedIndexerService(indexerServiceLoggerDummy);
        var fileService = new FileService(fileInfoMock, fileServiceLoggerDummy);
        indexerService.Build(fileService);
        var position = indexerService.GetLinePosition(1);

        var actual = fileService.GetText(position);

        actual.Should().BeEquivalentTo(expectedText);
    }
}