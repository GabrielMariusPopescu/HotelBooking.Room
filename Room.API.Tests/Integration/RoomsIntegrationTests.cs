namespace Room.API.Tests.Integration;

[ExcludeFromCodeCoverage]
public class RoomsIntegrationTests(RoomFactory factory): IClassFixture<RoomFactory>, IAsyncDisposable
{
    private readonly HttpClient _client = factory.CreateClient();

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RoomDbContext>();

        dbContext.Rooms.RemoveRange(dbContext.Rooms);
        await dbContext.SaveChangesAsync();
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    #region Create Room Tests

    [Fact]
    public async Task CreateRoom_WhenValidRequest_ReturnsSuccessAndRoom()
    {
        // Arrange
        const string uniqueName = "Luxury apartment";

        CreateRoomRequest request = new()
        {
            Name = uniqueName,
            Number = 101,
            RoomType = RoomType.Apartment.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName(),
            PricePerNight = 50.00M
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/rooms", request, TestContext.Current.CancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            throw new Exception($"API failed with {response.StatusCode}." +
                                $"Details: {error}.");
        }
        
        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var room = await response.Content.ReadFromJsonAsync<Domain.Models.Room>(TestContext.Current.CancellationToken);
        room.Should().NotBeNull();
        room.Id.Should().NotBeEmpty();
        room.Name.Should().Be(request.Name);
        room.Number.Should().Be(request.Number);
        room.RoomType.Should().Be(request.RoomType);
        room.RoomStatus.Should().Be(request.RoomStatus);
        room.PricePerNight.Should().Be(request.PricePerNight);
        room.IsExcluded.Should().BeFalse();
    }

    [Fact]
    public async Task CreateRoom_WhenInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        CreateRoomRequest request = new()
        {
            Name = string.Empty,
            Number = 99,
            RoomType = RoomType.Unknown.GetDisplayName(),
            RoomStatus = RoomStatus.Unknown.GetDisplayName(),
            PricePerNight = 0.0M
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/rooms", request, TestContext.Current.CancellationToken);

        var error = string.Empty;
        if (!response.IsSuccessStatusCode)
            error = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        error.Should().Contain("Room number can not be less than 100.");
        error.Should().Contain("Room type can not be unknown.");
        error.Should().Contain("Room status can not be unknown.");
        error.Should().Contain("Room price per night can not be less than 50.00.");
    }

    #endregion

    #region Update Room Tests

    [Fact]
    public async Task UpdateRoom_WhenValidRequest_ReturnsSuccessAndRoom()
    {
        // Arrange
        CreateRoomRequest createRequest = new()
        {
            Name = "Pre-Update Room",
            Number = 101,
            RoomType = RoomType.Apartment.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName(),
            PricePerNight = 50.00M
        };

        var createResponseMessage = await _client.PostAsJsonAsync("/api/rooms", createRequest, TestContext.Current.CancellationToken);
        var createdRoom = await createResponseMessage.Content.ReadFromJsonAsync<Domain.Models.Room>(TestContext.Current.CancellationToken);

        createdRoom.Should().NotBeNull();

        UpdateRoomRequest updateRequest = new()
        {
            Id = createdRoom.Id,
            Name = "Post-Update Room",
            Number = 102,
            RoomType = RoomType.Hostel.GetDisplayName(),
            RoomStatus = RoomStatus.Maintenance.GetDisplayName(),
            PricePerNight = 25.00M,
            IsExcluded = false,
            LastUpdated = DateTime.UtcNow
        };
        
        // Act
        var updateResponseMessage = await _client.PutAsJsonAsync(
            $"/api/rooms/{createdRoom.Id}", 
            updateRequest,
            TestContext.Current.CancellationToken);

        if (!updateResponseMessage.IsSuccessStatusCode)
        {
            var error = await updateResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            throw new Exception($"API failed with {updateResponseMessage.StatusCode}." +
                                $"Details: {error}.");
        }

        var updatedRoom = await updateResponseMessage.Content.ReadFromJsonAsync<Domain.Models.Room>(TestContext.Current.CancellationToken);

        // Assert
        updateResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedRoom.Should().NotBeNull();
        updatedRoom.Id.Should().NotBeEmpty();
        updatedRoom.Name.Should().NotBe(createdRoom.Name);
        updatedRoom.Number.Should().NotBe(createdRoom.Number);
        updatedRoom.RoomType.Should().NotBe(createdRoom.RoomType);
        updatedRoom.RoomStatus.Should().NotBe(createdRoom.RoomStatus);
        updatedRoom.PricePerNight.Should().NotBe(createdRoom.PricePerNight);
        updatedRoom.IsExcluded.Should().Be(createdRoom.IsExcluded);
        updatedRoom.LastUpdated.Should().NotBeNull();
        updatedRoom.LastUpdated.Value.Should().BeAfter(createdRoom.Created);
    }

    [Fact]
    public async Task UpdateRoom_WhenInvalidRequest_ReturnsBadRequest()
    {
         // Arrange
        CreateRoomRequest createRequest = new()
        {
            Name = "Pre-Update Room",
            Number = 101,
            RoomType = RoomType.Apartment.GetDisplayName(),
            RoomStatus = RoomStatus.Available.GetDisplayName(),
            PricePerNight = 50.00M
        };

        var createResponseMessage = await _client.PostAsJsonAsync("/api/rooms", createRequest, TestContext.Current.CancellationToken);
        var createdRoom = await createResponseMessage.Content.ReadFromJsonAsync<Domain.Models.Room>(TestContext.Current.CancellationToken);

        createdRoom.Should().NotBeNull();

        UpdateRoomRequest updateRequest = new()
        {
            Id = createdRoom.Id,
            Name = "Post-Update Room",
            Number = 102,
            RoomType = "Wrong room type.",
            RoomStatus = "Wrong room status.",
            PricePerNight = 25.00M,
            IsExcluded = false,
            LastUpdated = DateTime.UtcNow
        };
        
        // Act
        var updateResponseMessage = await _client.PutAsJsonAsync(
            $"/api/rooms/{createdRoom.Id}", 
            updateRequest,
            TestContext.Current.CancellationToken);

        var error = string.Empty;
        if (!updateResponseMessage.IsSuccessStatusCode)
            error = await updateResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        
        // Assert
        updateResponseMessage.IsSuccessStatusCode.Should().BeFalse();
        updateResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        error.Should().Contain("Wrong room type.");
    }

    #endregion
}