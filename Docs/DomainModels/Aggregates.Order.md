 # Domain Model - Order

```csharp
class Order : AggregateRoot<Guid>
{
    Order Create();
    void AddOrderItem();
    void Update(Order Order);
    void SetAwaitingValidationStatus();
    void SetStockConfirmedStatus();
    void SetPaidStatus();
    void SetShippedStatus();
    void SetCancelledStatus();
    void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems);
    void AddOrderStartedDomainEvent(string userId, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration);
    void StatusChangeException(OrderStatus orderStatusToChange);
    decimal GetTotal();
}

class Address : ValueObject
{
    Address Create();
    IEnumerable<object> GetEqualityComponents();
    // yield return Street;
}

class OrderItem : Entity
{
    OrderItem Create();
    decimal GetCurrentDiscount();
    int GetUnits();
    decimal GetUnitPrice();
    void AddUnits(int units);
}

public enum OrderStatus{
    Submitted = 1,
    AwaitingValidation = 2,
    StockConfirmed = 3,
    Paid = 4,
    Shipped = 5,
    Cancelled = 6
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
        "street" : "",
        "city": "",
        "state" : "",
        "country" : "",
        "zipCode": ""
    },
    "orderStatus" : "",
    "orderItems": [
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "iPhone 15 pro",
            "pictureUrl": "",
            "unitPrice": "",
            "discount": ""
        },
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "iPhone 15 pro max",
            "pictureUrl": "",
            "unitPrice": "",
            "discount": ""
        }
    ],
    "bill" : {
        "id" : "0000000-0000-0000-0000-000000000000",
        "paymentType" : "",
        "billingAddress": {
            "street" : "",
            "city": "",
            "state" : "",
            "country" : "",
            "zipCode": ""
        },
        "billingStatus" : "",
        "createdDateTime": "2024-01-01T00:00:00.000Z",
        "updatedDateTime": "2024-01-01T00:00:00.000Z",
    }
}
```