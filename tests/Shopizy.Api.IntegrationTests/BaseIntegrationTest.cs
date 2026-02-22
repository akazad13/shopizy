using Xunit;
using MediatR;
using Shopizy.Infrastructure.Common.Persistence;
using System.Net.Http.Headers;
using Shopizy.Contracts.Authentication;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Users;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.Carts;
using System.Reflection;
using System.Net.Http.Json;

namespace Shopizy.Api.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly AppDbContext DbContext;
    protected readonly HttpClient HttpClient;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        HttpClient = factory.CreateClient();
    }

    /// <summary>
    /// Registers a new user and returns the registration response.
    /// </summary>
    protected async Task<HttpResponseMessage> RegisterUserAsync(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        var registerRequest = new RegisterRequest(firstName, lastName, email, password);
        return await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", registerRequest, TestContext.Current.CancellationToken);
    }

    /// <summary>
    /// Logs in a user and returns the JWT token.
    /// </summary>
    protected async Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var loginRequest = new LoginRequest(email, password);
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/login", loginRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse?.Token ?? throw new InvalidOperationException("Failed to get auth token");
    }

    /// <summary>
    /// Sets the Authorization header with the provided JWT token.
    /// </summary>
    protected void SetAuthToken(string token)
    {
        HttpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Clears the Authorization header.
    /// </summary>
    protected void ClearAuthToken()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Seeds an admin user if not exists and authenticates.
    /// </summary>
    protected async Task<(string Token, Guid UserId)> AuthenticateAsAdminAsync()
    {
        var adminId = Guid.Parse("E68D8E76-72A1-42ED-91D0-0ED0296D662E");
        var email = "admin@shopizy.com";
        var password = "Password123!";

        using (var scope = _scope.ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordManager = scope.ServiceProvider.GetRequiredService<Shopizy.Application.Common.Interfaces.Authentication.IPasswordManager>();

            var existingUser = (await dbContext.Users.FindAsync([UserId.Create(adminId)], TestContext.Current.CancellationToken));
            if (existingUser is null)
            {
                var hashedPassword = passwordManager.CreateHashString(password);
                var constructor = typeof(User).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(UserId), typeof(string), typeof(string), typeof(string), typeof(string), typeof(IList<PermissionId>) },
                    null);

                var user = (User)constructor!.Invoke([UserId.Create(adminId), "System", "Admin", email, hashedPassword, new List<PermissionId>()]);
                dbContext.Users.Add(user);
                dbContext.Carts.Add(Cart.Create(user.Id));
                await dbContext.SaveChangesAsync();
            }
        }

        var loginRequest = new LoginRequest(email, password);
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/login", loginRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        SetAuthToken(authResponse!.Token);

        return (authResponse.Token, authResponse.Id);
    }

    /// <summary>
    /// Registers a user, logs in, and sets the auth token in one call.
    /// </summary>
    protected async Task<(string Token, Guid UserId)> AuthenticateAsNewUserAsync(
        string firstName = "Test",
        string lastName = "User",
        string? email = null,
        string password = "Password123!")
    {
        email ??= $"{Guid.NewGuid().ToString()[..8]}@test.com";
        
        await RegisterUserAsync(firstName, lastName, email, password);
        var token = await LoginAndGetTokenAsync(email, password);
        SetAuthToken(token);
        
        var userId = GetUserIdFromToken(token);
        return (token, userId);
    }

    #region Product Management Helpers

    /// <summary>
    /// Creates a product via API and returns the product ID.
    /// </summary>
    protected async Task<Guid> CreateProductAsync(
        Guid userId,
        string name,
        decimal price,
        Guid categoryId,
        string sku = "TEST-SKU",
        int stockQuantity = 100)
    {
        var createRequest = new
        {
            name,
            shortDescription = $"Short description for {name}",
            description = $"Full description for {name}",
            categoryId,
            unitPrice = price,
            currency = 1, // USD
            discount = 0m,
            sku,
            brand = "TestBrand",
            colors = "Red,Blue",
            sizes = "S,M,L",
            tags = "Test",
            barcode = Guid.NewGuid().ToString()[..8]
        };

        var response = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{userId}/products", createRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Product.ProductDetailResponse>(TestContext.Current.CancellationToken);
        return product?.ProductId ?? throw new InvalidOperationException("Failed to create product");
    }

    /// <summary>
    /// Gets a product by ID via API.
    /// </summary>
    protected async Task<Shopizy.Contracts.Product.ProductDetailResponse> GetProductAsync(Guid productId)
    {
        var response = await HttpClient.GetAsync($"/api/v1.0/products/{productId}", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Product.ProductDetailResponse>(TestContext.Current.CancellationToken);
        return product ?? throw new InvalidOperationException("Failed to get product");
    }

    /// <summary>
    /// Searches for products via API.
    /// </summary>
    protected async Task<List<Shopizy.Contracts.Product.ProductResponse>> SearchProductsAsync(
        string? name = null,
        Guid? categoryId = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var queryParams = new List<string> { $"pageNumber={pageNumber}", $"pageSize={pageSize}" };
        if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={name}");
        if (categoryId.HasValue) queryParams.Add($"categoryId={categoryId}");

        var query = string.Join("&", queryParams);
        var response = await HttpClient.GetAsync($"/api/v1.0/products?{query}", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<Shopizy.Contracts.Product.ProductResponse>>(TestContext.Current.CancellationToken);
        return products ?? new List<Shopizy.Contracts.Product.ProductResponse>();
    }

    #endregion

    #region Category Management Helpers

    /// <summary>
    /// Creates a category via API and returns the category ID.
    /// </summary>
    protected async Task<Guid> CreateCategoryAsync(Guid userId, string name, Guid? parentId = null)
    {
        var createRequest = new { name, parentId };
        var response = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{userId}/categories", createRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var category = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Category.CategoryResponse>(TestContext.Current.CancellationToken);
        return category?.Id ?? throw new InvalidOperationException("Failed to create category");
    }

    /// <summary>
    /// Lists all categories via API.
    /// </summary>
    protected async Task<List<Shopizy.Contracts.Category.CategoryResponse>> ListCategoriesAsync()
    {
        var response = await HttpClient.GetAsync("/api/v1.0/categories", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var categories = await response.Content.ReadFromJsonAsync<List<Shopizy.Contracts.Category.CategoryResponse>>(TestContext.Current.CancellationToken);
        return categories ?? new List<Shopizy.Contracts.Category.CategoryResponse>();
    }

    #endregion

    #region Cart Management Helpers

    /// <summary>
    /// Gets the user's cart via API.
    /// </summary>
    protected async Task<Shopizy.Contracts.Cart.CartResponse> GetCartAsync(Guid userId)
    {
        var response = await HttpClient.GetAsync($"/api/v1.0/users/{userId}/carts", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var cart = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Cart.CartResponse>(TestContext.Current.CancellationToken);
        return cart ?? throw new InvalidOperationException("Failed to get cart");
    }

    /// <summary>
    /// Adds an item to the cart via API.
    /// </summary>
    protected async Task<Guid> AddToCartAsync(
        Guid userId,
        Guid cartId,
        Guid productId,
        int quantity = 1,
        string color = "Red",
        string size = "M")
    {
        var addRequest = new { productId, color, size, quantity };
        var response = await HttpClient.PatchAsJsonAsync($"/api/v1.0/users/{userId}/carts/{cartId}", addRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var cart = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Cart.CartResponse>(TestContext.Current.CancellationToken);
        var addedItem = cart?.CartItems.FirstOrDefault(i => i.ProductId == productId);
        return addedItem?.CartItemId ?? throw new InvalidOperationException("Failed to add item to cart");
    }

    /// <summary>
    /// Updates cart item quantity via API.
    /// </summary>
    protected async Task UpdateCartItemQuantityAsync(Guid userId, Guid cartId, Guid itemId, int quantity)
    {
        var updateRequest = new { quantity };
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/carts/{cartId}/items/{itemId}",
            updateRequest,
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Removes an item from the cart via API.
    /// </summary>
    protected async Task RemoveFromCartAsync(Guid userId, Guid cartId, Guid itemId)
    {
        var response = await HttpClient.DeleteAsync(
            $"/api/v1.0/users/{userId}/carts/{cartId}/items/{itemId}",
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region Order Management Helpers

    /// <summary>
    /// Places an order via API and returns the order ID.
    /// </summary>
    protected async Task<Guid> PlaceOrderAsync(
        Guid userId,
        IEnumerable<object> orderItems,
        object? shippingAddress = null)
    {
        shippingAddress ??= new
        {
            street = "123 Test St",
            city = "Test City",
            state = "TS",
            country = "Test Country",
            zipCode = "12345"
        };

        var createOrderRequest = new
        {
            promoCode = "",
            deliveryMethod = 1,
            deliveryCharge = new { amount = 5.0, currency = "USD" },
            shippingAddress,
            orderItems
        };

        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/orders",
            createOrderRequest,
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Order.OrderDetailResponse>(TestContext.Current.CancellationToken);
        return order?.OrderId ?? throw new InvalidOperationException("Failed to place order");
    }

    /// <summary>
    /// Gets an order by ID via API.
    /// </summary>
    protected async Task<Shopizy.Contracts.Order.OrderDetailResponse> GetOrderAsync(Guid userId, Guid orderId)
    {
        var response = await HttpClient.GetAsync($"/api/v1.0/users/{userId}/orders/{orderId}", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<Shopizy.Contracts.Order.OrderDetailResponse>(TestContext.Current.CancellationToken);
        return order ?? throw new InvalidOperationException("Failed to get order");
    }

    /// <summary>
    /// Lists orders for a user via API.
    /// </summary>
    protected async Task<List<Shopizy.Contracts.Order.OrderResponse>> ListOrdersAsync(Guid userId, string? status = null)
    {
        var url = $"/api/v1.0/users/{userId}/orders";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";

        var response = await HttpClient.GetAsync(url, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<Shopizy.Contracts.Order.OrderResponse>>(TestContext.Current.CancellationToken);
        return orders ?? new List<Shopizy.Contracts.Order.OrderResponse>();
    }

    #endregion

    #region Payment Helpers

    /// <summary>
    /// Processes payment for an order via API.
    /// </summary>
    protected async Task ProcessPaymentAsync(
        Guid userId,
        Guid orderId,
        decimal amount,
        string paymentMethod = "card",
        string currency = "USD")
    {
        var paymentRequest = new
        {
            orderId,
            paymentMethod,
            amount,
            currency,
            token = "tok_visa"
        };

        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/payments",
            paymentRequest,
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
    }

    #endregion

    #region User Helpers

    /// <summary>
    /// Extracts user ID from authentication token.
    /// </summary>
    protected Guid GetUserIdFromToken(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id");
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new InvalidOperationException($"Failed to extract user ID from token. Claims present: {string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}={c.Value}"))}");
        }

        return userId;
    }

    #endregion

    public void Dispose()
    {
        _scope.Dispose();
        DbContext.Dispose();
        HttpClient.Dispose();
    }
}
