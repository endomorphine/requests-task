using requests_task.Dto;

namespace requests_task.Entities;

public class Request
{
    public string Resource { get; set; }

    public string Decision { get; set; }

    public ResponseDto ToDto(string reason) => new(Resource, Decision, reason);
}
