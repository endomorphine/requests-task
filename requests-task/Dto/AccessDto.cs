using System.ComponentModel.DataAnnotations;
//using requests_task.Entities;

namespace requests_task.Dto
{
    public sealed class AccessDto
    {
        [Required]
        public string Resource { get; set; }

        [Required]
        public string Decision { get; set; }
    }
}
