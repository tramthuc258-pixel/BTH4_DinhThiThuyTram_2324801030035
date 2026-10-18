namespace PricingService.Domain;

public sealed class Calculation
{
    public string ProductCode { get; }
    public DateTimeOffset PolicyFrom { get; }
    public DateTimeOffset PolicyTo { get; }
    public IReadOnlyList<string> SelectedCovers { get; }
    public IReadOnlyDictionary<string, object> Subject { get; }

    public Calculation(string productCode, DateTimeOffset policyFrom, DateTimeOffset policyTo,
        IEnumerable<string> selectedCovers, IDictionary<string, object> subject)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);
        ArgumentNullException.ThrowIfNull(selectedCovers);
        ArgumentNullException.ThrowIfNull(subject);
        if (policyTo < policyFrom)
            throw new ArgumentException("PolicyTo must not precede PolicyFrom.", nameof(policyTo));

        ProductCode = productCode;
        PolicyFrom = policyFrom;
        PolicyTo = policyTo;
        SelectedCovers = selectedCovers.Distinct(StringComparer.Ordinal).ToArray();
        Subject = new Dictionary<string, object>(subject, StringComparer.Ordinal);
    }
}
