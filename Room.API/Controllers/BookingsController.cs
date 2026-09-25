namespace Room.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Tags("Bookings")]
public class BookingsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Response<Booking>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        var command = new CreateBookingCommand(
            request.CheckIn,
            request.CheckOut,
            request.Guests,
            request.TotalPrice,
            Enum.Parse<BookingStatus>(request.BookingStatus));
        
        var response = await mediator.Send(command);
        return response.IsSuccessful
            ? Created("api/bookings", response.Data)
            : BadRequest(response.Message);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(Response<IEnumerable<Booking>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookings()
    {
        var query = new GetBookingsQuery();
        var response = await mediator.Send(query);
        return response.IsSuccessful
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Response<Booking>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBooking([FromRoute]Guid id)
    {
        var query = new GetBookingDetailsQuery(id);
        var response = await mediator.Send(query);
        return response.IsSuccessful
            ? Ok(response.Data)
            : NotFound(response.Message);
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response<Booking>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] UpdateBookingRequest request)
    {
        var command = new UpdateBookingCommand(
            id,
            request.CheckIn,
            request.CheckOut,
            request.Guests,
            request.TotalPrice,
            Enum.Parse<BookingStatus>(request.BookingStatus));

        var response = await mediator.Send(command);
        return response.IsSuccessful
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Response<Booking>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBooking([FromRoute]Guid id)
    {
        var command = new DeleteBookingCommand(id);
        var response = await mediator.Send(command);
        return response.IsSuccessful
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}
