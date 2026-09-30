namespace Room.API.Tests.Queries.Bookings;

[ExcludeFromCodeCoverage]
public class GetBookingsQueryHandlerTests
{
    private readonly Mock<IRepository<Booking>> _repositoryMock;
    private readonly GetBookingsQueryHandler _sut;

    public GetBookingsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Booking>>();
        _sut = new GetBookingsQueryHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenBookingsExist_ReturnsSuccessResponse()
    {
        // Arrange
        var query = new GetBookingsQuery(); 
        var expectedBookings = new List<Booking> { CreateValidBooking() };

        _repositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>())) 
            .ReturnsAsync(expectedBookings);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();
        result.Data.Should().BeEquivalentTo(expectedBookings);

        // Verify
        _repositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoBookingsExist_ReturnsFailureResponse()
    {
        // Arrange
        var query = new GetBookingsQuery(); 

        _repositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>())) 
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be("No bookings found."); 

        // Verify
        _repositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    // Encapsulates entity instantiation to satisfy the Single Responsibility Principle
    private static Booking CreateValidBooking()
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            TotalPrice = 500.00m,
            BookingStatus = BookingStatus.Confirmed.GetDisplayName(),
            BookingItems = new List<BookingItem>()
        };
    }

    #endregion
}