namespace Presentation.Api.Tests.Services.IndexerServiceTests;

public class SizeTests
{
    [Fact]
    public void WhenBuildIsNotCalled_ShouldReturnZeroTest()
    {
        var loggerDummy = A.Dummy<ILogger<NewlineIndexerService>>();
        var service = new NewlineIndexerService(loggerDummy);

        var actual = service.Size;

        actual.Should().Be(0);
    }

    [Fact]
    public void WhenFileIsEmpty_ShouldReturnZeroTest()
    {
        var loggerDummy = A.Dummy<ILogger<NewlineIndexerService>>();
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream());
        var service = new NewlineIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.Size;
        
        actual.Should().Be(0);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenFileDoesNotContainNewline_ShouldReturnZeroTest()
    {
        var loggerDummy = A.Dummy<ILogger<NewlineIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("May the force be with you!");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new NewlineIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.Size;
        
        actual.Should().Be(0);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenFileDoesContainNewline_ShouldReturnOneTest()
    {
        var loggerDummy = A.Dummy<ILogger<NewlineIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("May the force be with you!\n");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new NewlineIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.Size;
        
        actual.Should().Be(1);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenFileDoesContainConsecutiveNewlines_ShouldCountAsALineTest()
    {
        var loggerDummy = A.Dummy<ILogger<NewlineIndexerService>>();
        var fileContent = Encoding.ASCII.GetBytes("\n\n\n");
        var fileServiceMock = A.Fake<IFileService>(m => m.Strict());
        A.CallTo(() => fileServiceMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new NewlineIndexerService(loggerDummy);
        service.Build(fileServiceMock);

        var actual = service.Size;
        
        actual.Should().Be(3);
        A.CallTo(() => fileServiceMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
}