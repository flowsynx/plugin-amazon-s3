using Amazon;
using Amazon.S3;
using FlowSynx.Plugins.Amazon.S3.Models;
using FlowSynx.Plugins.Amazon.S3.Services;
using System.Reflection;

namespace FlowSynx.Plugins.Amazon.S3.UnitTests.Services;

public class AmazonS3ConnectionTests
{
    private readonly AmazonS3Connection _connection;

    public AmazonS3ConnectionTests()
    {
        _connection = new AmazonS3Connection();
    }

    [Fact]
    public void Connect_WithNullAccessKey_ThrowsArgumentNullException()
    {
        // Arrange
        var specs = new AmazonS3Specifications
        {
            AccessKey = string.Empty,
            SecretKey = "secret"
        };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => _connection.Connect(specs));
        Assert.Equal("AccessKey", ex.ParamName);
    }

    [Fact]
    public void Connect_WithNullSecretKey_ThrowsArgumentNullException()
    {
        // Arrange
        var specs = new AmazonS3Specifications
        {
            AccessKey = "access",
            SecretKey = string.Empty
        };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => _connection.Connect(specs));
        Assert.Equal("SecretKey", ex.ParamName);
    }

    [Fact]
    public void Connect_WithAccessAndSecret_CreatesClient()
    {
        // Arrange
        var specs = new AmazonS3Specifications
        {
            AccessKey = "fake-access",
            SecretKey = "fake-secret",
            Region = "us-east-1"
        };

        // Act
        var client = _connection.Connect(specs);

        // Assert
        Assert.NotNull(client);
        Assert.IsType<AmazonS3Client>(client);
    }

    [Fact]
    public void Connect_WithSessionToken_UsesSessionAWSCredentials()
    {
        // Arrange
        var specs = new AmazonS3Specifications
        {
            AccessKey = "key",
            SecretKey = "secret",
            SessionToken = "session",
            Region = "us-east-1"
        };

        // Act
        var client = _connection.Connect(specs);

        // Assert
        Assert.NotNull(client);
        Assert.IsType<AmazonS3Client>(client);
    }

    [Fact]
    public void GetConfig_WithNullRegion_ReturnsDefaultConfig()
    {
        // Use reflection to call private method
        var method = typeof(AmazonS3Connection).GetMethod("GetConfig", 
            BindingFlags.NonPublic | 
            BindingFlags.Instance);
        var result = method!.Invoke(_connection, new object?[] { null });

        Assert.NotNull(result);
        Assert.IsType<AmazonS3Config>(result);
    }

    [Fact]
    public void ToRegionEndpoint_WithNull_ThrowsArgumentNullException()
    {
        var method = typeof(AmazonS3Connection).GetMethod("ToRegionEndpoint", 
            BindingFlags.NonPublic | 
            BindingFlags.Instance);

        var ex = Assert.Throws<TargetInvocationException>(() => method!.Invoke(_connection, new object?[] { null }));
        Assert.IsType<ArgumentNullException>(ex.InnerException);
        Assert.Equal("region", ((ArgumentNullException)ex.InnerException!).ParamName);
    }

    [Fact]
    public void ToRegionEndpoint_WithValidRegion_ReturnsRegionEndpoint()
    {
        var method = typeof(AmazonS3Connection).GetMethod("ToRegionEndpoint", 
            BindingFlags.NonPublic | 
            BindingFlags.Instance);

        var result = method!.Invoke(_connection, new object[] { "us-east-1" });

        Assert.NotNull(result);
        Assert.IsType<RegionEndpoint>(result);
        Assert.Equal("us-east-1", ((RegionEndpoint)result).SystemName);
    }
}