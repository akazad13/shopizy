 # Domain Model - Order

```csharp
class Order
{
    Order Create();
    void Update(Order Order);
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "customerId": "0000000-0000-0000-0000-000000000000",
    "totalAmount": 150,
    "discount": 20,
    "deliveryCharges": 10,
    "finalAmount": 140,
    "createdDateTime": "2024-01-01T00:00:00.000Z",
    "updatedDateTime": "2024-01-01T00:00:00.000Z",
    "paymentMethod": "Amex Credit Card",
    "transactionId": "13543423",
    "promoId": "0000000-0000-0000-0000-000000000000",
    "currency": "usd",
    "shippingAddress": {
        "id": "0000000-0000-0000-0000-000000000000",
        "name": "Apple"
    },
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
}
```