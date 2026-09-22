using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class FixedCashAmountCalculator : IRebateCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedCashAmount;

    public bool TryCalculate(Rebate rebate, Product product, CalculateRebateRequest request, out decimal rebateAmount)
    {
        rebateAmount = 0m;

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount) ||
            rebate.Amount <= 0m)
        {
            return false;
        }

        rebateAmount = rebate.Amount;
        return true;
    }
}
