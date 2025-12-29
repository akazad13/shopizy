# Shopizy API

- [Shopizy API](#shopizy-api)
  - [Auth](#auth)
    - [Register](#register)
    - [Login](#login)
  - [Users](#users)
    - [Get User](#get-user)
    - [Update User](#update-user)
    - [Update Address](#update-address)
    - [Update Password](#update-password)
  - [Products](#products)
    - [List Products](#list-products)
    - [Get Product](#get-product)
    - [Create Product](#create-product)
    - [Update Product](#update-product)
    - [Delete Product](#delete-product)
    - [Add Product Image](#add-product-image)
    - [Delete Product Image](#delete-product-image)
  - [Categories](#categories)
    - [List Categories](#list-categories)
    - [Get Category Tree](#get-category-tree)
    - [Get Category](#get-category)
    - [Create Category](#create-category)
    - [Update Category](#update-category)
    - [Delete Category](#delete-category)
  - [Carts](#carts)
    - [Get Cart](#get-cart)
    - [Add Item](#add-item)
    - [Update Item Quantity](#update-item-quantity)
    - [Remove Item](#remove-item)
  - [Orders](#orders)
    - [List Orders](#list-orders)
    - [Get Order](#get-order)
    - [Create Order](#create-order)
    - [Cancel Order](#cancel-order)
  - [Payments](#payments)
    - [Process Payment](#process-payment)

## Auth

### Register

```http
POST {{host}}/api/v1.0/auth/register
```

#### Request

```json
{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "password": "Password123!"
}
```

#### Response

```json
{
    "success": true,
    "message": "Your account has been added. Please log in."
}
```

### Login

```http
POST {{host}}/api/v1.0/auth/login
```

#### Request

```json
{
    "email": "john.doe@example.com",
    "password": "Password123!"
}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "token": "eyJh..."
}
```

## Users

### Get User

```http
GET {{host}}/api/v1.0/users/{userId}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "profileImageUrl": "https://example.com/image.jpg",
    "phone": "+1234567890",
    "address": {
        "street": "123 Main St",
        "city": "New York",
        "state": "NY",
        "country": "USA",
        "zipCode": "10001"
    }
}
```

### Update User

```http
PUT {{host}}/api/v1.0/users/{userId}
```

#### Request

```json
{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890",
    "address": {
        "street": "123 Main St",
        "city": "New York",
        "state": "NY",
        "country": "USA",
        "zipCode": "10001"
    }
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully updated user."
}
```

### Update Address

```http
PATCH {{host}}/api/v1.0/users/{userId}/address
```

#### Request

```json
{
    "street": "456 Elm St",
    "city": "Los Angeles",
    "state": "CA",
    "country": "USA",
    "zipCode": "90001"
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully updated address."
}
```

### Update Password

```http
PATCH {{host}}/api/v1.0/users/{userId}/password
```

#### Request

```json
{
    "currentPassword": "OldPassword123!",
    "newPassword": "NewPassword123!",
    "confirmNewPassword": "NewPassword123!"
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully updated password."
}
```

## Products

### List Products

```http
GET {{host}}/api/v1.0/products?pageNumber=1&pageSize=10&name=Shirt&categoryId={categoryId}
```

#### Response

```json
[
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "T-Shirt",
        "shortDescription": "Cotton T-Shirt",
        "price": {
            "amount": 29.99,
            "currency": "USD"
        },
        "imageUrl": "https://example.com/tshirt.jpg"
    }
]
```

### Get Product

```http
GET {{host}}/api/v1.0/products/{productId}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "T-Shirt",
    "description": "High quality cotton t-shirt...",
    "price": {
        "amount": 29.99,
        "currency": "USD"
    },
    "images": [
        {
            "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "url": "https://example.com/tshirt.jpg"
        }
    ],
    "colors": ["Red", "Blue"],
    "sizes": ["S", "M", "L"]
}
```

### Create Product

```http
POST {{host}}/api/v1.0/users/{userId}/products
```

#### Request

```json
{
    "name": "New Product",
    "shortDescription": "Short desc",
    "description": "Full description",
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "unitPrice": 99.99,
    "currency": 1,
    "discount": 0,
    "sku": "SKU123",
    "brand": "BrandName",
    "colors": "Red,Blue",
    "sizes": "S,M,L",
    "tags": "New,Sale",
    "barcode": "123456789"
}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "New Product",
    ...
}
```

### Update Product

```http
PATCH {{host}}/api/v1.0/users/{userId}/products/{productId}
```

#### Request

```json
{
    "name": "Updated Product Name",
    "unitPrice": 89.99
    // ... other fields
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully updated product."
}
```

### Delete Product

```http
DELETE {{host}}/api/v1.0/users/{userId}/products/{productId}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully deleted product."
}
```

### Add Product Image

```http
POST {{host}}/api/v1.0/users/{userId}/products/{productId}/image
Content-Type: multipart/form-data
```

#### Request

Form data: `file` (Binary)

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "url": "https://..."
}
```

### Delete Product Image

```http
DELETE {{host}}/api/v1.0/users/{userId}/products/{productId}/image/{imageId}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully deleted product image."
}
```

## Categories

### List Categories

```http
GET {{host}}/api/v1.0/categories
```

#### Response

```json
[
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Electronics"
    }
]
```

### Get Category Tree

```http
GET {{host}}/api/v1.0/categories/tree
```

#### Response

```json
[
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Electronics",
        "children": [
            {
                "id": "...",
                "name": "Laptops"
            }
        ]
    }
]
```

### Create Category

```http
POST {{host}}/api/v1.0/users/{userId}/categories
```

#### Request

```json
{
    "name": "New Category",
    "parentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" // Optional
}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "New Category"
}
```

### Update Category

```http
PATCH {{host}}/api/v1.0/users/{userId}/categories/{categoryId}
```

#### Request

```json
{
    "name": "Updated Category Name",
    "parentId": null
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully updated category."
}
```

### Delete Category

```http
DELETE {{host}}/api/v1.0/users/{userId}/categories/{categoryId}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully deleted category."
}
```

## Carts

### Get Cart

```http
GET {{host}}/api/v1.0/users/{userId}/carts
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "items": [
        {
            "id": "...",
            "productId": "...",
            "productName": "...",
            "quantity": 1,
            "unitPrice": 10.00,
            "totalPrice": 10.00
        }
    ],
    "totalPrice": 10.00
}
```

### Add Item

```http
PATCH {{host}}/api/v1.0/users/{userId}/carts/{cartId}
```

#### Request

```json
{
    "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "color": "Red",
    "size": "M",
    "quantity": 1
}
```

#### Response

```json
{
    // Updated Cart object
}
```

### Update Item Quantity

```http
PATCH {{host}}/api/v1.0/users/{userId}/carts/{cartId}/items/{itemId}
```

#### Request

```json
{
    "quantity": 2
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully updated cart."
}
```

### Remove Item

```http
DELETE {{host}}/api/v1.0/users/{userId}/carts/{cartId}/items/{itemId}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully removed product from cart."
}
```

## Orders

### List Orders

```http
GET {{host}}/api/v1.0/users/{userId}/orders?status=Pending
```

#### Response

```json
[
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "orderStatus": "Pending",
        "totalPrice": 100.00,
        "createdOn": "2023-01-01T00:00:00Z"
    }
]
```

### Get Order

```http
GET {{host}}/api/v1.0/users/{userId}/orders/{orderId}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "orderItems": [...],
    "shippingAddress": {...},
    "totalPrice": 100.00
}
```

### Create Order

```http
POST {{host}}/api/v1.0/users/{userId}/orders
```

#### Request

```json
{
    "promoCode": "SAVE10",
    "deliveryMethod": 1,
    "deliveryCharge": {
        "amount": 5.00,
        "currency": "USD"
    },
    "shippingAddress": {
        "street": "123 Main St",
        "city": "New York",
        "state": "NY",
        "country": "USA",
        "zipCode": "10001"
    },
    "orderItems": [
        {
            "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "color": "Red",
            "size": "M",
            "quantity": 1
        }
    ]
}
```

#### Response

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    // ... order details
}
```

### Cancel Order

```http
PATCH {{host}}/api/v1.0/users/{userId}/orders/{orderId}/cancel
```

#### Request

```json
{
    "reason": "Changed mind"
}
```

#### Response

```json
{
    "success": true,
    "message": "Successfully canceled the order."
}
```

## Payments

### Process Payment

```http
POST {{host}}/api/v1.0/users/{userId}/payments
```

#### Request

```json
{
    "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "paymentMethod": "card", // or "cash"
    "amount": 100.00,
    "currency": "USD",
    "token": "tok_visa" // for card payments
}
```

#### Response

```json
{
    "success": true,
    "message": "Payment successfull collected."
}
```