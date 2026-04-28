using System.Net;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Brands;

public class BrandTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateBrand_AsAdmin_ReturnsOkWithBrand()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new CreateBrandRequest(
            $"Brand_{Guid.NewGuid().ToString()[..8]}",
            $"https://example.com/{Guid.NewGuid():N}.png",
            "Kenya"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/brands",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var brand = await response.Content.ReadFromJsonAsync<BrandResponse>(
            TestContext.Current.CancellationToken
        );
        brand.ShouldNotBeNull();
        brand.Id.ShouldNotBe(Guid.Empty);
        brand.Name.ShouldBe(request.Name);
        brand.LogoUrl.ShouldBe(request.LogoUrl);
        brand.Country.ShouldBe(request.Country);
    }

    [Fact]
    public async Task CreateBrand_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("Regular", "User");
        var request = new CreateBrandRequest(
            $"Brand_{Guid.NewGuid().ToString()[..8]}",
            $"https://example.com/{Guid.NewGuid():N}.png",
            "Kenya"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/brands",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetBrand_Anonymous_ReturnsBrand()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/brands",
            new CreateBrandRequest(
                $"Brand_{Guid.NewGuid().ToString()[..8]}",
                $"https://example.com/{Guid.NewGuid():N}.png",
                "Kenya"
            ),
            TestContext.Current.CancellationToken
        );
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<BrandResponse>(
            TestContext.Current.CancellationToken
        );
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/brands/{created!.Id}",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var brand = await response.Content.ReadFromJsonAsync<BrandResponse>(
            TestContext.Current.CancellationToken
        );
        brand.ShouldNotBeNull();
        brand.Id.ShouldBe(created.Id);
        brand.Name.ShouldBe(created.Name);
        brand.LogoUrl.ShouldBe(created.LogoUrl);
        brand.Country.ShouldBe(created.Country);
    }

    [Fact]
    public async Task ListBrands_Anonymous_ReturnsCreatedBrand()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = new CreateBrandRequest(
            $"Brand_{Guid.NewGuid().ToString()[..8]}",
            $"https://example.com/{Guid.NewGuid():N}.png",
            "Kenya"
        );
        await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/brands",
            request,
            TestContext.Current.CancellationToken
        );
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/brands",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var brands = await response.Content.ReadFromJsonAsync<List<BrandResponse>>(
            TestContext.Current.CancellationToken
        );
        brands.ShouldNotBeNull();
        brands.ShouldContain(brand => brand.Name == request.Name);
        brands.ShouldContain(brand =>
            brand.LogoUrl == request.LogoUrl && brand.Country == request.Country
        );
    }

    [Fact]
    public async Task UpdateBrand_AsAdmin_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/brands",
            new CreateBrandRequest(
                $"OldBrand_{Guid.NewGuid().ToString()[..8]}",
                $"https://example.com/{Guid.NewGuid():N}.png",
                "Kenya"
            ),
            TestContext.Current.CancellationToken
        );
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<BrandResponse>(
            TestContext.Current.CancellationToken
        );
        var updateRequest = new UpdateBrandRequest(
            $"NewBrand_{Guid.NewGuid().ToString()[..8]}",
            $"https://example.com/{Guid.NewGuid():N}.png",
            "Uganda"
        );

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/brands/{created!.Id}",
            updateRequest,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResult>(
            TestContext.Current.CancellationToken
        );
        result.ShouldNotBeNull();

        var getResponse = await HttpClient.GetAsync(
            $"/api/v1.0/brands/{created.Id}",
            TestContext.Current.CancellationToken
        );
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updated = await getResponse.Content.ReadFromJsonAsync<BrandResponse>(
            TestContext.Current.CancellationToken
        );
        updated.ShouldNotBeNull();
        updated.Name.ShouldBe(updateRequest.Name);
        updated.LogoUrl.ShouldBe(updateRequest.LogoUrl);
        updated.Country.ShouldBe(updateRequest.Country);
    }

    [Fact]
    public async Task DeleteBrand_AsAdmin_RemovesBrand()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/brands",
            new CreateBrandRequest(
                $"DeleteBrand_{Guid.NewGuid().ToString()[..8]}",
                $"https://example.com/{Guid.NewGuid():N}.png",
                "Kenya"
            ),
            TestContext.Current.CancellationToken
        );
        createResponse.EnsureSuccessStatusCode();

        var created = await createResponse.Content.ReadFromJsonAsync<BrandResponse>(
            TestContext.Current.CancellationToken
        );

        // Act
        var response = await HttpClient.DeleteAsync(
            $"/api/v1.0/admin/brands/{created!.Id}",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var getResponse = await HttpClient.GetAsync(
            $"/api/v1.0/brands/{created.Id}",
            TestContext.Current.CancellationToken
        );
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
