using System;
using System.Collections.Generic;
using PricingService.Domain;
using Xunit;

namespace PricingService.Test.Domain;

public class TariffBehaviorTests
{
    private static Calculation Calculate(string product, string[] covers, Dictionary<string, object> answers)
        => new(product, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(5), covers, answers);

    [Fact]
    public void WorldTravelAppliesMarkupToEverySelectedCover()
    {
        var price = TariffFactory.Travel().CalculatePrice(Calculate("TRI", new[] { "C1", "C2", "C3" }, new()
        {
            ["NUM_OF_ADULTS"] = 1M, ["NUM_OF_CHILDREN"] = 1M, ["DESTINATION"] = "WORLD"
        }));

        Assert.Equal(159M, price.TotalPremium);
        Assert.Equal(51M, price.Covers["C1"].Price);
        Assert.Equal(78M, price.Covers["C2"].Price);
        Assert.Equal(30M, price.Covers["C3"].Price);
    }

    [Fact]
    public void HouseCombinesMarkupsAndRoundsEachCoverToCurrencyPrecision()
    {
        var price = TariffFactory.House().CalculatePrice(Calculate("HSI", new[] { "C1", "C2", "C3" }, new()
        {
            ["TYP"] = "APT", ["AREA"] = 95M, ["NUM_OF_CLAIM"] = 2M, ["FLOOD"] = "YES"
        }));

        Assert.Equal(222.66M, price.Covers["C1"].Price);
        Assert.Equal(44.53M, price.Covers["C2"].Price);
        Assert.Equal(56.25M, price.Covers["C3"].Price);
        Assert.Equal(323.44M, price.TotalPremium);
    }

    [Fact]
    public void FarmCombinesFloodAndClaimMarkups()
    {
        var price = TariffFactory.Farm().CalculatePrice(Calculate("FAI", new[] { "C1", "C2", "C3", "C4" }, new()
        {
            ["FLOOD"] = "YES", ["NUM_OF_CLAIM"] = 3M
        }));
        Assert.Equal(300M, price.TotalPremium);
    }

    [Fact]
    public void CarAppliesClaimMarkup()
    {
        var price = TariffFactory.Car().CalculatePrice(Calculate("CAR", new[] { "C1" }, new()
        {
            ["NUM_OF_CLAIM"] = 3M
        }));
        Assert.Equal(150M, price.TotalPremium);
    }

    [Fact]
    public void DuplicateCoverDoesNotChargeTheCustomerTwice()
    {
        var price = TariffFactory.Car().CalculatePrice(Calculate("CAR", new[] { "C1", "C1" }, new()
        {
            ["NUM_OF_CLAIM"] = 0M
        }));
        Assert.Single(price.Covers);
        Assert.Equal(100M, price.TotalPremium);
    }

    [Fact]
    public void UnknownCoverIsRejectedInsteadOfReturningAFreePremium()
    {
        Assert.Throws<ArgumentException>(() => TariffFactory.Car().CalculatePrice(
            Calculate("CAR", new[] { "UNKNOWN" }, new() { ["NUM_OF_CLAIM"] = 0M })));
    }
}
