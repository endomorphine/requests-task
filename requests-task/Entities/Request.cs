using requests_task.Common;
using requests_task.Dto;

namespace requests_task.Entities;

public class Request
{
    public Request(string resource, CancellationTokenSource cts)
    {
        Resource = resource;
        CancellationTokenSource = cts;
    }

    public string Resource { get; set; }

    public Decision Decision { get; set; }

    public CancellationTokenSource CancellationTokenSource { get; set; }

    public ResponseDto ToDto()
    {
        return new ResponseDto(
            Resource,
            Decision == Decision.Deny ? DecisionMsg.DeniedMsg : DecisionMsg.GrantedMsg,
            Decision == Decision.Deny ? ReasonMsg.DeniedByUser : string.Empty);
    }
}