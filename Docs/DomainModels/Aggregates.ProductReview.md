# Domain Model - ProductReview

```csharp
class ProductReview : AggregateRoot<Guid>
{
    ProductReview Create();
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "productId": "0000000-0000-0000-0000-000000000000",
    "CustomerId": "0000000-0000-0000-0000-000000000000",
    "rating": 4.7,
    "comment": "Very good product.",
    "createdOn": "2024-01-01T00:00:00.000Z",
    "modifiedOn": "2024-01-01T00:00:00.000Z",
}
```