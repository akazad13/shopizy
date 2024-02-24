# Domain Model - User

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
    "phone": "+3584573969860",
    "password": "xxxxxxxxxxxx",
    "CreatedOn": "2024-01-01T00:00:00.000Z",
    "ModifiedOn": "2024-01-01T00:00:00.000Z"
}
```