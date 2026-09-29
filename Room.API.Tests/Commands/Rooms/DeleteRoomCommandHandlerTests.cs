using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Room.API.Tests.Commands.Rooms;

[ExcludeFromCodeCoverage]
public class DeleteRoomCommandHandlerTests
{
    private readonly Mock<IRepository<Domain.Models.Room>> _repositoryMock; 
    private readonly DeleteRoomCommandHandler _sut;

    public DeleteRoomCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Domain.Models.Room>>();
        _sut = new DeleteRoomCommandHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenRoomExistsAndDisableSucceeds_ReturnsSuccessResponse()
    {
        // Arrange
        var command = new DeleteRoomCommand(Guid.NewGuid());
        var existingRoom = CreateValidRoom(command.Id);

        _repositoryMock
            .Setup(r => r.Get(command.Id, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync(existingRoom);

        _repositoryMock
            .Setup(r => r.Disable(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().Be(existingRoom.Id); 

        existingRoom.IsExcluded.Should().BeTrue();
        existingRoom.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Disable(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoomDoesNotExist_ReturnsFailureAndShortCircuits()
    {
        // Arrange
        var command = new DeleteRoomCommand(Guid.NewGuid());

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
        _repositoryMock.Verify(r => r.Disable(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDisableFails_ReturnsFailureResponse()
    {
        // Arrange
        var command = new DeleteRoomCommand(Guid.NewGuid());
        var existingRoom = CreateValidRoom(command.Id);

        _repositoryMock
            .Setup(r => r.Get(command.Id, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync(existingRoom);

        _repositoryMock
            .Setup(r => r.Disable(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>())) 
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(command.Id.ToString()); 
        result.Message.Should().Contain("could not be disabled"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(command.Id, false, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.Disable(It.IsAny<Domain.Models.Room>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static Domain.Models.Room CreateValidRoom(Guid id)
    {
        return new Domain.Models.Room(
            "Ocean View Suite",
            DateTime.UtcNow,
            101,
            "Apartment",
            "Available",
            150.00M,
            false
        )
        {
            Id = id 
        };
    }

    #endregion
}