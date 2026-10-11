using PolicyService.Api.Commands.Dtos;

namespace PolicyService.Api.Commands;

public class CreatePolicyCommand
{
    public string OfferNumber { get; set; } = string.Empty;
    public PersonDto PolicyHolder { get; set; } = new();
    public AddressDto PolicyHolderAddress { get; set; } = new();
}
