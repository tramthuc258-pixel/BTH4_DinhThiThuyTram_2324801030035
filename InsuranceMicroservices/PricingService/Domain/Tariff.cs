using DynamicExpresso;

namespace PricingService.Domain;

public sealed record CoverPrice(decimal Price);

public sealed class Price
{
    public Dictionary<string, CoverPrice> Covers { get; } = new(StringComparer.Ordinal);
    public decimal TotalPremium => Covers.Values.Sum(cover => cover.Price);
}

public sealed class Tariff
{
    public string Code { get; }
    public BasePremiumRules BasePremiumRules { get; set; } = new();
    public DiscountMarkupRules DiscountMarkupRules { get; set; } = new();

    public Tariff(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    public Price CalculatePrice(Calculation calculation)
    {
        ArgumentNullException.ThrowIfNull(calculation);
        if (calculation.ProductCode != Code)
            throw new ArgumentException("The calculation product does not match this tariff.");

        var interpreter = new Interpreter();
        foreach (var answer in calculation.Subject)
            interpreter.SetVariable(answer.Key, answer.Value);

        var multiplier = DiscountMarkupRules.CalculateMultiplier(interpreter);
        var result = new Price();
        foreach (var coverCode in calculation.SelectedCovers)
        {
            var premium = BasePremiumRules.Calculate(coverCode, interpreter) * multiplier;
            if (premium < 0)
                throw new InvalidOperationException("A price rule produced a negative premium.");
            result.Covers.Add(coverCode, new CoverPrice(decimal.Round(premium, 2, MidpointRounding.AwayFromZero)));
        }
        return result;
    }
}
