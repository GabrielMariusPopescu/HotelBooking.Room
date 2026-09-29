namespace Room.API.Tests.Controllers;

[ExcludeFromCodeCoverage]
public class BookingsControllerTests
{
    private readonly Mock<ISender> _mediatorMock;

    private readonly BookingsController _sut;

    public BookingsControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        _sut = new BookingsController(_mediatorMock.Object);
    }

    #region Create Booking Tests

    [Fact]
    public async Task CreateBooking_WhenValidRequest_SendsCommandAndReturnsSuccess()
    {
        // Arrange
        CreateBookingRequest request = new()
        {
            CheckIn = new DateOnly(2026,1,1),
            CheckOut = new DateOnly(2026,1,10),
            Guests = 2,
            RoomIds = [Guid.NewGuid()]
        };

        var booking = new Domain.Models.Booking(request.CheckIn, request.CheckOut, request.Guests);

        var response = Response<Domain.Models.Booking>.Success(booking);

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<CreateBookingCommand>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Assert
        var actual = await _sut.CreateBooking(request);

        // Act
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<CreatedResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status201Created);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<CreateBookingCommand>(command =>
                            command.CheckIn == request.CheckIn &&
                            command.CheckOut == request.CheckOut &&
                            command.Guests == request.Guests &&
                            command.RoomIds == request.RoomIds),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    [Fact]
    public async Task CreateBooking_WhenInvalidRequest_SendsCommandAndReturnsFailure()
    {
        // Arrange
        CreateBookingRequest request = new()
        {
            CheckIn = new DateOnly(2026, 1, 1),
            CheckOut = new DateOnly(2026, 1, 10),
            Guests = 2,
            RoomIds = [Guid.NewGuid()]
        };

        var response = Response<Domain.Models.Booking>.Failure($"Create {request} booking failed.");

        _mediatorMock
            .Setup(sender => sender.Send(It.IsAny<CreateBookingCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Assert
        var actual = await _sut.CreateBooking(request);

        // Act
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<CreateBookingCommand>(command =>
                            command.CheckIn == request.CheckIn &&
                            command.CheckOut == request.CheckOut &&
                            command.Guests == request.Guests &&
                            command.RoomIds == request.RoomIds),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    #endregion

    #region Update Booking Tests

    [Fact]
    public async Task Update_Booking_WhenValidRequest_SendsCommandAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        UpdateBookingRequest request = new()
        {
            Id = id,
            CheckIn = new DateOnly(2026,1,1),
            CheckOut = new DateOnly(2026,1,10),
            Guests = 3,
            BookingStatus = BookingStatus.Confirmed.GetDisplayName() 
        };

        var booking = new Domain.Models.Booking(
            request.CheckIn,
            request.CheckOut,
            request.Guests);

        var response = Response<Domain.Models.Booking>.Success(booking);

        _mediatorMock
            .Setup(repository => repository.Send(
                It.IsAny<UpdateBookingCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.UpdateBooking(id, request);

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<UpdateBookingCommand>(command =>
                            command.Id == request.Id &&
                            command.CheckIn == request.CheckIn &&
                            command.CheckOut == request.CheckOut &&
                            command.Guests == request.Guests &&
                            command.BookingStatus == Enum.Parse<BookingStatus>(request.BookingStatus)),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    [Fact]
    public async Task UpdateBooking_WhenInvalidRequest_SendsCommandAndReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        UpdateBookingRequest request = new()
        {
            Id = id,
            CheckIn = new DateOnly(2026, 1, 1),
            CheckOut = new DateOnly(2026, 1, 10),
            Guests = 3,
            BookingStatus = BookingStatus.Confirmed.GetDisplayName()
        };

        var response = Response<Domain.Models.Booking>.Failure($"Update {request.Id} booking failed.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<UpdateBookingCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Assert
        var actual = await _sut.UpdateBooking(id, request);

        // Act
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<UpdateBookingCommand>(command =>
                            command.Id == request.Id &&
                            command.CheckIn == request.CheckIn &&
                            command.CheckOut == request.CheckOut &&
                            command.Guests == request.Guests &&
                            command.BookingStatus == Enum.Parse<BookingStatus>(request.BookingStatus)),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    #endregion

    #region Delete Booking Tests

    [Fact]
    public async Task DeleteBooking_WhenValidRequest_SendsCommandAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = Response<Guid>.Success(id);

        _mediatorMock
            .Setup(sender => sender.Send(It.Is<DeleteBookingCommand>(command => command.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.DeleteBooking(id);

        // Assert
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                It.Is<DeleteBookingCommand>(command => command.Id == id),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task DeleteBooking_WhenInvalidRequest_SendsCommandAndReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = Response<Guid>.Failure($"Booking with '{id}' identifier was not found.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.Is<DeleteBookingCommand>(command => command.Id == id),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.DeleteBooking(id);

        // Assert
        var result = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                    It.Is<DeleteBookingCommand>(command => command.Id == id),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    #endregion

    #region Get All Bookings Tests

    [Fact]
    public async Task GetBookings_WhenExisting_SendsQueryAndReturnSuccess()
    {
        // Arrange
        List<Domain.Models.Booking> bookings = [new Domain.Models.Booking()];
        var response = Response<IEnumerable<Domain.Models.Booking>>.Success(bookings);

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetBookingsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetBookings();

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                It.IsAny<GetBookingsQuery>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task GetBookings_WhenNoneExisting_SendQueryAndReturnsFailure()
    {
        // Arrange
        var response = Response<IEnumerable<Domain.Models.Booking>>.Failure("No bookings was found.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetBookingsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetBookings();


        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                It.IsAny<GetBookingsQuery>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    #endregion

    #region Get Booking Tests

    [Fact]
    public async Task GetBooking_WhenValidId_SendQueryDetailsAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        Domain.Models.Booking booking = new();
        var response = Response<Domain.Models.Booking>.Success(booking);

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetBookingDetailsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetBooking(id);

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                    It.IsAny<GetBookingDetailsQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task GetBooking_WhenInvalidId_SendQueryDetailsAndReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = Response<Domain.Models.Booking>.Failure($"Booking with '{id}' identifier not found.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetBookingDetailsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetBooking(id);

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                    It.IsAny<GetBookingDetailsQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    #endregion
}