using FluentAssertions;
using Hackathon.Application.DTOs.Responses;

namespace Hackathon.Application.Tests.DTOs.Responses;

public class PagedResponseTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var response = new PagedResponse<string>();

        // Assert
        response.Data.Should().BeEmpty();
        response.PageNumber.Should().Be(0);
        response.PageSize.Should().Be(0);
        response.TotalRecords.Should().Be(0);
        response.TotalPages.Should().Be(0);
        response.HasPreviousPage.Should().BeFalse();
        response.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void Data_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new PagedResponse<string>();
        var expectedData = new List<string> { "item1", "item2" };

        // Act
        response.Data = expectedData;
        var result = response.Data;

        // Assert
        result.Should().BeEquivalentTo(expectedData);
    }

    [Fact]
    public void PageNumber_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new PagedResponse<string>();
        const int expectedPageNumber = 2;

        // Act
        response.PageNumber = expectedPageNumber;
        var result = response.PageNumber;

        // Assert
        result.Should().Be(expectedPageNumber);
    }

    [Fact]
    public void PageSize_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new PagedResponse<string>();
        const int expectedPageSize = 20;

        // Act
        response.PageSize = expectedPageSize;
        var result = response.PageSize;

        // Assert
        result.Should().Be(expectedPageSize);
    }

    [Fact]
    public void TotalRecords_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new PagedResponse<string>();
        const int expectedTotalRecords = 100;

        // Act
        response.TotalRecords = expectedTotalRecords;
        var result = response.TotalRecords;

        // Assert
        result.Should().Be(expectedTotalRecords);
    }

    [Fact]
    public void TotalPages_SetterAndGetter_ShouldWorkCorrectly()
    {
        // Arrange
        var response = new PagedResponse<string>();
        const int expectedTotalPages = 5;

        // Act
        response.TotalPages = expectedTotalPages;
        var result = response.TotalPages;

        // Assert
        result.Should().Be(expectedTotalPages);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void HasPreviousPage_ShouldReturnCorrectValue(int pageNumber, bool expected)
    {
        // Arrange
        var response = new PagedResponse<string> { PageNumber = pageNumber };

        // Act
        var result = response.HasPreviousPage;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 10, 5, true)]  // Page 1 of 5
    [InlineData(5, 10, 5, false)] // Page 5 of 5
    public void HasNextPage_ShouldReturnCorrectValue(int pageNumber, int pageSize, int totalPages, bool expected)
    {
        // Arrange
        var response = new PagedResponse<string> 
        { 
            PageNumber = pageNumber, 
            PageSize = pageSize, 
            TotalPages = totalPages 
        };

        // Act
        var result = response.HasNextPage;

        // Assert
        result.Should().Be(expected);
    }
}
