namespace Template.Api.Extensions.Http;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage message, JsonSerializerOptions serializerOptions) => JsonSerializer.Deserialize<T>(await message.Content.ReadAsStringAsync(), serializerOptions)!;
}
