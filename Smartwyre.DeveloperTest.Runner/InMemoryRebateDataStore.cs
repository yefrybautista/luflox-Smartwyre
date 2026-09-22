using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

internal sealed class InMemoryRebateDataStore : IRebateDataStore
{
    private readonly IReadOnlyDictionary<string, Rebate> _rebates;

    public InMemoryRebateDataStore(IEnumerable<Rebate> rebates)
    {
        var rebatesByIdentifier = new Dictionary<string, Rebate>();
        foreach (var rebate in rebates)
        {
            rebatesByIdentifier[rebate.Identifier] = rebate;
        }

        _rebates = rebatesByIdentifier;
    }

    public Rebate GetRebate(string rebateIdentifier)
    {
        _rebates.TryGetValue(rebateIdentifier, out var rebate);
        return rebate;
    }

    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        LastCalculation = new RebateCalculation
        {
            RebateIdentifier = rebate.Identifier,
            IncentiveType = rebate.Incentive,
            Amount = rebateAmount
        };
    }

    public RebateCalculation LastCalculation { get; private set; }
}
