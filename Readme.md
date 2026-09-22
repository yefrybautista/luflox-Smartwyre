# Smartwyre Developer Test Instructions

You have been selected to complete our candidate coding exercise. Please follow the directions in this readme.

Clone, **DO NOT FORK**, this repository to your account on the online Git resource of your choosing (GitHub, BitBucket, GitLab, etc.). Your solution should retain previous commit history and you should utilize best practices for committing your changes to the repository.

You are welcome to use whatever tools you normally would when coding — including documentation, libraries, frameworks, or AI tools (such as ChatGPT or Copilot).

However, it is important that you fully understand your solution. As part of the interview process, we will review your code with you in detail. You should be able to:

- Explain the design choices you made.
- Walk us through how your solution works.
- Make modifications or extensions to your code during the review.

Please note: if your submission appears to have been generated entirely by an AI agent or another third party, without your own understanding or contribution, it will not meet our evaluation criteria.

# The Exercise

In the 'RebateService.cs' file you will find a method for calculating a rebate. At a high level the steps for calculating a rebate are:

 1. Lookup the rebate that the request is being made against.
 2. Lookup the product that the request is being made against.
 2. Check that the rebate and request are valid to calculate the incentive type rebate.
 3. Store the rebate calculation.

What we'd like you to do is refactor the code with the following things in mind:

 - Adherence to SOLID principles
 - Testability
 - Readability
 - Currently there are 3 known incentive types. In the future the business will want to add many more incentive types. Your solution should make it easy for developers to add new incentive types in the future.

We’d also like you to 
 - Add some unit tests to the Smartwyre.DeveloperTest.Tests project to show how you would test the code that you’ve produced 
 - Run the RebateService from the Smartwyre.DeveloperTest.Runner console application accepting inputs (either via command line arguments or via prompts is fine)

The only specific "rules" are:

- The solution must build
- All tests must pass

You are free to use any frameworks/NuGet packages that you see fit. You should plan to spend around 1 hour completing the exercise.

Feel free to use code comments to describe your changes. You are also welcome to update this readme with any important details for us to consider.

Once you have completed the exercise either ensure your repository is available publicly or contact the hiring manager to set up a private share.

## Solution notes

`RebateService` is now responsible only for orchestrating the calculation: it retrieves the rebate and product, selects a calculator, and stores a successful result. Data access is injected through `IRebateDataStore` and `IProductDataStore`, which keeps the service independent from concrete storage and makes it straightforward to unit test.

Each incentive type has its own `IRebateCalculator` implementation. Adding another incentive requires a new calculator and registration at the composition root, without changing `RebateService`. Calculation inputs must be positive and the product must support the requested incentive.

The runner uses small in-memory data stores because the data stores supplied with the exercise contain placeholder database implementations. It can be executed with:

```shell
dotnet run --project Smartwyre.DeveloperTest.Runner -- rebate-rate product-1 5
```

Available sample rebates are `rebate-cash`, `rebate-rate`, and `rebate-uom`. The sample product is `product-1`.

Build and run the tests with:

```shell
dotnet build Smartwyre.DeveloperTest.sln
dotnet test Smartwyre.DeveloperTest.sln
```
