using System.ComponentModel.DataAnnotations;

namespace requests_task.Dto;
public sealed class RequestsDto
{
    [Required]
    public string Resource { get; set; }
}