namespace DashboardService.Api.Queries;

public class GetAgentsSalesQuery
{
    public string? ProductCode { get; set; }
    public DateTime SalesDateFrom { get; set; }
    public DateTime SalesDateTo { get; set; }
}
