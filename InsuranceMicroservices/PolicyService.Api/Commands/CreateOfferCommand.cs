using PolicyService.Api.Commands.Dtos;

namespace PolicyService.Api.Commands;

public class CreateOfferCommand
{
    public string ProductCode { get; set; } = string.Empty;
    public DateTime PolicyFrom { get; set; }
    public DateTime PolicyTo { get; set; }
    public IList<string> SelectedCovers { get; set; } = new List<string>();
    public IList<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
}
