using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.User;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Users;

public class UserAddressesEndpointTests : BaseIntegrationTest
{
    public UserAddressesEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task AddressEndpoints_FullLifecycle_Succeeds()
    {
        // 1. Authenticate user
        var (token, userId) = await AuthenticateAsNewUserAsync(
            "AddressUser",
            "Test",
            "addressuser@example.com"
        );

        // 2. Add User Address
        var addRequest = new AddUserAddressRequest(
            Street: "123 Main St",
            City: "Metropolis",
            State: "NY",
            Country: "USA",
            ZipCode: "10001",
            IsDefault: true
        );

        var addResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/addresses",
            addRequest,
            TestContext.Current.CancellationToken
        );

        addResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var addedAddress = await addResponse.Content.ReadFromJsonAsync<UserAddressResponse>(
            TestContext.Current.CancellationToken
        );
        addedAddress.ShouldNotBeNull();
        addedAddress.Street.ShouldBe("123 Main St");
        addedAddress.IsDefault.ShouldBeTrue();

        var addressId = addedAddress.AddressId;

        // 3. Get User Addresses
        var getResponse = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/addresses",
            TestContext.Current.CancellationToken
        );

        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var addresses = await getResponse.Content.ReadFromJsonAsync<List<UserAddressResponse>>(
            TestContext.Current.CancellationToken
        );
        addresses.ShouldNotBeNull();
        addresses.ShouldContain(a => a.AddressId == addressId);

        // 4. Update User Address
        var updateRequest = new UpdateUserAddressRequest(
            Street: "456 Updated St",
            City: "Metropolis",
            State: "NY",
            Country: "USA",
            ZipCode: "10002"
        );

        var updateResponse = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/addresses/{addressId}",
            updateRequest,
            TestContext.Current.CancellationToken
        );

        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updatedAddress = await updateResponse.Content.ReadFromJsonAsync<UserAddressResponse>(
            TestContext.Current.CancellationToken
        );
        updatedAddress.ShouldNotBeNull();
        updatedAddress.Street.ShouldBe("456 Updated St");

        // 5. Set Default Address
        var setDefaultResponse = await HttpClient.PatchAsync(
            $"/api/v1.0/users/{userId}/addresses/{addressId}/set-default",
            null,
            TestContext.Current.CancellationToken
        );

        setDefaultResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // 6. Delete User Address
        var deleteResponse = await HttpClient.DeleteAsync(
            $"/api/v1.0/users/{userId}/addresses/{addressId}",
            TestContext.Current.CancellationToken
        );

        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetUserAddresses_WhenUnauthorizedUserTriesAccess_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync(
            "OwnerUser",
            "Test",
            "owner@example.com"
        );
        var (_, user2Id) = await AuthenticateAsNewUserAsync(
            "OtherUser",
            "Test",
            "other@example.com"
        );

        // Act - Currently authenticated as user2, attempting to read user1's addresses
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{user1Id}/addresses",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
