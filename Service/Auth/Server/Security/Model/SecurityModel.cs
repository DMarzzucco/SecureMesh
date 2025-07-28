using System;
using System.Text.Json.Serialization;

namespace Auth.Server.Security.Model;

public class IpInfoResponse
{
    [JsonPropertyName("city")]
    public string? City { get; set; }
}
public class SessionModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Ip { get; set; }
    public required string UserAgent { get; set; }
    public required string Location { get; set; }
    public bool IsActive { get; set; }
}
