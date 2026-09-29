namespace Room.API.Tests.Queries.Rooms;

[ExcludeFromCodeCoverage]
public class GetRoomsQueryHandlerTests
{
    private readonly Mock<IRepository<Domain.Models.Room>> _repositoryMock; 
    private readonly GetRoomsQueryHandler _sut;

    public GetRoomsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<Domain.Models.Room>>();
        _sut = new GetRoomsQueryHandler(_repositoryMock.Object); 
    }

    #region Handle Tests

    [Fact]
    public async Task Handle_WhenRoomsExist_ReturnsSuccessResponse()
    {
        // Arrange
        var query = new GetRoomsQuery(); 
        var expectedRooms = new List<Domain.Models.Room> { CreateValidRoom() };

        _repositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>())) 
            .ReturnsAsync(expectedRooms);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Data.Should().NotBeNullOrEmpty();
        result.Data.Should().BeEquivalentTo(expectedRooms);

        // Verify
        _repositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoRoomsExist_ReturnsFailureResponse()
    {
        // Arrange
        var query = new GetRoomsQuery(); 

        _repositoryMock
            .Setup(r => r.Get(It.IsAny<CancellationToken>())) 
            .ReturnsAsync([]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be("No rooms were found."); 

        // Verify
        _repositoryMock.Verify(r => r.Get(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data Factories

    private static Domain.Models.Room CreateValidRoom()
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
            Id = Guid.NewGuid()
        };
    }

    #endregion
}
