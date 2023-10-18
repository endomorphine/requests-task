using Microsoft.AspNetCore.Mvc.Testing;
using requests_task;
using System.Net;
using System.Text;
using System.Text.Json;
using requests_task.Common;
using requests_task.Dto;
using requests_task.Entities;

namespace xUnit_requests_task;

public class Tests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string EXAMPLE_RESOURCE = "example_resource";
    private readonly WebApplicationFactory<Program> _factory;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Tests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostRequestsThenAccess_ValidRequest_ReturnsOk()
    {
        var client = _factory.CreateClient();

        HttpResponseMessage? requestsResponse = null;
        HttpResponseMessage? accessResponse = null;
        string? requestsJsonResponse = null;
        await Task.WhenAll(Task.Run(async () =>
        {
            var requestDto = new RequestsDto { Resource = EXAMPLE_RESOURCE };
            var json = JsonSerializer.Serialize(requestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            requestsResponse = await client.PostAsync("/api/requests", content);
            requestsJsonResponse = await requestsResponse.Content.ReadAsStringAsync();

        }), Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            var accessDto = new AccessDto { Resource = EXAMPLE_RESOURCE, Decision = Decision.Deny };
            var json = JsonSerializer.Serialize(accessDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            accessResponse = await client.PostAsync("/api/access", content);
        }));

        // access test
        Assert.Equal(HttpStatusCode.OK, accessResponse?.StatusCode);

        // requests tests
        Assert.Equal(HttpStatusCode.OK, requestsResponse?.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(requestsJsonResponse), "requests response should not be empty");

        var responseObject = JsonSerializer.Deserialize<ResponseDto>(requestsJsonResponse, _serializerOptions);
        Assert.False(responseObject == null, "requests response should deserialize correctly");

        Assert.Equal(EXAMPLE_RESOURCE, responseObject.Resource);
        Assert.Equal(DecisionMsg.DeniedMsg, responseObject.Decision);
        Assert.Equal(ReasonMsg.DeniedByUser, responseObject.Reason);
    }

    [Fact]
    public async Task PostRequests_ValidRequest_ReturnsTimeout()
    {
        var client = _factory.CreateClient();
        var requestDto = new RequestsDto { Resource = EXAMPLE_RESOURCE };
        var json = JsonSerializer.Serialize(requestDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/requests", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(jsonResponse), "response should not be empty");

        var responseObject = JsonSerializer.Deserialize<ResponseDto>(jsonResponse, _serializerOptions);
        Assert.False(responseObject == null, "response should deserialize correctly");

        Assert.Equal(EXAMPLE_RESOURCE, responseObject.Resource);
        Assert.Equal(DecisionMsg.DeniedMsg, responseObject.Decision);
        Assert.Equal(ReasonMsg.Timeout, responseObject.Reason);
    }

    [Fact]
    public async Task PostAccess_ValidRequest_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var accessDto = new AccessDto { Resource = "example_resource", Decision = Decision.Grant };
        var json = JsonSerializer.Serialize(accessDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/access", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostAccess_InvalidRequest_Returns400()
    {
        var client = _factory.CreateClient();
        var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/access", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

}