using Presentation.Api.Models;

namespace Presentation.Api.Tests.Services.FileServiceTests;

public class GetTextTests
{
    [Fact]
    public void WhenRequestingTheWholeFile_ShouldReturnAllTextTest()
    {
        const string expectedText = "May the force be with you!";
        var loggerDummy = A.Dummy<ILogger<FileService>>();
        var fileContent = Encoding.ASCII.GetBytes(expectedText);
        var fileInfoMock = A.Fake<IFileInfo>(m => m.Strict());
        A.CallTo(() => fileInfoMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new FileService(fileInfoMock, loggerDummy);

        var actual = service.GetText(new Position(0, 26));

        actual.Should().BeEquivalentTo(expectedText);
        A.CallTo(() => fileInfoMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenRequestingASpecificPosition_ShouldReturnOnlyThatTextTest()
    {
        const string expectedText = "the force";
        const string text = $"May {expectedText} be with you!";
        
        var loggerDummy = A.Dummy<ILogger<FileService>>();
        var fileContent = Encoding.ASCII.GetBytes(text);
        var fileInfoMock = A.Fake<IFileInfo>(m => m.Strict());
        A.CallTo(() => fileInfoMock.OpenRead()).Returns(new MemoryStream(fileContent));
        var service = new FileService(fileInfoMock, loggerDummy);

        var actual = service.GetText(new Position(4, 13));

        actual.Should().BeEquivalentTo(expectedText);
        A.CallTo(() => fileInfoMock.OpenRead()).MustHaveHappenedOnceExactly();
    }
}