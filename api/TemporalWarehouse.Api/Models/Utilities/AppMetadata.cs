

namespace TemporalWarehouse.Api.Models.Utilities;

public sealed class AppMetadata
{
    public ApiInfo Api { get; set; } = new();
    public DatabaseInfo Database { get; set; } = new();
}

public sealed class ApiInfo
{
    public string Platform { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public string Framework { get; init; } = string.Empty;
}
public sealed class DatabaseInfo
{
    public string Platform { get; init; } = string.Empty;
    public string Engine { get; init; } = string.Empty;
}