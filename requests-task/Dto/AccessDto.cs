using System.ComponentModel.DataAnnotations;
using requests_task.Entities;

namespace requests_task.Dto;

public sealed class AccessDto
{
    [Required]
    public string Resource { get; set; }

    [Required]
    [EnumDataType(typeof(Decision))]
    public Decision Decision { get; set; }
}