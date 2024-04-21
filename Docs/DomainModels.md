# Shopizy Domain Models

- [Shopizy Domain Models](#Shopizy-domain-models)
  - [User Aggregate](#user-aggregate)
  - [Customer Aggregate](#customer-aggregate)
  - [Category Aggregate](#category-aggregate)
  - [Product Aggregate](#product-aggregate)
  - [Product Review Aggregate](#product-review-aggregate)
  - [Order Aggregate](#order-aggregate)
  - [Promo Code Aggregate](#promo-code-aggregate)
  - [Payment Aggregate](#payment-aggregate)

## User Aggregate

```csharp
class User : AggregateRoot<Guid>
{
    User Create();
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "firstName": "John",
    "lastName": "Doe",
    "profileImageUrl" : "",
    "phone": "+3584573969860",
    "password": "xxxxxxxxxxxx",
    "address": {
        "street" : "",
        "city": "",
        "state" : "",
        "country" : "",
        "zipCode": ""
    },
    "CreatedOn": "2024-01-01T00:00:00.000Z",
    "ModifiedOn": "2024-01-01T00:00:00.000Z"
}
```

## Customer Aggregate

```csharp
class Customer : AggregateRoot<Guid>
{
    Customer Create();
}
```

```json
{
    "orders": [
        {
            "id" : "0000000-0000-0000-0000-000000000000",
            "finalAmount" : {
                "amount": 150,
                "currency": "usd"
            },
            "orderStatus" : ""
        },
        {
            "id" : "0000000-0000-0000-0000-000000000000",
            "finalAmount" : {
                "amount": 120,
                "currency": "usd"
            },
            "orderStatus" : ""
        },
    ],
    "reviews": [
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "productId": "0000000-0000-0000-0000-000000000000",
            "rating": 4.7,
            "comment": "Very good product.",
            "createdOn": "2024-01-01T00:00:00.000Z",
            "modifiedOn": "2024-01-01T00:00:00.000Z",
        },
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "productId": "0000000-0000-0000-0000-000000000000",
            "rating": 4.5,
            "comment": "Another good product.",
            "createdOn": "2024-01-01T00:00:00.000Z",
            "modifiedOn": "2024-01-01T00:00:00.000Z",
        }
    ]

}
```

## Category Aggregate

```csharp
class Category
{
    Category Create();
    void Update(Category category);
    void UpdateParentId(CategoryId id);
}
```

```json
{
    "id": "0000000-0000-0000-0000-000000000000",
    "name": "Mobile",
    "parentId": "0000000-0000-0000-0000-000000000000"
}
```

## Product Aggregate

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
    "description": "Latest iPhone 15 pro",
    "category": {
        "id": "0000000-0000-0000-0000-000000000000",
        "name": "Apple"
    },
    "createdOn": "2024-01-01T00:00:00.000Z",
    "modifiedOn": "2024-01-01T00:00:00.000Z",
    "unitPrice": {
        "amount" : 960.00,
        "currency" : "usd"
    },
    "discount": 30.00, // %
    "sku": "111111111111111",
    "brand": "Apple",
    "tags": "iphone, apple",
    "barcode": "asdlnqwezxcljqwelndfdsaf0u343lef9u234",
    "stockQuantity": 100,
    "averageRating": {
        "value" : 4.6,
        "numRatings": 10
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
    "breadCrumbs": "Electronics|Mobile|Apple",
    "productImages" : [
        {
            "url" : "",
            "seq" : 1
        },
        {
            "url" : "",
            "seq" : 2
        }
    ]
}
```

## Product Review Aggregate

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

## Order Aggregate

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
    "totalAmount": {
        "amount": 150,
        "currency": "usd"
    },
    "totalDiscount": {
        "amount": 20,
        "currency": "usd"
    },
    "deliveryCharges": {
        "amount": 40,
        "currency": "usd"
    },
    "finalAmount": {
        "amount": 170,
        "currency": "usd"
    },
    "orderStatus" : "Submitted",
    "promoCode": "WELCOME20",
    "createdOn": "2024-01-01T00:00:00.000Z",
    "modifiedOn": "2024-01-01T00:00:00.000Z",
    "shippingAddress": {
        "line" : "17292 McFadden Ave",
        "city": "Tustin",
        "state" : "California",
        "country" : "USA",
        "zipCode": "92780"
    },
    "orderItems": [
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "iPhone 15 pro",
            "pictureUrl": "",
            "unitPrice": {
                "amount": 100,
                "currency": "usd"
            },
            "quantity": 4,
            "discount": 15.00   // %
        },
        {
            "id": "0000000-0000-0000-0000-000000000000",
            "name": "iPhone 15 pro max",
            "pictureUrl": "",
            "unitPrice": {
                "amount": 50,
                "currency": "usd"
            },
            "quantity": 2,
            "discount": 10.00 // %
        }
    ],
    "paymentStatus" : "Payed"
}
```

## Promo Code Aggregate

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

## Payment Aggregate

```csharp
class Payment : AggregateRoot<Guid>
{
    Payment Create();
    
    PaymentId Id;
    string PaymentMethod;
    string TransactionId;
    PaymentStatus PaymentStatus;
    Price Total;
    Address BillingAddress;
    DateTime CreatedOn;
    DateTime ModifiedOn;
}

public enum PaymentStatus
{
    Pending = 1,
    Cancelled = 2,
    Payed = 3
}

```

```json
{
    "id" : "0000000-0000-0000-0000-000000000000",
    "orderId": "0000000-0000-0000-0000-000000000000",
    "customerId": "0000000-0000-0000-0000-000000000000",
    "paymentMethod": "Credit Card",
    "transactionId": "13543423",
    "paymentStatus" : "Payed",
    "total" : {
        "amount": 150,
        "currency": "usd"
    },
    "billingAddress": {
        "line" : "17292 McFadden Ave",
        "city": "Tustin",
        "state" : "California",
        "country" : "USA",
        "zipCode": "92780"
    },
    "createdOn": "2024-01-01T00:00:00.000Z",
    "modifiedOn": "2024-01-01T00:00:00.000Z",
}
```