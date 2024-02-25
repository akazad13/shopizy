 # Domain Model - Promo Code

```csharp
class PromoCode : AggregateRoot<Guid>
{
    Giud Id;
    string Code;
    string Description;
    decimal Discount;
    bool IsPerchantage;
    bool IsActive;
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "code": "WELCOME20",
    "description": "This is a welcome promo code for starter.",
    "discount": 20,
    "isPerchantage": true,
    "isActive": true,
    "createdOn": "2024-01-01T00:00:00.000Z",
    "modifiedOn": "2024-01-01T00:00:00.000Z",
}
```