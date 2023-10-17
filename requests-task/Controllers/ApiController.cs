using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using requests_task.Dto;
using requests_task.Entities;

namespace requests_task.Controllers;

[ApiController]
[Route("api/")]
public class ApiController : ControllerBase
{
    private const int TIMEOUT_DELAY = 20;

    private static readonly ConcurrentDictionary<string, Request> PendingRequests = new();

    [HttpPost("requests")]
    public async Task<IActionResult> Requests([FromBody] RequestsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var request = PendingRequests.GetOrAdd(dto.Resource, _ => new Request(dto.Resource, new CancellationTokenSource()));
        
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(TIMEOUT_DELAY), request.CancellationTokenSource.Token);
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
