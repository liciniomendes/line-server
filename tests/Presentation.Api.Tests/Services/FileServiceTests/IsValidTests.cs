namespace Presentation.Api.Tests.Services.FileServiceTests;

public class IsValidTests
{
    [Fact]
    public void WhenFilesDoesNotExist_ShouldReturnFalseTest()
    {
        var loggerDummy = A.Dummy<ILogger<FileService>>();
        var fileInfoMock = A.Fake<IFileInfo>(m => m.Strict());
        A.CallTo(() => fileInfoMock.Exists).Returns(false);
        A.CallTo(() => fileInfoMock.FullName).Returns(string.Empty);
        var service = new FileService(fileInfoMock, loggerDummy);

        var actual = service.IsValid();

        actual.Should().BeFalse();
        A.CallTo(() => fileInfoMock.Exists).MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public void WhenFilesExist_ShouldReturnTrueTest()
    {
        var loggerDummy = A.Dummy<ILogger<FileService>>();
        var fileInfoMock = A.Fake<IFileInfo>(m => m.Strict());
        A.CallTo(() => fileInfoMock.Exists).Returns(true);
        A.CallTo(() => fileInfoMock.FullName).Returns(string.Empty);
        var service = new FileService(fileInfoMock, loggerDummy);

        var actual = service.IsValid();

        actual.Should().BeTrue();
        A.CallTo(() => fileInfoMock.Exists).MustHaveHappenedOnceExactly();
    }
}