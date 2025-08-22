using FluentAssertions;
using Hackathon.Application.Results;

namespace Hackathon.Application.Tests.Results;

public class PagedResultTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var data = new List<string> { "item1", "item2", "item3" };
        const int currentPage = 2;
        const int pageSize = 10;
        const int totalItems = 25;

        // Act
        var result = new PagedResult<string>(data, totalItems, currentPage, pageSize);

        // Assert
        result.Items.Should().BeEquivalentTo(data);
        result.CurrentPage.Should().Be(currentPage);
        result.PageSize.Should().Be(pageSize);
        result.TotalItems.Should().Be(totalItems);
    }

    [Theory]
    [InlineData(1, 10, 25, 3)] // 25 records, 10 per page = 3 pages
    [InlineData(1, 5, 12, 3)]  // 12 records, 5 per page = 3 pages
    [InlineData(1, 10, 10, 1)] // 10 records, 10 per page = 1 page
    public void TotalPages_ShouldCalculateCorrectly(int currentPage, int pageSize, int totalItems, int expectedTotalPages)
    {
        // Arrange
        var data = new List<string>();
        var result = new PagedResult<string>(data, totalItems, currentPage, pageSize);

        // Act
        var totalPages = result.TotalPages;

        // Assert
        totalPages.Should().Be(expectedTotalPages);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(5, true)]
    public void HasPreviousPage_ShouldReturnCorrectValue(int currentPage, bool expected)
    {
        // Arrange
        var data = new List<string>();
        var result = new PagedResult<string>(data, 100, currentPage, 10);

        // Act
        var hasPrevious = result.HasPreviousPage;

        // Assert
        hasPrevious.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 10, 25, true)]  // Page 1 of 3
    [InlineData(2, 10, 25, true)]  // Page 2 of 3
    [InlineData(3, 10, 25, false)] // Page 3 of 3
    [InlineData(1, 10, 10, false)] // Page 1 of 1
    public void HasNextPage_ShouldReturnCorrectValue(int currentPage, int pageSize, int totalItems, bool expected)
    {
        // Arrange
        var data = new List<string>();
        var result = new PagedResult<string>(data, totalItems, currentPage, pageSize);

        // Act
        var hasNext = result.HasNextPage;

        // Assert
        hasNext.Should().Be(expected);
    }
}
