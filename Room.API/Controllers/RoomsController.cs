namespace Room.API.Controllers;

[ApiController]
[Route("api/rooms")]
[Tags("Rooms")]
public class RoomsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Response<Domain.Models.Room>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
    {
        var command = new CreateRoomCommand(
            request.Name,
            request.Number,
            Enum.Parse<RoomType>(request.RoomType),
            Enum.Parse<RoomStatus>(request.RoomStatus),
            request.PricePerNight);

        var response = await mediator.Send(command);
        return response.IsSuccessful
            ? Created("api/rooms", response.Data)
            : BadRequest(response.Message);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(Response<IEnumerable<Domain.Models.Room>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRooms()
    {
        var query = new GetRoomsQuery();
        var response = await mediator.Send(query);
        return response.IsSuccessful
            ? Ok(response.Data)
            : NotFound(response.Message);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Response<Domain.Models.Room>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoom([FromRoute]Guid id)
    {
        var query = new GetRoomDetailsQuery(id);
        var response = await mediator.Send(query);
        return response.IsSuccessful
            ? Ok(response.Data)
            : NotFound(response.Message);
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response<Domain.Models.Room>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomRequest request)
    {
        var command = new UpdateRoomCommand(
            id,
            request.Name,
            request.Number,
            Enum.Parse<RoomType>(request.RoomType),
            Enum.Parse<RoomStatus>(request.RoomStatus),
            request.PricePerNight);

        var response = await mediator.Send(command);
        return response.IsSuccessful
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Response<Domain.Models.Room>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRoom([FromRoute]Guid id)
    {
        var command = new DeleteRoomCommand(id);
        var response = await mediator.Send(command);
        return response.IsSuccessful
            ? Ok(response.Data)
            : BadRequest(response.Message);
    }
}
