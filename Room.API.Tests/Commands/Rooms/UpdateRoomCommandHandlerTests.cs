namespace Room.API.Tests.Commands.Rooms;

[ExcludeFromCodeCoverage]
public class UpdateRoomCommandHandlerTests
{
    private readonly Mock<IRepository<Domain.Models.Room>> _repositoryMock; 
    private readonly UpdateRoomCommandHandler _sut;

    public UpdateRoomCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Domain.Models.Room>>();
        _sut = new UpdateRoomCommandHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenRoomExistsAndValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingRoom = CreateValidRoom(command.Id);

        _repositoryMock
            .Setup(r => r.Get(command.Id, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync(existingRoom);

        _repositoryMock
            .Setup(r => r.Update(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();

        // Verify
        result.Data.Name.Should().Be(command.Name);
        result.Data.Number.Should().Be(command.Number);
        result.Data.PricePerNight.Should().Be(command.PricePerNight);

        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoomDoesNotExist_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = CreateValidCommand();

        _repositoryMock
            .Setup(r => r.Get(command.Id, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync((Domain.Models.Room?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Id.ToString()); 
        result.Message.Should().Contain("could not be found"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = CreateInvalidCommand();
        var existingRoom = CreateValidRoom(command.Id);

        _repositoryMock
            .Setup(r => r.Get(command.Id, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync(existingRoom);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("Errors:"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ReturnsFailureResponse()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingRoom = CreateValidRoom(command.Id);

        _repositoryMock
            .Setup(r => r.Get(command.Id, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync(existingRoom);

        _repositoryMock
            .Setup(r => r.Update(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Id.ToString()); 
        result.Message.Should().Contain("could not be updated"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static UpdateRoomCommand CreateValidCommand()
    {
        return new UpdateRoomCommand(
            Guid.NewGuid(),
            "Post-Update Suite",
            202,
            RoomType.Apartment,
            RoomStatus.Available,
            250.00M,
            false,
            DateTime.UtcNow
        ); 
    }

    private static UpdateRoomCommand CreateInvalidCommand()
    {
        return new UpdateRoomCommand(
            Guid.NewGuid(),
            string.Empty,
            0,
            RoomType.Unknown,
            RoomStatus.Unknown,
            -10.00M,
            false,
            DateTime.UtcNow
        ); 
    }

    private static Domain.Models.Room CreateValidRoom(Guid id)
    {
        return new Domain.Models.Room(
            "Pre-Update Suite",
            DateTime.UtcNow.AddDays(-1),
            101,
            "Hostel",
            "Maintenance",
            100.00M,
            false
        )
        {
            Id = id
        };
    }

    #endregion
}
