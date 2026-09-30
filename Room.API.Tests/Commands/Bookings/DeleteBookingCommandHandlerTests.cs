namespace Room.API.Tests.Commands.Bookings;

[ExcludeFromCodeCoverage]
public class DeleteBookingCommandHandlerTests
{
    private readonly Mock<IRepository<Booking>> _repositoryMock;
    private readonly DeleteBookingCommandHandler _sut;

    public DeleteBookingCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Booking>>();
        _sut = new DeleteBookingCommandHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenBookingExistsAndDisableSucceeds_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new DeleteBookingCommand(Guid.NewGuid()); 
        var existingBooking = CreateValidBooking(command.Id);

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                false,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>())) 
            .ReturnsAsync(existingBooking);

        _repositoryMock
            .Setup(r => r.Disable(It.IsAny<Booking>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be(command.Id);
        existingBooking.BookingStatus.Should().Be(BookingStatus.Cancelled.GetDisplayName()); 
        existingBooking.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1)); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Booking, object>>[]>()), Times.Once);
        _repositoryMock.Verify(r => r.Disable(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingDoesNotExist_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = new DeleteBookingCommand(Guid.NewGuid()); 

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                false,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Id.ToString()); 
        result.Message.Should().Contain("could not be found"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Booking, object>>[]>()), Times.Once);
        _repositoryMock.Verify(r => r.Disable(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDisableFails_ReturnsFailureResponse()
    {
        // Arrange
        var command = new DeleteBookingCommand(Guid.NewGuid()); 
        var existingBooking = CreateValidBooking(command.Id);

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                false,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
            .ReturnsAsync(existingBooking);

        _repositoryMock
            .Setup(r => r.Disable(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Id.ToString()); 
        result.Message.Should().Contain("could not be disabled"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Booking, object>>[]>()), Times.Once);
        _repositoryMock.Verify(r => r.Disable(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static Booking CreateValidBooking(Guid id)
    {
        return new Booking(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            2,
            500.00M,
            BookingStatus.Confirmed.GetDisplayName()
        )
        {
            Id = id,
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };
    }

    #endregion
}