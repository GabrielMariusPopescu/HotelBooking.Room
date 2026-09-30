namespace Room.API.Tests.Commands.Bookings;

[ExcludeFromCodeCoverage]
public class UpdateBookingCommandHandlerTests
{
    private readonly Mock<IRepository<Booking>> _repositoryMock;
    private readonly UpdateBookingCommandHandler _sut;

    public UpdateBookingCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Booking>>();
        _sut = new UpdateBookingCommandHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenBookingExistsAndNoOverlap_ReturnsSuccessResponse()
    {
        // Arrange
        var commandId = Guid.NewGuid();
        var command = CreateValidCommand(commandId);
        var existingBooking = CreateValidBooking(commandId);

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>())) 
            .ReturnsAsync(existingBooking);

        _repositoryMock
            .Setup(r => r.Any(
                It.IsAny<Expression<Func<Booking, bool>>>(),
                It.IsAny<CancellationToken>())) 
            .ReturnsAsync(false); 

        _repositoryMock
            .Setup(r => r.Update(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); 

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.CheckIn.Should().Be(command.CheckIn); 
        result.Data.CheckOut.Should().Be(command.CheckOut); 
        result.Data.Guests.Should().Be(command.Guests); 
        result.Data.TotalPrice.Should().Be(1000.00M);
        result.Data.BookingStatus.Should().Be(command.BookingStatus.GetDisplayName()); 
        result.Data.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Booking, object>>[]>()), Times.Once);
        _repositoryMock.Verify(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookingDoesNotExist_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var commandId = Guid.NewGuid();
        var command = CreateValidCommand(commandId);

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                true,
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
        _repositoryMock.Verify(r => r.Get(command.Id, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Booking, object>>[]>()), Times.Once);
        _repositoryMock.Verify(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOverlappingBookingExists_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var commandId = Guid.NewGuid();
        var command = CreateValidCommand(commandId);
        var existingBooking = CreateValidBooking(commandId);

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
            .ReturnsAsync(existingBooking);

        _repositoryMock
            .Setup(r => r.Any(
                It.IsAny<Expression<Func<Booking, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("conflict with an existing reservation"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, true, It.IsAny<CancellationToken>(), It.IsAny<Expression<Func<Booking, object>>[]>()), Times.Once);
        _repositoryMock.Verify(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ReturnsFailureResponse()
    {
        // Arrange
        var commandId = Guid.NewGuid();
        var command = CreateValidCommand(commandId);
        var existingBooking = CreateValidBooking(commandId);

        _repositoryMock
            .Setup(r => r.Get(
                command.Id,
                true,
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
            .ReturnsAsync(existingBooking);

        _repositoryMock
            .Setup(r => r.Any(
                It.IsAny<Expression<Func<Booking, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.Update(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); 

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Id.ToString()); 
        result.Message.Should().Contain("cannot be updated"); 

        // Verify
        _repositoryMock.Verify(r => r.Update(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static UpdateBookingCommand CreateValidCommand(Guid id)
    {
        return new UpdateBookingCommand(
            id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), 
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)), 
            4,
            9999.00M, 
            BookingStatus.Confirmed
        ); 
    }

    private static Booking CreateValidBooking(Guid id)
    {
        return new Booking(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            2,
            500.00M,
            BookingStatus.Pending.GetDisplayName()
        )
        {
            Id = id,
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BookingItems = new List<BookingItem>
            {
                new() { RoomId = Guid.NewGuid(), PricePerNight = 100.00M, Adults = 2 },
                new() { RoomId = Guid.NewGuid(), PricePerNight = 150.00M, Adults = 2 }
            } 
        };
    }

    #endregion
}