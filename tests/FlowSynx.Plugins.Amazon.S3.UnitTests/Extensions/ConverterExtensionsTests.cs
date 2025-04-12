using Amazon.S3.Model;
using Amazon.S3;
using Moq;
using System.Text;
using Amazon;
using FlowSynx.Plugins.Amazon.S3.Extensions;

namespace FlowSynx.Plugins.Amazon.S3.UnitTests.Extensions;

public class ConverterExtensionsTests
{
    [Fact]
    public async Task ToContext_TextFile_ReturnsContent()
    {
        // Arrange
        var bucketName = "test-bucket";
        var key = "text-file.txt";
        var textContent = "Hello, this is a test!";
        var contentBytes = Encoding.UTF8.GetBytes(textContent);

        var mockClient = new Mock<AmazonS3Client>("accessKey", "secretKey", RegionEndpoint.USEast1);
        var mockResponseStream = new MemoryStream(contentBytes);

        var mockGetObjectResponse = new GetObjectResponse
        {
            BucketName = bucketName,
            Key = key,
            ResponseStream = mockResponseStream,
            ContentLength = contentBytes.Length,
            LastModified = DateTime.UtcNow,
            ETag = "\"etagvalue\"",
            StorageClass = S3StorageClass.Standard
        };

        mockClient.Setup(c => c.GetObjectAsync(
                It.Is<GetObjectRequest>(r => r.BucketName == bucketName && r.Key == key),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockGetObjectResponse);

        mockClient.Setup(c => c.GetObjectMetadataAsync(
                bucketName,
                key,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new GetObjectMetadataResponse
            {
                Headers = { ContentType = "text/plain" },
            });

        // Act
        var result = await mockClient.Object.ToContext(bucketName, key, includeMetadata: true, CancellationToken.None);

        // Assert
        Assert.Equal(key, result.Id);
        Assert.Equal("File", result.SourceType);
        Assert.Null(result.RawData);
        Assert.Equal(textContent, result.Content);
        Assert.Equal("text/plain", result.Format);
    }

    [Fact]
    public async Task ToContext_BinaryFile_ReturnsRawData()
    {
        // Arrange
        var bucketName = "test-bucket";
        var key = "binary.dat";
        var binaryData = new byte[] { 0x00, 0xFF, 0x10, 0x20, 0x30, 0x40 };

        var mockClient = new Mock<AmazonS3Client>("accessKey", "secretKey", RegionEndpoint.USEast1);
        var mockResponseStream = new MemoryStream(binaryData);

        var mockGetObjectResponse = new GetObjectResponse
        {
            BucketName = bucketName,
            Key = key,
            ResponseStream = mockResponseStream,
            ContentLength = binaryData.Length,
            LastModified = DateTime.UtcNow,
            ETag = "\"etagvalue\"",
            StorageClass = S3StorageClass.Standard
        };

        mockClient.Setup(c => c.GetObjectAsync(
                It.Is<GetObjectRequest>(r => r.BucketName == bucketName && r.Key == key),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockGetObjectResponse);

        mockClient.Setup(c => c.GetObjectMetadataAsync(
                bucketName,
                key,
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(new GetObjectMetadataResponse
            {
                Headers = { ContentType = "application/octet-stream" }
            });

        // Act
        var result = await mockClient.Object.ToContext(bucketName, key, includeMetadata: true, CancellationToken.None);

        // Assert
        Assert.Equal(key, result.Id);
        Assert.Equal("File", result.SourceType);
        Assert.NotNull(result.RawData);
        Assert.Null(result.Content);
        Assert.Equal("application/octet-stream", result.Format);
    }

    [Fact]
    public async Task ToContext_WithoutMetadata_DoesNotAddMetadata()
    {
        // Arrange
        var bucketName = "test-bucket";
        var key = "file.txt";
        var contentBytes = Encoding.UTF8.GetBytes("Some content");

        var mockClient = new Mock<AmazonS3Client>("accessKey", "secretKey", RegionEndpoint.USEast1);
        var mockResponseStream = new MemoryStream(contentBytes);

        var mockGetObjectResponse = new GetObjectResponse
        {
            BucketName = bucketName,
            Key = key,
            ResponseStream = mockResponseStream,
            ContentLength = contentBytes.Length,
            LastModified = DateTime.UtcNow,
            ETag = "\"etagvalue\"",
            StorageClass = S3StorageClass.Standard
        };

        mockClient.Setup(c => c.GetObjectAsync(
                It.Is<GetObjectRequest>(r => r.BucketName == bucketName && r.Key == key),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(mockGetObjectResponse);

        // Act
        var result = await mockClient.Object.ToContext(bucketName, key, includeMetadata: false, CancellationToken.None);

        // Assert
        Assert.Equal(key, result.Id);
        Assert.Equal("File", result.SourceType);
        Assert.Null(result.RawData);
        Assert.NotNull(result.Content);
        Assert.Empty(result.Metadata); // Metadata should be empty
    }
}