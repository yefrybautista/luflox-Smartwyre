using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class AmountPerUomCalculator : IRebateCalculator
{
    public IncentiveType IncentiveType => IncentiveType.AmountPerUom;

    public bool TryCalculate(Rebate rebate, Product product, CalculateRebateRequest request, out decimal rebateAmount)
    {
        rebateAmount = 0m;

        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom) ||
            rebate.Amount <= 0m ||
            request.Volume <= 0m)
        {
            return false;
        }

        rebateAmount = rebate.Amount * request.Volume;
        return true;
    }
}
