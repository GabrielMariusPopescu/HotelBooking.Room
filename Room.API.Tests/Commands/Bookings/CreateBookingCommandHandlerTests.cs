namespace Room.API.Tests.Commands.Bookings;

[ExcludeFromCodeCoverage]
public class CreateBookingCommandHandlerTests
{
    private readonly Mock<IRepository<Booking>> _bookingRepositoryMock;
    private readonly Mock<IRepository<Domain.Models.Room>> _roomRepositoryMock;
    private readonly CreateBookingCommandHandler _sut;

    public CreateBookingCommandHandlerTests()
    {
        _bookingRepositoryMock = new Mock<IRepository<Booking>>();
        _roomRepositoryMock = new Mock<IRepository<Domain.Models.Room>>();

        _sut = new CreateBookingCommandHandler(
            _bookingRepositoryMock.Object,
            _roomRepositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = CreateValidCommand();
        var expectedRooms = CreateValidRooms(command.RoomIds.ToList());
        var expectedBooking = CreateValidBooking(command);

        _bookingRepositoryMock
            .Setup(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); 

        _roomRepositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRooms); 

        // 3. Mock database persistence to succeed
        _bookingRepositoryMock
            .Setup(r => r.Add(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBooking); 

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.CheckIn.Should().Be(command.CheckIn);

        // Verify the entire chain executed properly
        _bookingRepositoryMock.Verify(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _roomRepositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
        _bookingRepositoryMock.Verify(r => r.Add(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOverlappingBookingExists_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = CreateValidCommand();

        // Simulates an existing booking overlapping with the requested dates/rooms
        _bookingRepositoryMock
            .Setup(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); 

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("already booked"); 

        // Ensures the handler completely bypassed room fetching and database insertion
        _bookingRepositoryMock.Verify(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        _roomRepositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Never);
        _bookingRepositoryMock.Verify(r => r.Add(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRoomsDoNotExist_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = CreateValidCommand();

        _bookingRepositoryMock
            .Setup(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roomRepositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<Domain.Models.Room>); 

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("requested rooms do not exist"); 

        _roomRepositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
        _bookingRepositoryMock.Verify(r => r.Add(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDatabaseAddFails_ReturnsFailureResponse()
    {
        // Arrange
        var command = CreateValidCommand();
        var expectedRooms = CreateValidRooms(command.RoomIds.ToList());

        _bookingRepositoryMock
            .Setup(r => r.Any(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roomRepositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRooms);

        _bookingRepositoryMock
            .Setup(r => r.Add(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("could not be created"); 

        _bookingRepositoryMock.Verify(r => r.Add(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static CreateBookingCommand CreateValidCommand()
    {
        return new CreateBookingCommand(
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            2,
            new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        ); 
    }

    private static IEnumerable<Domain.Models.Room> CreateValidRooms(IEnumerable<Guid> roomIds)
    {
        return roomIds.Select(id => new Domain.Models.Room(
            "Test Suite",
            DateTime.UtcNow,
            101,
            "Suite",
            "Available",
            150.00M,
            false)
        {
            Id = id
        }).ToList();
    }

    private static Booking CreateValidBooking(CreateBookingCommand command)
    {
        return new Booking(
            command.CheckIn,
            command.CheckOut,
            command.Guests,
            1500.00M, // Arbitrary total price for testing
            "Pending"
        )
        {
            Id = Guid.NewGuid(),
            BookingItems = command.RoomIds.Select(id => new BookingItem { RoomId = id }).ToList()
        };
    }

    #endregion
}