# Domain Model - User

```csharp
class Customer : AggregateRoot<Guid>
{
    Customer Create();
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "userId": "0000000-0000-0000-0000-000000000000",
    "profileImageUrl" : "",
    "createdOn": "2024-01-01T00:00:00.000Z",
    "modifiedOn": "2024-01-01T00:00:00.000Z",
    "address": {
        "street" : "",
        "city": "",
        "state" : "",
        "country" : "",
        "zipCode": ""
    },
    "orders": [
        {
            "id" : "0000000-0000-0000-0000-000000000000",
            "finalAmount" : 140,
            "currency": "usd",
            "orderStatus" : ""
        },
        {
            "id" : "0000000-0000-0000-0000-000000000000",
            "finalAmount" : 120,
            "currency": "usd",
            "orderStatus" : ""
        },
    ]
}
```