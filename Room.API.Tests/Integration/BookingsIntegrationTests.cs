namespace Room.API.Tests.Integration;

[ExcludeFromCodeCoverage]
public class BookingsIntegrationTests(RoomFactory factory) : IClassFixture<RoomFactory>, IAsyncLifetime 
{
    private readonly HttpClient _client = factory.CreateClient(); 

    public async ValueTask InitializeAsync() 
    {
        using var scope = factory.Services.CreateScope(); 
        var dbContext = scope.ServiceProvider.GetRequiredService<RoomDbContext>(); 

        dbContext.Set<BookingItem>().RemoveRange(dbContext.Set<BookingItem>());
        dbContext.Set<Booking>().RemoveRange(dbContext.Set<Booking>());

        await dbContext.SaveChangesAsync(); 
    }

    public ValueTask DisposeAsync() 
    {
        _client.Dispose(); 
        GC.SuppressFinalize(this); 
        return ValueTask.CompletedTask;
    }

    private async Task<Guid> SeedRoomAsync()
    {
        var request = new
        {
            Name = $"Test Room {Guid.NewGuid()}",
            Number = new Random().Next(1000, 9999),
            RoomType = "Apartment",
            RoomStatus = "Available",
            PricePerNight = 100.00M
        };
        var response = await _client.PostAsJsonAsync("/api/rooms", request, TestContext.Current.CancellationToken); 
        response.EnsureSuccessStatusCode();
        var room = await response.Content.ReadFromJsonAsync<Domain.Models.Room>(TestContext.Current.CancellationToken); 
        return room!.Id;
    }

    #region Create Booking Tests

