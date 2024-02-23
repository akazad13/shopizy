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
}

class OrderItem : Entity
{
    OrderItemId Id;
    string Name;
    string PictureUrl;
    decimal UnitPrice;
    int Quantity;
    decimal Discount;
    OrderItem Create();
    decimal GetCurrentDiscount();
    int GetUnits();
    decimal GetUnitPrice();
    void AddUnits(int units);
}

class Bill : Entity
{
    BillId Id;
    string PaymentMethod;
    string TransactionId;
    string BillingStatus;
    decimal Total;
    Address BillingAddress;
    DateTime CreatedDateTime;
    DateTime UpdatedDateTime;
    Bill Create();
}

public enum OrderStatus
{
    Submitted = 1,
    AwaitingValidation = 2,
    StockConfirmed = 3,
    Paid = 4,
    Shipped = 5,
    Cancelled = 6
}
public enum Currency
{
    usd = 0,
    bdt = 1,
    euro = 2
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "customerId": "0000000-0000-0000-0000-000000000000",
    "totalAmount": 150,
    "totalDiscount": 20,
    "deliveryCharges": 10,
    "finalAmount": 140,
    "currency": "usd",
    "orderStatus" : "",
    "promoCode": "WELCOME20",
    "createdDateTime": "2024-01-01T00:00:00.000Z",
    "updatedDateTime": "2024-01-01T00:00:00.000Z",
    "shippingAddress": {
        "line" : "",
        "city": "",
        "state" : "",
        "country" : "",
        "zipCode": ""
    },
    "orderItems": [
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "iPhone 15 pro",
            "pictureUrl": "",
            "unitPrice": "",
            "quantity": 4,
            "discount": ""
        },
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "iPhone 15 pro max",
            "pictureUrl": "",
            "unitPrice": "",
            "quantity": 2,
            "discount": ""
        }
    ],
    "bill" : {
        "id" : "0000000-0000-0000-0000-000000000000",
        "paymentMethod": "Credit Card",
        "transactionId": "13543423",
        "billingAddress": {
            "line" : "",
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