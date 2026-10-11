namespace PolicyService.Api.Queries.Dtos;

public class PolicyDto
{
    public string Number { get; set; } = string.Empty;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string PolicyHolder { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public IList<string> Covers { get; set; } = new List<string>();
}
