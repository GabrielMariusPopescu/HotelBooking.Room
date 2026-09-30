namespace Room.API.Tests.Queries.Bookings;

[ExcludeFromCodeCoverage]
public class GetBookingDetailsQueryHandlerTests
{
    private readonly Mock<IRepository<Booking>> _repositoryMock;
    private readonly GetBookingDetailsQueryHandler _sut;

    public GetBookingDetailsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Booking>>();
        _sut = new GetBookingDetailsQueryHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenBookingExists_ReturnsSuccessResponse()
    {
        // Arrange
        var queryId = Guid.NewGuid();
        var query = new GetBookingDetailsQuery(queryId); 
        var existingBooking = CreateValidBooking(queryId);

        _repositoryMock
            .Setup(r => r.Get(
                queryId,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
            .ReturnsAsync(existingBooking);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(queryId);

        // Verify
        _repositoryMock.Verify(r => r.Get(
            queryId,
            true,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Booking, object>>[]>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingDoesNotExist_ReturnsFailureResponse()
    {
        // Arrange
        var queryId = Guid.NewGuid();
        var query = new GetBookingDetailsQuery(queryId); 

        _repositoryMock
            .Setup(r => r.Get(
                queryId,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(queryId.ToString()); 
        result.Message.Should().Contain("was not found"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(
            queryId,
            true,
            It.IsAny<CancellationToken>(),
            It.IsAny<Expression<Func<Booking, object>>[]>()),
            Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static Booking CreateValidBooking(Guid id)
    {
        return new Booking
        {
            Id = id,
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