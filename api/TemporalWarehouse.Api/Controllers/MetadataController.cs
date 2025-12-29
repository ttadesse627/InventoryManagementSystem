


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TemporalWarehouse.Api.Contracts.ResponseDtos;
using TemporalWarehouse.Api.Models.Utilities;

namespace TemporalWarehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetadataController(IOptions<AppMetadata> options) : ControllerBase
{
    private readonly AppMetadata _metadata = options.Value;

    [HttpGet]
    public async Task<ActionResult<MetadataResponse>> GetMetadataAsync()
    {

        return Ok(new MetadataResponse
        {
            Api = new ApiInfoDto(
                _metadata.Api.Platform,
                _metadata.Api.Language,
                _metadata.Api.Framework),
            Database = new DatabaseInfoDto(
                _metadata.Database.Platform,
                _metadata.Database.Engine)
        });
    }
}