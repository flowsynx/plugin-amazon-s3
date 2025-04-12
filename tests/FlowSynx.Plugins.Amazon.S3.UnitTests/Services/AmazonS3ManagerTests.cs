using Amazon.S3.Model;
using Amazon.S3;
using FlowSynx.PluginCore;
using FlowSynx.Plugins.Amazon.S3.Services;
using Moq;
using Amazon;

namespace FlowSynx.Plugins.Amazon.S3.UnitTests.Services;

public class AmazonS3ManagerTests
{
    private readonly Mock<IPluginLogger> _loggerMock;
    private readonly Mock<AmazonS3Client> _clientMock;
    private readonly AmazonS3Manager _s3Manager;
    private const string BucketName = "test-bucket";

    public AmazonS3ManagerTests()
    {
        _loggerMock = new Mock<IPluginLogger>();
        _clientMock = new Mock<AmazonS3Client>("accessKey", "secretKey", RegionEndpoint.USEast1);
        _s3Manager = new AmazonS3Manager(_loggerMock.Object, _clientMock.Object, BucketName);
    }

    [Fact]
    public async Task Create_WithValidParameters_CreatesBucketAndFolder()
    {
        // Arrange
        var parameters = new PluginParameters
        {
            { "path", "test/path/" }
        };

        _clientMock
            .Setup(c => c.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListBucketsResponse
            {
                Buckets = new List<S3Bucket>()
            });

        _clientMock
            .Setup(c => c.PutBucketAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutBucketResponse());

        _clientMock
            .Setup(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse());

        // Act
        await _s3Manager.Create(parameters, CancellationToken.None);

        // Assert
        _clientMock.Verify(c => c.PutBucketAsync(BucketName, It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(c => c.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Exist_WithExistingPath_ReturnsTrue()
    {
        // Arrange
        var parameters = new PluginParameters
        {
            { "path", "test/path/file.txt" }
        };

        _clientMock
            .Setup(c => c.ListObjectsAsync(It.IsAny<ListObjectsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListObjectsResponse
            {
                S3Objects = new List<S3Object> { new S3Object { Key = "test/path/file.txt" } }
            });

        // Act
        var result = await _s3Manager.Exist(parameters, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Delete_WhenPathDoesNotExist_ThrowsException()
    {
        // Arrange
        var parameters = new PluginParameters
        {
            { "path", "test/path/file.txt" }
        };

        _clientMock
            .Setup(c => c.ListObjectsAsync(It.IsAny<ListObjectsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListObjectsResponse());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _s3Manager.Delete(parameters, CancellationToken.None));
    }
}
