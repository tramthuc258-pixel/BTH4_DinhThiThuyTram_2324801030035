using DynamicExpresso;

namespace PricingService.Domain;

public sealed record BasePriceRule(string CoverCode, string? Condition, string Expression);
public sealed record PercentMarkupRule(string Condition, decimal Multiplier);

public sealed class BasePremiumRules
{
    public List<BasePriceRule> Rules { get; set; } = new();

    public void AddBasePriceRule(string coverCode, string? condition, string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(coverCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);
        Rules.Add(new BasePriceRule(coverCode, condition, expression));
    }

    internal decimal Calculate(string coverCode, Interpreter interpreter)
    {
        var applicable = Rules.Where(rule => rule.CoverCode == coverCode &&
            (string.IsNullOrWhiteSpace(rule.Condition) || interpreter.Eval<bool>(rule.Condition))).ToArray();
        if (applicable.Length == 0)
            throw new ArgumentException($"No matching price rule for cover '{coverCode}'.");
        if (applicable.Length > 1)
            throw new InvalidOperationException($"Multiple price rules match cover '{coverCode}'.");

        return interpreter.Eval<decimal>(applicable[0].Expression);
    }
}

public sealed class DiscountMarkupRules
{
    public List<PercentMarkupRule> Rules { get; set; } = new();

    public void AddPercentMarkup(string condition, decimal multiplier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(condition);
        if (multiplier < 0)
            throw new ArgumentOutOfRangeException(nameof(multiplier));
        Rules.Add(new PercentMarkupRule(condition, multiplier));
    }

    internal decimal CalculateMultiplier(Interpreter interpreter) => Rules
        .Where(rule => interpreter.Eval<bool>(rule.Condition))
        .Aggregate(1M, (multiplier, rule) => multiplier * rule.Multiplier);
}
