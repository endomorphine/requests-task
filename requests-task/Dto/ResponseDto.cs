namespace requests_task.Dto
{
    public sealed class ResponseDto
    {
        public ResponseDto(string resource, string decision, string reason)
        {
            Resource = resource;
            Decision = decision;
            Reason = reason;
        }

        public string Resource { get; set; }

        public string Decision { get; set; }

        public string Reason { get; set; }
    }
}
