namespace Room.API.Tests.Commands.Rooms;

[ExcludeFromCodeCoverage] 
public class CreateRoomCommandHandlerTests
{
    private readonly Mock<IRepository<Domain.Models.Room>> _repositoryMock; 
    private readonly CreateRoomCommandHandler _sut;

    public CreateRoomCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Domain.Models.Room>>();
        _sut = new CreateRoomCommandHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenValidRequestAndSaveSucceeds_ReturnsSuccess()
    {
        // Arrange
        var command = CreateValidCommand();
        var expectedRoom = CreateValidRoom(command);

        _repositoryMock
            .Setup(r => r.Add(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync(expectedRoom);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be(command.Name);
        result.Data.Number.Should().Be(command.Number);

        // Verify
        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Once); 
    }

    [Fact]
    public async Task Handle_WhenValidRequestAndSaveFails_ReturnsFailure()
    {
        // Arrange
        var command = CreateValidCommand();

        _repositoryMock
            .Setup(r => r.Add(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync((Domain.Models.Room?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Name); 
        result.Message.Should().Contain("could not be created"); 

        // Verify
        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Once); 
    }

    [Fact]
    public async Task Handle_WhenInvalidRequest_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = CreateInvalidCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain("Errors:");

        // Verify
        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Test Data Factories

    private static CreateRoomCommand CreateValidCommand()
        => new("Ocean View Suite", 101, RoomType.Apartment, RoomStatus.Available, 150.00M); 

    private static CreateRoomCommand CreateInvalidCommand()
        => new(string.Empty, 99, RoomType.Unknown, RoomStatus.Unknown, 0.0M); 

    private static Domain.Models.Room CreateValidRoom(CreateRoomCommand command)
    {
        return new Domain.Models.Room(
            command.Name,
            DateTime.UtcNow,
            command.Number,
            command.RoomType.GetDisplayName(),
            command.RoomStatus.GetDisplayName(),
            command.PricePerNight,
            false
        );
    }

    #endregion
}