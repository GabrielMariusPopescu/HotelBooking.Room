namespace Room.API.Tests.Controllers;

[ExcludeFromCodeCoverage]
public class RoomsControllerTests
{
    private readonly Mock<ISender> _mediatorMock;

    private readonly RoomsController _sut;

    public RoomsControllerTests()
    {
        _mediatorMock = new Mock<ISender>();
        _sut = new RoomsController(_mediatorMock.Object);
    }

    #region Create Room Tests

    [Fact]
    public async Task CreateRoom_WhenValidRequest_SendsCommandAndReturnsSuccess()
    {
        // Arrange
        CreateRoomRequest request = new()
        {
            Name = "Luxury",
            Number = 101,
            PricePerNight = 100.00M,
            RoomType = RoomType.Apartment.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName()
        };

        var room = new Domain.Models.Room(
            request.Name,
            DateTime.UtcNow,
            request.Number,
            request.RoomType,
            request.RoomStatus,
            request.PricePerNight,
            false);

        var response = RoomResponse<Domain.Models.Room>.Success(room);

        _mediatorMock
            .Setup(sender => sender.Send(It.IsAny<CreateRoomCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Assert
        var actual = await _sut.CreateRoom(request);

        // Act
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<CreatedResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status201Created);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<CreateRoomCommand>(command =>
                            command.Name == request.Name &&
                            command.Number == request.Number &&
                            command.PricePerNight == request.PricePerNight &&
                            command.RoomType == Enum.Parse<RoomType>(request.RoomType) &&
                            command.RoomStatus == Enum.Parse<RoomStatus>(request.RoomStatus)),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    [Fact]
    public async Task CreateRoom_WhenInvalidRequest_SendsCommandAndReturnsFailure()
    {
        // Arrange
        CreateRoomRequest request = new()
        {
            Name = "Luxury",
            Number = 101,
            PricePerNight = 100.00M,
            RoomType = RoomType.Apartment.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName()
        };

        var response = RoomResponse<Domain.Models.Room>.Failure($"Create {request.Name} room failed.");

        _mediatorMock
            .Setup(sender => sender.Send(It.IsAny<CreateRoomCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Assert
        var actual = await _sut.CreateRoom(request);

        // Act
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<CreateRoomCommand>(command =>
                            command.Name == request.Name &&
                            command.Number == request.Number &&
                            command.PricePerNight == request.PricePerNight &&
                            command.RoomType == Enum.Parse<RoomType>(request.RoomType) &&
                            command.RoomStatus == Enum.Parse<RoomStatus>(request.RoomStatus)),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    #endregion

    #region Update Room Tests

    [Fact]
    public async Task Update_Room_WhenValidRequest_SendsCommandAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        UpdateRoomRequest request = new()
        {
            Id = id,
            Name = "Luxury Smart",
            Number = 101,
            RoomType = RoomType.Suite.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName(),
            PricePerNight = 200.00M
        };

        var room = new Domain.Models.Room(
            request.Name, 
            DateTime.UtcNow, 
            request.Number, 
            request.RoomType,
            request.RoomStatus, 
            request.PricePerNight, 
            false);

        var response = RoomResponse<Domain.Models.Room>.Success(room);

        _mediatorMock
            .Setup(repository => repository.Send(It.IsAny<UpdateRoomCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        // Act
        var actual = await _sut.UpdateRoom(id, request);
        
        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<UpdateRoomCommand>(command =>
                            command.Id == request.Id &&
                            command.Name == request.Name &&
                            command.Number == request.Number &&
                            command.PricePerNight == request.PricePerNight &&
                            command.RoomType == Enum.Parse<RoomType>(request.RoomType) &&
                            command.RoomStatus == Enum.Parse<RoomStatus>(request.RoomStatus)),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    [Fact]
    public async Task UpdateRoom_WhenInvalidRequest_SendsCommandAndReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        UpdateRoomRequest request = new()
        {
            Id = id,
            Name = "Luxury",
            Number = 101,
            PricePerNight = 100.00M,
            RoomType = RoomType.Apartment.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName()
        };

        var response = RoomResponse<Domain.Models.Room>.Failure($"Update {request.Id} room failed.");

        _mediatorMock
            .Setup(sender => sender.Send(It.IsAny<UpdateRoomCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Assert
        var actual = await _sut.UpdateRoom(id, request);

        // Act
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Verify
        _mediatorMock
            .Verify(sender =>
                    sender.Send(It.Is<UpdateRoomCommand>(command =>
                            command.Id == request.Id &&
                            command.Name == request.Name &&
                            command.Number == request.Number &&
                            command.PricePerNight == request.PricePerNight &&
                            command.RoomType == Enum.Parse<RoomType>(request.RoomType) &&
                            command.RoomStatus == Enum.Parse<RoomStatus>(request.RoomStatus)),
                        It.IsAny<CancellationToken>()),
                Times.Once());
    }

    #endregion

    #region Delete Room Tests

    [Fact]
    public async Task DeleteRoom_WhenValidRequest_SendsCommandAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = RoomResponse<Guid>.Success(id);

        _mediatorMock
            .Setup(sender => sender.Send(It.Is<DeleteRoomCommand>(command => command.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        // Act
        var actual = await _sut.DeleteRoom(id);
        
        // Assert
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        
        // Verify
_mediatorMock
    .Verify(sender => sender.Send(
        It.Is<DeleteRoomCommand>(command => command.Id == id),
        It.IsAny<CancellationToken>()), 
        Times.Once);
    }

    [Fact]
    public async Task DeleteRoom_WhenInvalidRequest_SendsCommandAndReturnsFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = RoomResponse<Guid>.Failure($"Room with '{id}' identifier was not found.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.Is<DeleteRoomCommand>(command => command.Id == id), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.DeleteRoom(id);

        // Assert
        var result = actual.Should().BeOfType<BadRequestObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                    It.Is<DeleteRoomCommand>(command => command.Id == id),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    #endregion

    #region Get All Rooms Tests

    [Fact]
    public async Task GetRooms_WhenExisting_SendsQueryAndReturnSuccess()
    {
        // Arrange
        List<Domain.Models.Room> rooms = [new Domain.Models.Room()];
        var response = RoomResponse<IEnumerable<Domain.Models.Room>>.Success(rooms);

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetRoomsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        // Act
        var actual = await _sut.GetRooms();
        
        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        
        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                It.IsAny<GetRoomsQuery>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task GetRooms_WhenNoneExisting_SendQueryAndReturnsFailure()
    {
        // Arrange
        var response = RoomResponse<IEnumerable<Domain.Models.Room>>.Failure("No rooms was found.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetRoomsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetRooms();


        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                It.IsAny<GetRoomsQuery>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }
    
    #endregion

    #region Get Room Tests

    [Fact]
    public async Task GetRoom_WhenValidId_SendQueryDetailsAndReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        Domain.Models.Room room = new();
        var response = RoomResponse<Domain.Models.Room>.Success(room);

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetRoomDetailsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetRoom(id);

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<OkObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status200OK);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                    It.IsAny<GetRoomDetailsQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task GetRoom_WhenInvalidId_SendQueryDetailsAndReturnFailure()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = RoomResponse<Domain.Models.Room>.Failure($"Room with '{id}' identifier not found.");

        _mediatorMock
            .Setup(sender => sender.Send(
                It.IsAny<GetRoomDetailsQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var actual = await _sut.GetRoom(id);

        // Assert
        actual.Should().NotBeNull();
        var result = actual.Should().BeOfType<NotFoundObjectResult>().Subject;
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        // Verify
        _mediatorMock
            .Verify(sender => sender.Send(
                    It.IsAny<GetRoomDetailsQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
    }
    
    #endregion
}