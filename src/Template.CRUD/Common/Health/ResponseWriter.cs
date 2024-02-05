using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Template.Api.Common.Health;

public static class ResponseWriter
{
    public static async Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var healthReportEntry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(healthReportEntry.Key);
                jsonWriter.WriteString("status",
                    healthReportEntry.Value.Status.ToString());
                jsonWriter.WriteString("description",
                    healthReportEntry.Value.Description);
                if (healthReportEntry.Value.Exception != null)
                {
                    jsonWriter.WriteString("exception", healthReportEntry.Value.Exception.Message);
                    if (healthReportEntry.Value.Exception is HttpConnectionHealthException httpConnEx)
                    {
                        jsonWriter.WriteString("reasonphrase", httpConnEx.Response?.ReasonPhrase);
                        jsonWriter.WriteString("statuscode", httpConnEx.Response?.StatusCode.ToString());
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        jsonWriter.WriteString("content", await httpConnEx.Response?.Content.ReadAsStringAsync());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                }
                jsonWriter.WriteStartObject("data");

                foreach (var item in healthReportEntry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value?.GetType() ?? typeof(object));
                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        await context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
