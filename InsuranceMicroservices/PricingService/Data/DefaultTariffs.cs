using PricingService.Domain;

namespace PricingService.Data;

public static class DefaultTariffs
{
    public static IEnumerable<Tariff> Create()
    {
        var travel = new Tariff("TRI");
        travel.BasePremiumRules.AddBasePriceRule("C1", null, "NUM_OF_ADULTS * (DESTINATION == \"EUR\" ? 26M : 34M)");
        travel.BasePremiumRules.AddBasePriceRule("C2", null, "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 26M");
        travel.BasePremiumRules.AddBasePriceRule("C3", null, "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 10M");
        travel.DiscountMarkupRules.AddPercentMarkup("DESTINATION == \"WORLD\"", 1.5M);
        yield return travel;

        var house = new Tariff("HSI");
        house.BasePremiumRules.AddBasePriceRule("C1", "TYP == \"APT\"", "AREA * 1.25M");
        house.BasePremiumRules.AddBasePriceRule("C1", "TYP == \"HOUSE\"", "AREA * 1.50M");
        house.BasePremiumRules.AddBasePriceRule("C2", "TYP == \"APT\"", "AREA * 0.25M");
        house.BasePremiumRules.AddBasePriceRule("C2", "TYP == \"HOUSE\"", "AREA * 0.45M");
        house.BasePremiumRules.AddBasePriceRule("C3", null, "30M");
        house.BasePremiumRules.AddBasePriceRule("C4", null, "50M");
        house.DiscountMarkupRules.AddPercentMarkup("FLOOD == \"YES\"", 1.5M);
        house.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 1", 1.25M);
        yield return house;

        var farm = new Tariff("FAI");
        for (var i = 1; i <= 4; i++)
            farm.BasePremiumRules.AddBasePriceRule($"C{i}", null, $"{i * 10}M");
        farm.DiscountMarkupRules.AddPercentMarkup("FLOOD == \"YES\"", 1.5M);
        farm.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 2", 2M);
        yield return farm;

        var car = new Tariff("CAR");
        car.BasePremiumRules.AddBasePriceRule("C1", null, "100M");
        car.DiscountMarkupRules.AddPercentMarkup("NUM_OF_CLAIM > 2", 1.5M);
        yield return car;
    }
}
