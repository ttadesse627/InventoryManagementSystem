


namespace TemporalWarehouse.Api.Contracts.ResponseDtos;

public record MetadataResponse
{
    public required ApiInfoDto Api { get; init; }
    public required DatabaseInfoDto Database { get; init; }
}

public record ApiInfoDto(string Platform, string Language, string Framework);
public record DatabaseInfoDto(string Platform, string Engine);