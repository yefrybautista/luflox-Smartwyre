using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public interface IRebateCalculator
{
    IncentiveType IncentiveType { get; }

    bool TryCalculate(
        Rebate rebate,
        Product product,
        CalculateRebateRequest request,
        out decimal rebateAmount);
}
