using FluentAssertions;
using Hackathon.Application.DTOs.Requests;

namespace Hackathon.Application.Tests.DTOs.Requests;

public class PagedRequestTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var request = new PagedRequest();

        // Assert
        request.PageNumber.Should().Be(1);
        request.PageSize.Should().Be(10);
    }

    [Fact]
    public void PageNumber_Setter_ShouldSetValue()
    {
        // Arrange
        var request = new PagedRequest();
        const int expectedPageNumber = 5;

        // Act
        request.PageNumber = expectedPageNumber;

        // Assert
        request.PageNumber.Should().Be(expectedPageNumber);
    }

    [Fact]
    public void PageSize_Setter_ShouldSetValue()
    {
        // Arrange
        var request = new PagedRequest();
        const int expectedPageSize = 20;

        // Act
        request.PageSize = expectedPageSize;

        // Assert
        request.PageSize.Should().Be(expectedPageSize);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(5, 5)]
    public void GetValidPageNumber_ShouldReturnValidPageNumber(int input, int expected)
    {
        // Arrange
        var request = new PagedRequest { PageNumber = input };

        // Act
        var result = request.GetValidPageNumber();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(5, 5)]
    [InlineData(100, 100)]
    public void GetValidPageSize_ShouldReturnValidPageSize(int input, int expected)
    {
        // Arrange
        var request = new PagedRequest { PageSize = input };

        // Act
        var result = request.GetValidPageSize();

        // Assert
        result.Should().Be(expected);
    }
}
