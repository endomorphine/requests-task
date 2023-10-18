using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using requests_task.Dto;
using requests_task.Entities;
using requests_task.Configuration;

namespace requests_task.Controllers;

[ApiController]
[Route("api/")]
public class ApiController : ControllerBase
{
    private readonly int _requestTimeoutSeconds;
    private static readonly ConcurrentDictionary<string, Request> PendingRequests = new();

    public ApiController(IOptions<TimeoutSettings> timeoutSettings)
    {
        _requestTimeoutSeconds = timeoutSettings.Value.RequestTimeoutSeconds;
    }

    [HttpPost("requests")]
    public async Task<IActionResult> Requests([FromBody] RequestsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var request = PendingRequests.GetOrAdd(dto.Resource, _ => new Request(dto.Resource, new CancellationTokenSource()));
        
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(_requestTimeoutSeconds), request.CancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            PendingRequests.Remove(dto.Resource, out _);
            return Ok(request.ToDto());
        }

        return Ok(ResponseDto.GetTimeoutDto(request.Resource));
    }

    [HttpPost("access")]
    public IActionResult Access([FromBody] AccessDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (PendingRequests.TryGetValue(dto.Resource, out var request))
        {
            request.Decision = dto.Decision;
            request.CancellationTokenSource.Cancel();
        }

        return Ok();
    }
}
