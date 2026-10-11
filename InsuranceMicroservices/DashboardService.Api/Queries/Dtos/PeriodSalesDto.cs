namespace DashboardService.Api.Queries.Dtos;

public class PeriodSalesDto
{
    public string Period { get; set; } = string.Empty;
    public DateTime PeriodDate { get; set; }
    public SalesDto Sales { get; set; } = new();
}
