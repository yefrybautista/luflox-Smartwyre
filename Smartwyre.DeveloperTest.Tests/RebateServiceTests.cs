using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    public static TheoryData<IncentiveType, decimal, decimal, decimal, decimal, decimal> SuccessfulCalculations =>
        new()
        {
            { IncentiveType.FixedCashAmount, 25m, 0m, 0m, 0m, 25m },
            { IncentiveType.FixedRateRebate, 0m, 0.10m, 20m, 5m, 10m },
            { IncentiveType.AmountPerUom, 3m, 0m, 0m, 4m, 12m }
        };

    [Theory]
    [MemberData(nameof(SuccessfulCalculations))]
    public void Calculate_StoresExpectedAmount_WhenRequestIsValid(
        IncentiveType incentive,
        decimal rebateAmount,
        decimal percentage,
        decimal productPrice,
        decimal volume,
        decimal expectedAmount)
    {
        var rebate = new Rebate
        {
            Identifier = "rebate-1",
            Incentive = incentive,
            Amount = rebateAmount,
            Percentage = percentage
        };
        var product = new Product
        {
            Identifier = "product-1",
            Price = productPrice,
            SupportedIncentives = ToSupportedIncentive(incentive)
        };
        var rebateStore = new RebateDataStoreStub { Rebate = rebate };
        var service = CreateService(rebateStore, new ProductDataStoreStub { Product = product });

        var result = service.Calculate(new CalculateRebateRequest
        {
            RebateIdentifier = rebate.Identifier,
            ProductIdentifier = product.Identifier,
            Volume = volume
        });

        Assert.True(result.Success);
        Assert.Equal(expectedAmount, result.Amount);
        Assert.Equal(1, rebateStore.StoreCallCount);
        Assert.Same(rebate, rebateStore.StoredRebate);
        Assert.Equal(expectedAmount, rebateStore.StoredAmount);
    }

    [Fact]
    public void Calculate_ReturnsFailure_WhenRebateDoesNotExist()
    {
        var rebateStore = new RebateDataStoreStub();
        var service = CreateService(rebateStore, new ProductDataStoreStub());

        var result = service.Calculate(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal(0, rebateStore.StoreCallCount);
    }

    [Fact]
    public void Calculate_ReturnsFailure_WhenProductDoesNotExist()
    {
        var rebateStore = new RebateDataStoreStub { Rebate = ValidFixedCashRebate() };
        var service = CreateService(rebateStore, new ProductDataStoreStub());

        var result = service.Calculate(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal(0, rebateStore.StoreCallCount);
    }

    [Fact]
    public void Calculate_ReturnsFailure_WhenProductDoesNotSupportIncentive()
    {
        var rebateStore = new RebateDataStoreStub { Rebate = ValidFixedCashRebate() };
        var productStore = new ProductDataStoreStub
        {
            Product = new Product { SupportedIncentives = SupportedIncentiveType.None }
        };
        var service = CreateService(rebateStore, productStore);

        var result = service.Calculate(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal(0, rebateStore.StoreCallCount);
    }

    [Fact]
    public void Calculate_ReturnsFailure_WhenCalculationInputsAreNotPositive()
    {
        var rebateStore = new RebateDataStoreStub
        {
            Rebate = new Rebate
            {
                Incentive = IncentiveType.AmountPerUom,
                Amount = 2m
            }
        };
        var productStore = new ProductDataStoreStub
        {
            Product = new Product
            {
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            }
        };
        var service = CreateService(rebateStore, productStore);
        var request = ValidRequest();
        request.Volume = 0m;

        var result = service.Calculate(request);

        Assert.False(result.Success);
        Assert.Equal(0, rebateStore.StoreCallCount);
    }

    [Fact]
    public void Calculate_ReturnsFailure_WhenCalculatorIsNotRegistered()
    {
        var rebateStore = new RebateDataStoreStub { Rebate = ValidFixedCashRebate() };
        var productStore = new ProductDataStoreStub
        {
            Product = new Product
            {
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            }
        };
        var service = new RebateService(rebateStore, productStore, new List<IRebateCalculator>());

        var result = service.Calculate(ValidRequest());

        Assert.False(result.Success);
        Assert.Equal(0, rebateStore.StoreCallCount);
    }

    private static RebateService CreateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore) =>
        new(
            rebateDataStore,
            productDataStore,
            new IRebateCalculator[]
            {
                new FixedCashAmountCalculator(),
                new FixedRateRebateCalculator(),
                new AmountPerUomCalculator()
            });

    private static CalculateRebateRequest ValidRequest() => new()
    {
        RebateIdentifier = "rebate-1",
        ProductIdentifier = "product-1",
        Volume = 1m
    };

    private static Rebate ValidFixedCashRebate() => new()
    {
        Identifier = "rebate-1",
        Incentive = IncentiveType.FixedCashAmount,
        Amount = 10m
    };

    private static SupportedIncentiveType ToSupportedIncentive(IncentiveType incentive) =>
        incentive switch
        {
            IncentiveType.FixedCashAmount => SupportedIncentiveType.FixedCashAmount,
            IncentiveType.FixedRateRebate => SupportedIncentiveType.FixedRateRebate,
            IncentiveType.AmountPerUom => SupportedIncentiveType.AmountPerUom,
            _ => SupportedIncentiveType.None
        };

    private sealed class RebateDataStoreStub : IRebateDataStore
    {
        public Rebate Rebate { get; set; }

        public int StoreCallCount { get; private set; }

        public Rebate StoredRebate { get; private set; }

        public decimal StoredAmount { get; private set; }

        public Rebate GetRebate(string rebateIdentifier) => Rebate;

        public void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
        {
            StoreCallCount++;
            StoredRebate = rebate;
            StoredAmount = rebateAmount;
        }
    }

    private sealed class ProductDataStoreStub : IProductDataStore
    {
        public Product Product { get; set; }

        public Product GetProduct(string productIdentifier) => Product;
    }
}
