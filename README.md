# Shopizy - Online Store Management API

![Shopizy Banner](https://github.com/akazad13/shopizy/assets/16265339/0db251b3-e701-4a6d-9964-3701ffe9166d)

Shopizy is a robust and scalable Online Store Management API built with .NET 10, following the principles of Clean Architecture. It provides a comprehensive set of endpoints for managing products, categories, carts, orders, and users, designed to support modern e-commerce applications.

## 🚀 About The Project

Shopizy aims to provide a solid foundation for e-commerce platforms. It separates concerns into distinct layers (Domain, Application, Infrastructure, API) to ensure maintainability, testability, and scalability.

Key features include:
-   **Product Management**: Create, update, delete, and search products.
-   **Category Management**: Organize products into hierarchical categories.
-   **Shopping Cart**: Manage user carts and items.
-   **Order Processing**: Place and track orders.
-   **User Management**: User authentication and profile management.
-   **Clean Architecture**: Decoupled layers for better code organization.

## 🛠️ Built With

*   [ASP.NET Core 10](https://dotnet.microsoft.com/en-us/apps/aspnet) - The web framework used.
*   [MediatR](https://github.com/jbogard/MediatR) - Simple, unambitious mediator implementation in .NET.
*   [Mapster](https://github.com/MapsterMapper/Mapster) - A fast, fun and stimulating object to object mapper.
*   [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Object-Relational Mapper (ORM).
*   [Redis](https://redis.io/) - In-memory data structure store, used for caching.
*   [Swagger](https://swagger.io/) - API Documentation.
*   [ErrorOr](https://github.com/amantinband/error-or) - A simple, fluent discrimination union for error handling.

## 🏁 Getting Started

Follow these steps to get a local copy up and running.

### Prerequisites

*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [Redis](https://redis.io/download) (Running locally or via Docker)
*   SQL Server (or configured database provider)

### Installation

1.  **Clone the repository**
    ```sh
    git clone https://github.com/akazad13/shopizy.git
    ```
2.  **Navigate to the project directory**
    ```sh
    cd shopizy
    ```
3.  **Restore dependencies**
    ```sh
    dotnet restore
    ```
4.  **Configure Database**
    Update the connection string in `src/Shopizy.Api/appsettings.json` (or `appsettings.Development.json`).
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=ShopizyDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```
5.  **Run Migrations**
    The application is configured to apply migrations on startup (`DbMigrationsHelper`), but you can also run them manually:
    ```sh
    cd src/Shopizy.Api
    dotnet ef database update
    ```
6.  **Run the API**
    ```sh
    dotnet run
    ```

## 🔌 Usage

Once the API is running, you can explore the endpoints using Swagger UI.

*   **Swagger UI**: `https://localhost:7066/swagger/index.html` (Port may vary)

### Key Endpoints

*   `GET /api/v1.0/products`: Search and filter products.
*   `POST /api/v1.0/users/{userId}/cart`: Add items to cart.
*   `POST /api/v1.0/users/{userId}/orders`: Place an order.

## 📚 Documentation

For more detailed information, please refer to:

*   **[API Documentation](docs/Api.md)**: Comprehensive details on all API endpoints, requests, and responses.
*   **[Domain Models](docs/Domain.md)**: In-depth explanation of the domain aggregates and entities.

## 🏗️ Architecture

The solution follows **Clean Architecture** principles:

*   **Shopizy.Domain**: Contains enterprise logic and types (Entities, Value Objects, Enums). No dependencies.
*   **Shopizy.Application**: Contains business logic and use cases. Depends on Domain.
*   **Shopizy.Infrastructure**: Implements interfaces defined in Application (Data access, External services). Depends on Application.
*   **Shopizy.Api**: The entry point (Controllers, Middleware). Depends on Application and Infrastructure.
*   **Shopizy.Contracts**: Shared DTOs (Data Transfer Objects).

## 🧪 Running Tests

To run the automated tests for this system, execute the following command in the root directory:

```sh
dotnet test
```

This will run all unit tests across the Application, Domain, and Infrastructure layers.

## 🤝 Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

## ✍️ Authors

*   **Ak Azad** - *Initial work* - [akazad13](https://github.com/akazad13)

## 🙏 Acknowledgments

*   Clean Architecture templates and resources.
*   Open source libraries used in this project.
