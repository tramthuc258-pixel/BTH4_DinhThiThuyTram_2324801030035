using PolicyService.Api.Queries.Dtos;

namespace PolicyService.Api.Queries;

public class GetPolicyDetailsQueryResult
{
    public PolicyDto Policy { get; set; } = new();
}
