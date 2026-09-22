using System;
using System.Globalization;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

internal static class Program
{
    private const string ProductIdentifier = "product-1";

    private static int Main(string[] args)
    {
        if (args.Length != 3 ||
            !decimal.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var volume) ||
            volume <= 0m)
        {
            PrintUsage();
            return 1;
        }

        var rebateDataStore = new InMemoryRebateDataStore(new[]
        {
            new Rebate
            {
                Identifier = "rebate-cash",
                Incentive = IncentiveType.FixedCashAmount,
                Amount = 25m
            },
            new Rebate
            {
                Identifier = "rebate-rate",
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = 0.10m
            },
            new Rebate
            {
                Identifier = "rebate-uom",
                Incentive = IncentiveType.AmountPerUom,
                Amount = 3m
            }
        });
        var productDataStore = new InMemoryProductDataStore(new[]
        {
            new Product
            {
                Identifier = ProductIdentifier,
                Price = 20m,
                SupportedIncentives =
                    SupportedIncentiveType.FixedCashAmount |
                    SupportedIncentiveType.FixedRateRebate |
                    SupportedIncentiveType.AmountPerUom
            }
        });
        var service = new RebateService(
            rebateDataStore,
            productDataStore,
            new IRebateCalculator[]
            {
                new FixedCashAmountCalculator(),
                new FixedRateRebateCalculator(),
                new AmountPerUomCalculator()
            });

        var result = service.Calculate(new CalculateRebateRequest
        {
            RebateIdentifier = args[0],
            ProductIdentifier = args[1],
            Volume = volume
        });

        if (!result.Success)
        {
            Console.Error.WriteLine("The rebate could not be calculated. Check the supplied identifiers and volume.");
            return 2;
        }

        Console.WriteLine($"Rebate calculated and stored successfully: {result.Amount.ToString("0.00", CultureInfo.InvariantCulture)}");
        return 0;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: dotnet run --project Smartwyre.DeveloperTest.Runner -- <rebate> <product> <volume>");
        Console.WriteLine();
        Console.WriteLine("Available rebates: rebate-cash, rebate-rate, rebate-uom");
        Console.WriteLine($"Available product: {ProductIdentifier}");
        Console.WriteLine("Example: dotnet run --project Smartwyre.DeveloperTest.Runner -- rebate-rate product-1 5");
    }
}
