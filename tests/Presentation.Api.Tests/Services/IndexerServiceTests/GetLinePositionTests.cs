namespace Presentation.Api.Tests.Services.IndexerServiceTests;

public class GetLinePositionTests
{
    [Fact]
    public void WhenTheFileIsEmpty_ShouldThrowIndexOutOfRangeExceptionTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream());
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual =  () =>service.GetLinePosition(0);

        actual.Should().Throw<IndexOutOfRangeException>();
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenFileDoesNotHaveALineFeed_ShouldThrowIndexOutOfRangeExceptionTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("May the force be with you!");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual =  () =>service.GetLinePosition(0);

        actual.Should().Throw<IndexOutOfRangeException>();
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenThereIsOnlyOneLine_ShouldReturnCorrectlyTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("May the force be with you!\n");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.GetLinePosition(0);

        actual.Start.Should().Be(0);
        actual.End.Should().Be(26);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenGettingFirstLine_ShouldReturnZeroForStartAndNextLineMinusOneForEndTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("May the force be with you!\n\n");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.GetLinePosition(0);

        actual.Start.Should().Be(0);
        actual.End.Should().Be(26);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenGettingLastLine_ShouldReturnAFilledPositionTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("\nMay the force be with you!\n");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.GetLinePosition(1);

        actual.Start.Should().Be(1);
        actual.End.Should().Be(27);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenLineIsInTheMiddleOfOthers_ShouldReturnAFilledPositionTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("\nMay the force be with you!\n\n");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.GetLinePosition(1);

        actual.Start.Should().Be(1);
        actual.End.Should().Be(27);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenLineDoesNotExist_ShouldThrowTest()
    {
        var loggerDummy = A.Dummy<ILogger<LineFeedIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("\nMay the force be with you!");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new LineFeedIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual =  () =>service.GetLinePosition(1);

        actual.Should().Throw<IndexOutOfRangeException>();
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
}