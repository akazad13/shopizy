 # Domain Model - Order

```csharp
class PromoCode : Entity
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
    "description": "",
    "discount": 20,
    "isPerchantage": true,
    "isActive": true
}
```