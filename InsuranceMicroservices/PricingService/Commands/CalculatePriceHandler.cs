using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PricingService.Api.Commands;
using PricingService.Domain;

namespace PricingService.Commands;

public sealed class CalculatePriceHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResult>
{
    private readonly IDataStore dataStore;

    public CalculatePriceHandler(IDataStore dataStore)
    {
        this.dataStore = dataStore;
    }

    public async Task<CalculatePriceResult> Handle(CalculatePriceCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        await new CalculatePriceCommandValidator().ValidateAndThrowAsync(request, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        var answers = new Dictionary<string, object>(StringComparer.Ordinal);
        // The validator above rejects missing ProductCode, SelectedCovers and Answers.
        foreach (var answer in request.Answers!)
        {
            if (answer is null || string.IsNullOrWhiteSpace(answer.QuestionCode) ||
                answer.GetAnswer() is not { } value || !answers.TryAdd(answer.QuestionCode, value))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(request.Answers), "Answers must have unique question codes and non-null values.")
                });
            }
        }

        var tariff = await dataStore.Tariffs[request.ProductCode!];
        var calculation = new Calculation(request.ProductCode!, request.PolicyFrom, request.PolicyTo,
            request.SelectedCovers!, answers);
        var price = tariff.CalculatePrice(calculation);
        return new CalculatePriceResult
        {
            TotalPrice = price.TotalPremium,
            CoverPrices = price.Covers.ToDictionary(cover => cover.Key, cover => cover.Value.Price)
        };
    }
}
