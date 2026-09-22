using System;
using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IReadOnlyDictionary<IncentiveType, IRebateCalculator> _calculators;

    public RebateService(
        IRebateDataStore rebateDataStore,
        IProductDataStore productDataStore,
        IEnumerable<IRebateCalculator> calculators)
    {
        _rebateDataStore = rebateDataStore ?? throw new ArgumentNullException(nameof(rebateDataStore));
        _productDataStore = productDataStore ?? throw new ArgumentNullException(nameof(productDataStore));
        _calculators = (calculators ?? throw new ArgumentNullException(nameof(calculators)))
            .ToDictionary(calculator => calculator.IncentiveType);
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        if (request == null)
        {
            return Failed();
        }

        var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        if (rebate == null)
        {
            return Failed();
        }

        var product = _productDataStore.GetProduct(request.ProductIdentifier);
        if (product == null || !_calculators.TryGetValue(rebate.Incentive, out var calculator))
        {
            return Failed();
        }

        if (!calculator.TryCalculate(rebate, product, request, out var rebateAmount))
        {
            return Failed();
        }

        _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);

        return new CalculateRebateResult
        {
            Success = true,
            Amount = rebateAmount
        };
    }

    private static CalculateRebateResult Failed() => new();
}
