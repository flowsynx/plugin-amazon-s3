namespace FlowSynx.Plugins.Amazon.S3.UnitTests;

public class PathHelperTests
{
    [Fact]
    public void IsDirectory_ShouldReturnTrue_WhenEndsWithSlash()
    {
        Assert.True(PathHelper.IsDirectory("folder/"));
    }

    [Fact]
    public void IsDirectory_ShouldReturnFalse_WhenNotEndsWithSlash()
    {
        Assert.False(PathHelper.IsDirectory("file.txt"));
    }

    [Fact]
    public void IsFile_ShouldReturnTrue_WhenNotEndsWithSlash()
    {
        Assert.True(PathHelper.IsFile("file.txt"));
    }

    [Fact]
    public void IsFile_ShouldReturnFalse_WhenEndsWithSlash()
    {
        Assert.False(PathHelper.IsFile("folder/"));
    }

    [Fact]
    public void AddTrailingPathSeparator_ShouldAdd_WhenNotPresent()
    {
        Assert.Equal("folder/", PathHelper.AddTrailingPathSeparator("folder"));
    }

    [Fact]
    public void AddTrailingPathSeparator_ShouldNotAdd_WhenAlreadyPresent()
    {
        Assert.Equal("folder/", PathHelper.AddTrailingPathSeparator("folder/"));
    }

    [Fact]
    public void AddPathSeparator_ShouldInsertAtIndex_WhenMissing()
    {
        Assert.Equal("folder/", PathHelper.AddPathSeparator("folder", 6));
    }

    [Fact]
    public void AddPathSeparator_ShouldDoNothing_WhenAlreadyEndsWithSeparator()
    {
        Assert.Equal("folder/", PathHelper.AddPathSeparator("folder/", 7));
    }

    [Fact]
    public void Combine_ShouldJoinParts_Correctly()
    {
        var combined = PathHelper.Combine("folder", "subfolder", "file.txt");
        Assert.Equal("folder/subfolder/file.txt", combined);
    }

    [Fact]
    public void Combine_ShouldHandleNulls_AndEmptyStrings()
    {
        var combined = PathHelper.Combine("folder", null, "", "file.txt");
        Assert.Equal("folder/file.txt", combined);
    }

    [Theory]
    [InlineData("folder/subfolder/file.txt", "folder/subfolder/")]
    [InlineData("file.txt", "/")]
    [InlineData("/", "")]
    [InlineData("", "")]
    public void GetParent_ShouldReturnCorrectParent(string input, string expected)
    {
        Assert.Equal(expected, PathHelper.GetParent(input));
    }

    [Theory]
    [InlineData("folder/../subfolder", "subfolder")]
    [InlineData("a/b/../c", "a/c")]
    [InlineData(null, "/")]
    [InlineData("", "/")]
    public void Normalize_ShouldResolvePathsCorrectly(string input, string expected)
    {
        Assert.Equal(expected, PathHelper.Normalize(input));
    }

    [Fact]
    public void NormalizePart_ShouldTrimSlashes()
    {
        Assert.Equal("folder", PathHelper.NormalizePart("/folder/"));
    }

    [Fact]
    public void NormalizePart_ShouldThrow_WhenNull()
    {
        Assert.Throws<ArgumentNullException>(() => PathHelper.NormalizePart(null));
    }

    [Fact]
    public void Split_ShouldReturnParts()
    {
        var result = PathHelper.Split("/a/b/c/");
        Assert.Equal(new[] { "a", "b", "c" }, result);
    }

    [Fact]
    public void Split_ShouldReturnEmptyArray_WhenNullOrEmpty()
    {
        Assert.Empty(PathHelper.Split(null));
        Assert.Empty(PathHelper.Split(""));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("/", true)]
    [InlineData("folder", false)]
    public void IsRootPath_ShouldIdentifyRootCorrectly(string input, bool expected)
    {
        Assert.Equal(expected, PathHelper.IsRootPath(input));
    }

    [Theory]
    [InlineData("folder/../file.txt", "file.txt", true)]
    [InlineData("folder/file.txt", "folder/file.txt", true)]
    [InlineData("folder/file.txt", "folder2/file.txt", false)]
    public void ComparePath_ShouldReturnTrueForEquivalentPaths(string p1, string p2, bool expected)
    {
        Assert.Equal(expected, PathHelper.ComparePath(p1, p2));
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(@"folder\subfolder\file.txt", "folder/subfolder/file.txt")]
    public void ToUnixPath_ShouldConvertBackslashesToForwardSlashes(string input, string expected)
    {
        Assert.Equal(expected, PathHelper.ToUnixPath(input));
    }
}