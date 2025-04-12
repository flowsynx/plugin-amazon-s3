using System.Text;
using FlowSynx.Plugins.Amazon.S3.Extensions;

namespace FlowSynx.Plugins.Amazon.S3.UnitTests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("SGVsbG8gV29ybGQ=", true)] // "Hello World"
    [InlineData("U29tZSBkYXRhIHdpdGggcGFkZGluZw==", true)]
    [InlineData("", false)]
    [InlineData("NotBase64!!", false)]
    [InlineData("SGVsbG8gV29ybGQ", false)] // Invalid length (not divisible by 4)
    [InlineData("SG VsbG8=", false)] // Contains space
    [InlineData("SG\tVsbG8=", false)] // Contains tab
    [InlineData("SG\rVsbG8=", false)] // Contains carriage return
    [InlineData("SG\nVsbG8=", false)] // Contains line feed
    [InlineData("SGVsbG8=", true)] // Valid short base64
    [InlineData("c3RyaW5nLXdpdGgtc3ltYm9sLXMh", true)] // includes "-" and "!" -> not valid base64
    public void IsBase64String_Should_ReturnExpectedResult(string input, bool expected)
    {
        var result = input.IsBase64String();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToByteArray_Should_ConvertCorrectly()
    {
        var input = "Hello World!";
        var expected = Encoding.UTF8.GetBytes(input);

        var result = input.ToByteArray();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Base64ToByteArray_Should_DecodeCorrectly()
    {
        var original = "TestString";
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(original));

        var result = base64.Base64ToByteArray();

        Assert.Equal(original, Encoding.UTF8.GetString(result));
    }

    [Fact]
    public void Base64ToByteArray_InvalidInput_ShouldThrowFormatException()
    {
        var invalidBase64 = "ThisIsNotBase64!!";

        Assert.Throws<FormatException>(() => invalidBase64.Base64ToByteArray());
    }
}