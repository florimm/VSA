
namespace Template.Api.Features.Users.V1;

public class UserService : IUserService
{
    private readonly HttpClient _salesforceService;
    private readonly JsonSerializerOptions _serializerOptions;

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _salesforceService = httpClientFactory.CreateClient("Salesforce");
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }

    public async Task<string?> GetProjectsByUserAsync(string email)
    {
        var res = await _salesforceService.GetAsync($"/user/projects/{email}");
        if (res.StatusCode == HttpStatusCode.OK)
        {
            return await res.DeserializeAsync<string>(_serializerOptions);
        }
        return null;
    }
}
