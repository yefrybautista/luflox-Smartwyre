using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class FixedRateRebateCalculator : IRebateCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedRateRebate;

    public bool TryCalculate(Rebate rebate, Product product, CalculateRebateRequest request, out decimal rebateAmount)
    {
        rebateAmount = 0m;

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate) ||
            rebate.Percentage <= 0m ||
            product.Price <= 0m ||
            request.Volume <= 0m)
        {
            return false;
        }

        rebateAmount = product.Price * rebate.Percentage * request.Volume;
        return true;
    }
}
