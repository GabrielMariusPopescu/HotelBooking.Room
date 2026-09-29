namespace Room.API.Tests.Queries.Rooms;

[ExcludeFromCodeCoverage]
public class GetRoomDetailsQueryHandlerTests
{
    private readonly Mock<IRepository<Domain.Models.Room>> _repositoryMock; 
    private readonly GetRoomDetailsQueryHandler _sut;

    public GetRoomDetailsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Domain.Models.Room>>();
        _sut = new GetRoomDetailsQueryHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenRoomExists_ReturnsSuccessResponse()
    {
        // Arrange
        var queryId = Guid.NewGuid();
        var query = new GetRoomDetailsQuery(queryId); 
        var existingRoom = CreateValidRoom(queryId);

        _repositoryMock
            .Setup(r => r.Get(queryId, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync(existingRoom);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(queryId);

        // Verify
        _repositoryMock.Verify(r => r.Get(queryId, false, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoomDoesNotExist_ReturnsFailureResponse()
    {
        // Arrange
        var queryId = Guid.NewGuid();
        var query = new GetRoomDetailsQuery(queryId); 

        _repositoryMock
            .Setup(r => r.Get(queryId, false, It.IsAny<CancellationToken>())) 
            .ReturnsAsync((Domain.Models.Room?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Contain(queryId.ToString()); 
        result.Message.Should().Contain("was not found"); 

        // Verify
        _repositoryMock.Verify(r => r.Get(queryId, false, It.IsAny<CancellationToken>()), Times.Once);
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
