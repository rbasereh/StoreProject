using Newtonsoft.Json;

namespace TP.Domain.Common;

public class ApiResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; } = true;
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
}
public class ApiResponse<T> : ApiResponse
{
    [JsonProperty("data")]
    public T? Data { get; set; }
}

public class ApiRequest
{
    public string Parameter { get; set; }
    // Add more properties as needed
}
