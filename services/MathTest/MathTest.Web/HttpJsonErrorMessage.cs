using System.Text.Json;

namespace MathTest.Web;

internal static class HttpJsonErrorMessage
{
    public static string FromResponseBody(string? jsonBody)
    {
        if (string.IsNullOrWhiteSpace(jsonBody))
        {
            return "Something went wrong. Please try again.";
        }

        try
        {
            using JsonDocument doc = JsonDocument.Parse(jsonBody);

            if (doc.RootElement.ValueKind == JsonValueKind.Object
                && doc.RootElement.TryGetProperty("message", out JsonElement msg)
                && msg.ValueKind == JsonValueKind.String)
            {
                string? text = msg.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
        }
        catch (JsonException)
        {
            // fall through
        }

        return jsonBody.Trim();
    }
}