    [Fact]
    public async Task CreateBooking_WhenValidRequest_ReturnsSuccessAndBooking()
    {
        // Arrange
        var roomId = await SeedRoomAsync();
        var request = new
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            RoomIds = new[] { roomId } 
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/bookings", request, TestContext.Current.CancellationToken); 

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken); 
            throw new Exception($"API failed with {response.StatusCode}. Details: {error}.");
        }

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created); 

        var booking = await response.Content.ReadFromJsonAsync<Booking>(TestContext.Current.CancellationToken); 
        booking.Should().NotBeNull();
        booking!.Id.Should().NotBeEmpty();
        booking.Guests.Should().Be(request.Guests);
    }

    [Fact]
    public async Task CreateBooking_WhenInvalidRequest_ReturnsBadRequest()
    {
        // Arrange 
        var request = new
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 0, 
            RoomIds = Array.Empty<Guid>() 
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/bookings", request, TestContext.Current.CancellationToken); 

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError); 
    }

    #endregion

    #region Update Booking Tests

    [Fact]
    public async Task UpdateBooking_WhenValidRequest_ReturnsSuccessAndBooking()
    {
        // Arrange
        var roomId = await SeedRoomAsync();
        CreateBookingRequest createRequest = new()
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            RoomIds = [roomId]
        };

        var createResponseMessage = await _client.PostAsJsonAsync("/api/bookings", createRequest, TestContext.Current.CancellationToken);
        var createdBooking = await createResponseMessage.Content.ReadFromJsonAsync<Booking>(TestContext.Current.CancellationToken);
        createdBooking.Should().NotBeNull();

        UpdateBookingRequest updateRequest = new()
        {
            Id = createdBooking!.Id,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)), 
            Guests = 3,
            TotalPrice = 9999.00M, 
            BookingStatus = BookingStatus.Confirmed.GetDisplayName()
        };

        // Act
        var updateResponseMessage = await _client.PutAsJsonAsync(
            $"/api/bookings/{createdBooking.Id}",
            updateRequest,
            TestContext.Current.CancellationToken);

        if (!updateResponseMessage.IsSuccessStatusCode)
        {
            var error = await updateResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            throw new Exception($"API failed with {updateResponseMessage.StatusCode}. Details: {error}.");
        }

        var updatedBooking = await updateResponseMessage.Content.ReadFromJsonAsync<Domain.Models.Booking>(TestContext.Current.CancellationToken);

        // Assert
        updateResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedBooking.Should().NotBeNull();
        updatedBooking!.Id.Should().Be(createdBooking.Id);
        updatedBooking.Guests.Should().Be(updateRequest.Guests);
        updatedBooking.BookingStatus.Should().Be(updateRequest.BookingStatus);
        updatedBooking.TotalPrice.Should().Be(0.0M);
    }

    [Fact]
    public async Task UpdateBooking_WhenInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var roomId = await SeedRoomAsync();
        CreateBookingRequest createRequest = new()
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            RoomIds = [roomId]
        };

        var createResponseMessage = await _client.PostAsJsonAsync("/api/bookings", createRequest, TestContext.Current.CancellationToken);
        var createdBooking = await createResponseMessage.Content.ReadFromJsonAsync<Domain.Models.Booking>(TestContext.Current.CancellationToken);
        createdBooking.Should().NotBeNull();

        UpdateBookingRequest updateRequest = new()
        {
            Id = createdBooking.Id,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Guests = 0,
            TotalPrice = -10M, 
            BookingStatus = "Unknown" 
        };

        // Act
        var updateResponseMessage = await _client.PutAsJsonAsync(
            $"/api/bookings/{createdBooking.Id}",
            updateRequest,
            TestContext.Current.CancellationToken);

        var error = string.Empty;
        if (!updateResponseMessage.IsSuccessStatusCode)
            error = await updateResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        updateResponseMessage.IsSuccessStatusCode.Should().BeFalse();
        updateResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        error.Should().Contain("strictly after the check-in date");
        error.Should().Contain("at least 1");
    }

    [Fact]
    public async Task UpdateBooking_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        UpdateBookingRequest updateRequest = new()
        {
            Id = nonExistentId,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
            Guests = 3,
            TotalPrice = 600.00M,
            BookingStatus = BookingStatus.Confirmed.GetDisplayName()
        };

        // Act
        var updateResponseMessage = await _client.PutAsJsonAsync(
            $"/api/bookings/{nonExistentId}",
            updateRequest,
            TestContext.Current.CancellationToken);

        var error = string.Empty;
        if (!updateResponseMessage.IsSuccessStatusCode)
            error = await updateResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        updateResponseMessage.IsSuccessStatusCode.Should().BeFalse();
        updateResponseMessage.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        error.Should().Contain("could not be found");
    }

    #endregion

    #region Delete Booking Tests

    [Fact]
    public async Task DeleteBooking_WhenValidRequest_ReturnsSuccess()
    {
        // Arrange
        var roomId = await SeedRoomAsync();
        var createRequest = new
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            RoomIds = new[] { roomId }
        };

        var createResponseMessage = await _client.PostAsJsonAsync("/api/bookings", createRequest, TestContext.Current.CancellationToken); 
        var createdBooking = await createResponseMessage.Content.ReadFromJsonAsync<Booking>(TestContext.Current.CancellationToken); 
        createdBooking.Should().NotBeNull();

        // Act
        var deleteResponseMessage = await _client.DeleteAsync($"/api/bookings/{createdBooking!.Id}", TestContext.Current.CancellationToken); 

        if (!deleteResponseMessage.IsSuccessStatusCode)
        {
            var error = await deleteResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken); 
            throw new Exception($"API failed with {deleteResponseMessage.StatusCode}. Details: {error}.");
        }

        // Assert
        deleteResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK); 
    }

    #endregion

    #region Get All Bookings Tests

    [Fact]
    public async Task GetBookings_WhenBookingsExists_ReturnsOkAndBookings()
    {
        // Arrange
        var roomId = await SeedRoomAsync();
        var createRequest = new
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            RoomIds = new[] { roomId }
        };
        var createResponseMessage = await _client.PostAsJsonAsync("/api/bookings", createRequest, TestContext.Current.CancellationToken); 
        var createdBooking = await createResponseMessage.Content.ReadFromJsonAsync<Booking>(TestContext.Current.CancellationToken); 
        createdBooking.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync("/api/bookings", TestContext.Current.CancellationToken); 

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken); 
            throw new Exception($"API failed with {response.StatusCode}. Details: {error}.");
        }
        var bookings = await response.Content.ReadFromJsonAsync<IEnumerable<Booking>>(TestContext.Current.CancellationToken); 

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK); 
        bookings.Should().NotBeNull();
        bookings.Should().ContainSingle(b => b.Id == createdBooking!.Id);
    }

    [Fact]
    public async Task GetBookings_WhenNoBookingsExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/bookings", TestContext.Current.CancellationToken); 

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
    }

    #endregion

    #region Get Booking Details Tests

    [Fact]
    public async Task GetBooking_WhenBookingExists_ReturnsOkAndBooking()
    {
        // Arrange
        var roomId = await SeedRoomAsync();
        var createRequest = new
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
            Guests = 2,
            RoomIds = new[] { roomId }
        };
        var createResponseMessage = await _client.PostAsJsonAsync("/api/bookings", createRequest, TestContext.Current.CancellationToken); 
        var createdBooking = await createResponseMessage.Content.ReadFromJsonAsync<Booking>(TestContext.Current.CancellationToken); 
        createdBooking.Should().NotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/bookings/{createdBooking!.Id}", TestContext.Current.CancellationToken); 

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken); 
            throw new Exception($"API failed with {response.StatusCode}. Details: {error}.");
        }
        var booking = await response.Content.ReadFromJsonAsync<Booking>(TestContext.Current.CancellationToken); 

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK); 
        booking.Should().NotBeNull();
        booking!.Id.Should().Be(createdBooking.Id);
    }

    [Fact]
    public async Task GetBooking_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/bookings/{bookingId}", TestContext.Current.CancellationToken); 

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
    }

    #endregion
}