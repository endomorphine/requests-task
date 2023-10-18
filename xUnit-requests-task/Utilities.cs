using Microsoft.Extensions.Configuration;

namespace xUnit_requests_task
{
    public static class Utilities
    {
        public static IConfiguration GetTestConfiguration()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "TimeoutSettings:RequestTimeoutSeconds", "1" }, })
                .Build();
        }
    }
}
