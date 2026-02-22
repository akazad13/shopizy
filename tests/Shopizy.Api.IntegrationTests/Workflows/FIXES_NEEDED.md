# E2E Test Build Error Fixes

## Root Cause

The E2E workflow tests use anonymous types instead of the actual contract types defined in `Shopizy.Contracts`. This causes build errors.

## Quick Fix Guide

### Issue 1: Anonymous Types for Requests

**Problem**: Helper methods use anonymous types like:
```csharp
var createRequest = new { name, price, ... };
await HttpClient.PostAsJsonAsync(url, createRequest);
```

**Solution**: Use actual contract types:
```csharp
using Shopizy.Contracts.Product;

var createRequest = new CreateProductRequest(...);
await HttpClient.PostAsJsonAsync(url, createRequest);
```

### Issue 2: Address and Price Types

**Location**: Both are defined in `Shopizy.Contracts.Order` namespace

**Usage**:
```csharp
using Shopizy.Contracts.Order;

var address = new Address("Street", "City", "State", "Country", "Zip");
var price = new Price(100.00m, "USD");
```

### Issue 3: UserDetails vs UserResponse

**Correct Type**: `Shopizy.Contracts.User.UserDetails`

## Files Requiring Updates

All 5 workflow test files need updates. The fixes are straightforward but numerous.

## Recommended Approach

Given the scope of fixes needed (~140 errors), I recommend:

1. **Option A - Minimal Working Test**: Start with one simple test file
2. **Option B - Fix Helper Methods**: Update `BaseIntegrationTest.cs` to use proper types
3. **Option C - Incremental**: Fix one workflow file at a time

## Option A: Minimal Working Example (Recommended)

Create a single working E2E test to validate the approach:

```csharp
using System.Net;
using Shouldly;
using Xunit;
using Shopizy.Contracts.Product;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Order;

namespace Shopizy.Api.IntegrationTests.Workflows;

public class SimpleE2ETest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task SimpleProductFlow_CreateAndRetrieve_Works()
    {
        // Arrange
        var token = await AuthenticateAsNewUserAsync();
        var userId = GetUserIdFromToken(token);

        // Create category using actual contract
        var categoryRequest = new CreateCategoryRequest("Test Category", null);
        var categoryResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/categories",
            categoryRequest,
            TestContext.Current.CancellationToken);
        categoryResponse.EnsureSuccessStatusCode();
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>(
            TestContext.Current.CancellationToken);

        // Create product using actual contract
        var productRequest = new CreateProductRequest(
            "Test Product",
            "Short desc",
            "Long description",
            category!.Id,
            99.99m,
            1, // Currency enum
            0m, // Discount
            "SKU-001",
            "TestBrand",
            "Red,Blue",
            "M,L",
            "Tag1",
            "BARCODE123"
        );

        var productResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/products",
            productRequest,
            TestContext.Current.CancellationToken);
        productResponse.EnsureSuccessStatusCode();
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDetailResponse>(
            TestContext.Current.CancellationToken);

        // Verify
        product.ShouldNotBeNull();
        product.Name.ShouldBe("Test Product");
    }
}
```

## Next Steps

1. Create `SimpleE2ETest.cs` with the minimal example above
2. Build and run to verify it works
3. Use it as a template to fix the other workflow tests
4. Update helper methods in `BaseIntegrationTest.cs` to use proper contracts

## Contract Type Reference

```csharp
// Product
using Shopizy.Contracts.Product;
CreateProductRequest, UpdateProductRequest
ProductResponse, ProductDetailResponse

// Category  
using Shopizy.Contracts.Category;
CreateCategoryRequest, UpdateCategoryRequest
CategoryResponse

// Cart
using Shopizy.Contracts.Cart;
AddProductToCartRequest
CartResponse, CartItemResponse

// Order
using Shopizy.Contracts.Order;
CreateOrderRequest, CancelOrderRequest
OrderResponse, OrderDetailResponse, OrderItemResponse
Address, Price // Shared types

// User
using Shopizy.Contracts.User;
UpdateUserRequest, UpdatePasswordRequest, UpdateAddressRequest
UserDetails

// Auth
using Shopizy.Contracts.Authentication;
LoginRequest, RegisterRequest
AuthResponse
```
