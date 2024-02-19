# Domain Model - Category

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
    "parentCategoryId": "0000000-0000-0000-0000-000000000000"
}
```