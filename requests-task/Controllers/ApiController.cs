using Microsoft.AspNetCore.Mvc;
using requests_task.Dto;
using requests_task.Entities;

namespace requests_task.Controllers;

[ApiController]
[Route("api/")]
public class ApiController : ControllerBase
{
    private static Request? _sharedRequest;
    private static CancellationTokenSource? _cancellationTokenSource;
    private const string DEFAULT_RESOURCE = "my-res";

    [HttpPost("requests")]
    public async Task<IActionResult> Requests([FromBody] RequestsDto dto)
    {
        if (!ModelState.IsValid || dto.Resource != DEFAULT_RESOURCE)
            return BadRequest();

        _sharedRequest = new Request { Resource = dto.Resource };
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(20), token);
            _sharedRequest.Decision = "Denied";
            return Ok(_sharedRequest.ToDto("Timeout expired"));
        }
        catch (TaskCanceledException)
        {
            return Ok(_sharedRequest.ToDto(_sharedRequest.Decision == "Denied" ? "Denied by user" : "Granted"));
        }
    }

    [HttpPost("access")]
    public IActionResult Access([FromBody] AccessDto dto)
    {
        if (!ModelState.IsValid || dto.Resource != DEFAULT_RESOURCE)
            return BadRequest();

        if (_sharedRequest != null && _sharedRequest.Resource == dto.Resource)
        {
            _sharedRequest.Decision = dto.Decision;
            _cancellationTokenSource.Cancel();
        }

        return Ok();
    }
}
