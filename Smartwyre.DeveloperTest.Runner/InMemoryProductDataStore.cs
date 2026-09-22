using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

internal sealed class InMemoryProductDataStore : IProductDataStore
{
    private readonly IReadOnlyDictionary<string, Product> _products;

    public InMemoryProductDataStore(IEnumerable<Product> products)
    {
        var productsByIdentifier = new Dictionary<string, Product>();
        foreach (var product in products)
        {
            productsByIdentifier[product.Identifier] = product;
        }

        _products = productsByIdentifier;
    }

    public Product GetProduct(string productIdentifier)
    {
        _products.TryGetValue(productIdentifier, out var product);
        return product;
    }
}
