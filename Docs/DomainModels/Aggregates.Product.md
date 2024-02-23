# Domain Model - Product

```csharp
class Product : AggregateRoot<Guid>
{
    Product Create();
    void Update(Product product);
    void UpdateSpecification(List<Specification> specifications);
    void UpdateCategory(CategoryId categoryId);
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "name": "iPhone 15 pro",
    "Description": "Latest iPhone 15 pro",
    "category": {
        "id": "0000000-0000-0000-0000-000000000000",
        "name": "Apple"
    },
    "createdDateTime": "2024-01-01T00:00:00.000Z",
    "updatedDateTime": "2024-01-01T00:00:00.000Z",
    "unitPrice": 960,
    "currency" : "usd",
    "discount": 30.00,
    "sku": "111111111111111",
    "Brand": "Apple",
    "Tags": "iphone, apple",
    "barcode": "asdlnqwezxcljqwelndfdsaf0u343lef9u234",
    "stockQuantity": 100,
    "averageRating": 4.6,
    "specifications": [
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "Released",
            "value": "2023-09-22T00:00:00.000Z"
        },
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "Display",
            "value": "6.1 inches, OLED,  Super Retina XDR, 120Hz, HDR support"
        },
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "Hardware",
            "value": "8GB RAM, 128GB ROM (not expandable), iOS (17.x)"
        }
    ],
    "breadCrumbs": [
        "Electronics",
        "Mobile",
        "Apple"
    ],
    "productImages" : [
        {
            "url" : "",
            "order" : ""
        },
        {
            "url" : "",
            "order" : ""
        }
    ]
}
```