# ecommerce_api

A .NET 10 e-commerce backend providing RESTful APIs for products, stores, users, orders, and delivery.

## Quick summary

- Framework: .NET 10 (see `global.json`)
- Database: PostgreSQL (EF Core Npgsql)
- API docs: Swagger available at `/swagger` when the app is running

## Key features

- Full CRUD for products, categories, subcategories, stores, banners, and users
- Authentication: JWT Bearer authentication with refresh-token support
- Global exception handling via `Exceptions/GlobalExceptionHandler.cs` and Problem Details
- Response shaping: `Filter/CustomResultFilter.cs` wraps results into a standard `Result` object
- Authorization helper: `Filter/GetUserIdFromUserClaims` attribute extracts user id from JWT claims
- Rate limiting: App-wide rate limiting configured; controllers use the `userAccessLimit` policy (`app.UseRateLimiter()` / `RequireRateLimiting("userAccessLimit")`)
- Caching: In-memory and distributed caching integrations (Redis / Postgres distributed cache packages referenced)

### Filters & result shaping

- `Filter/CustomResultFilter.cs`: global result filter that wraps controller responses into the project's `Result` envelope.
- `Filter/GetUserIdFromUserClaims`: attribute/resource filter that extracts the current user id from JWT claims and places it in `HttpContext.Items["id"]` for handlers to use.

### Caching

- Registered via the `AddCaching(configuration)` extension in `Program.cs`.
- Supports in-memory and distributed providers (StackExchange.Redis and Postgres distributed cache packages are referenced in the project).
- Use cache services via `IMemoryCache` or `IDistributedCache` in services/repositories.
- Real-time: SignalR hubs for banners, orders, order items and stores (`shared/signalr`)
- Geospatial: `NetTopologySuite` for spatial data handling
- Third-party integrations: Firebase (admin), Stripe payments, SMTP for email notifications
- EF Core + Unit of Work + Repository pattern for data access
- Swagger/OpenAPI + API versioning

## Repository layout (key folders)

- `application/` — application services, result types, dependency injection helpers
- `domain/` — entity models and repository interfaces
- `Presentation/` — API controllers and DTOs (HTTP layer)
- `Infrastructure/` — EF Core repositories, `IUnitOfWork`, and data access
- `Migrations/` — EF Core migrations
- `Settings/` — typed settings classes (SMTP, Stripe, credentials)
- `shared/` — shared helpers, middleware, SignalR hub code
- `images/`, `endpointimages/` — image assets used by the project

## Prerequisites

- .NET SDK 10.0 (the repo uses `global.json` to target SDK 10)
- PostgreSQL (or a running PostgreSQL-compatible DB)
- Optional: `dotnet-ef` tool for applying migrations

## Local setup

1. Restore and build

```bash
dotnet restore
dotnet build
```

2. Configure settings

Copy and edit `appsettings.Development.json` (or set environment variables) to configure:
- `ConnectionStrings:Default` — your PostgreSQL connection string
- JWT settings, SMTP and other credentials (see `Settings/` folder for typed classes)

3. Apply EF Core migrations (if needed)

```bash
dotnet tool install --global dotnet-ef # if not already installed
dotnet ef database update --project api.csproj
```

4. Run the API

```bash
dotnet run --project api.csproj
```

By default Swagger UI will be available at `https://localhost:5001/swagger` (or the configured URL).

## Database helpers

- A `Migrations/` folder contains the current EF migrations. Use `dotnet ef` commands from the repo root and point to `api.csproj` if necessary.
- A SQL file `trigger.sql` is present for any DB triggers the project expects — inspect and load it into your DB as needed.

## Running & development notes

- Controllers are under `Presentation/controller/` and DTOs in `Presentation/dto/`.
- Business logic lives in `application/` and data access in `Infrastructure/Repositories/`.
- Global exception handling is implemented in `Exceptions/GlobalExceptionHandler.cs` and registered in `Program.cs`.

## API documentation

Interactive API docs are exposed by Swagger when the app runs. For a quick reference, see `API_DOCUMENTATION.md` in the repo — note the live Swagger is authoritative.

## Contributing

Follow normal fork->branch->PR workflow. Include tests and update migrations only when schema changes are required.

---
If you'd like changes or extra sections (e.g., environment variables, CI, Docker, expanded endpoint examples), tell me which details to include.
# ecommerce_api

A .NET 10 e-commerce backend providing RESTful APIs for products, stores, users, orders, and delivery.

## Quick summary

- Framework: .NET 10 (see `global.json`)
- Database: PostgreSQL (EF Core Npgsql)
- API docs: Swagger available at `/swagger` when the app is running

## Repository layout (key folders)

- `application/` — application services, result types, dependency injection helpers
- `domain/` — entity models and repository interfaces
- `Presentation/` — API controllers and DTOs (HTTP layer)
- `Infrastructure/` — EF Core repositories, `IUnitOfWork`, and data access
- `Migrations/` — EF Core migrations
- `Settings/` — typed settings classes (SMTP, Stripe, credentials)
- `shared/` — shared helpers, middleware, SignalR hub code
- `images/`, `endpointimages/` — image assets used by the project

## Prerequisites

- .NET SDK 10.0 (the repo uses `global.json` to target SDK 10)
- PostgreSQL (or a running PostgreSQL-compatible DB)
- Optional: `dotnet-ef` tool for applying migrations

## Local setup

1. Restore and build

```bash
dotnet restore
dotnet build
```

2. Configure settings

Copy and edit `appsettings.Development.json` (or set environment variables) to configure:
- `ConnectionStrings:Default` — your PostgreSQL connection string
- JWT settings, SMTP and other credentials (see `Settings/` folder for typed classes)

3. Apply EF Core migrations (if needed)

```bash
dotnet tool install --global dotnet-ef # if not already installed
dotnet ef database update --project api.csproj
```

4. Run the API

```bash
dotnet run --project api.csproj
```

By default Swagger UI will be available at `https://localhost:5001/swagger` (or the configured URL).

## Database helpers

- A `Migrations/` folder contains the current EF migrations. Use `dotnet ef` commands from the repo root and point to `api.csproj` if necessary.
- A SQL file `trigger.sql` is present for any DB triggers the project expects — inspect and load it into your DB as needed.

## Running & development notes

- Controllers are under `Presentation/controller/` and DTOs in `Presentation/dto/`.
- Business logic lives in `application/` and data access in `Infrastructure/Repositories/`.
- Global exception handling is implemented in `Exceptions/GlobalExceptionHandler.cs`.

## API documentation

Interactive API docs are exposed by Swagger when the app runs. For a quick reference, see `API_DOCUMENTATION.md` in the repo — note the live Swagger is authoritative.

## Contributing

Follow normal fork->branch->PR workflow. Include tests and update migrations only when schema changes are required.

---
If you'd like changes or extra sections (e.g., environment variables, CI, Docker), tell me which details to include.
This is fantastic\! Having the API endpoints (from your Swagger/OAS images) and the technology stack (from your `.csproj` file) makes generating a highly effective **`README.md`** much easier.

Since this is a comprehensive API project, I'll generate a detailed `README.md` using the structure I previously recommended.

-----

## `README.md` for `ecommerc_dotnet` API

# 🛍️ ecommerc\_dotnet API

[](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
[](https://www.google.com/search?q=/swagger/v1/swagger.json)
[](https://www.postgresql.org/)

A robust, scalable RESTful API built with **.NET 9.0** for a modern e-commerce platform. This backend handles all core business logic, including product management, user authentication, order processing, and delivery coordination.

-----

## ✨ Features

  * **Full CRUD** operations for **Products, Categories, Stores**, and **Users**.
  * **Secure Authentication:** JWT-based user and delivery authentication (Login/Signup, OTP verification, Password Reset).
  * **Geospatial Capabilities:** Uses **NetTopologySuite** for handling location data (likely for store locations or delivery tracking).
  * **Real-time Communication:** Integrated with **SignalR** for real-time updates (e.g., delivery status changes).
  * **Order Management:** Complete workflow for placing, managing, and tracking orders and order items.

-----

## 🚀 Getting Started

### Prerequisites

To run this project locally, you will need:

1.  **[.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)**
2.  **PostgreSQL** Database instance

### Installation

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/your-username/ecommerc_dotnet.git
    cd ecommerc_dotnet
    ```

2.  **Configure Environment Variables:**
    Create a `appsettings.Development.json` file and configure the following:

      * **PostgreSQL Connection String**
      * **JWT Secret Key**

3.  **Apply Database Migrations:**

    ```bash
    dotnet ef migrations add initial
    dotnet ef database update
    ```
    *(Note: Ensure your `DbContext` is correctly configured before running this.)*

4. **Copy the trigger.sql content  in psql**
   ```bash
   sudo -i -u postgres;
   psql 
   \c ecommerce_db;```
  ### past file at that command 


5.  **Run the application:**

    ```bash
    dotnet run
    ```

    The API will typically start on `https://localhost:5001`.

-----

## 📚 API Endpoints Overview

The API is fully documented via **Swagger/OpenAPI** and can be accessed at: `https://localhost:5001/swagger`

| Resource | Key Endpoints | Description |
| :--- | :--- | :--- |
| **User** | `/api/User/signup`, `/api/User/login`, `/api/User/me` | Authentication, user profile management, address management, and OTP flow. |
| **Store** | `/api/Store`, `/api/Store/{storeId}`, `/api/Store/me` | CRUD operations for managing e-commerce stores. Includes status patching. |
| **Product** | `/api/Product`, `/api/Product/{productId}`, `/api/Product/category/{categoryId}` | Core CRUD for products, fetching products by store, category, or subcategory. |
| **Order** | `/api/Order`, `/api/Order/all/{pageNumber}`, `/api/Order/me/{pageNumber}` | Placing, viewing, and managing customer orders. |
| **Delivery** | `/api/Delivery/login`, `/api/Delivery/new`, `/api/Delivery/{orderId}` | Authentication and management for delivery personnel, including status updates. |
| **Category/Subcategory** | `/api/Category`, `/api/Subcategory` | CRUD for organizing products. |
| **Banner** | `/api/Banner` | Management of promotional banners displayed in the application. |

-----

## 🛠️ Technology Stack

| Category | Technology | Purpose |
| :--- | :--- | :--- |
| **Framework** | **.NET 9.0 (ASP.NET Core)** | Core application framework. |
| **Database** | **Npgsql.EntityFrameworkCore.PostgreSQL** | ORM for PostgreSQL. |
| **Geospatial** | **NetTopologySuite** | Handling geographical data types and operations. |
| **Auth** | **JWT Bearer Authentication** | Secure token-based authentication. |
| **Real-time** | **Microsoft.AspNetCore.SignalR** | Real-time communication for updates. |
| **Documentation** | **Swashbuckle.AspNetCore** | Generates the Swagger/OpenAPI documentation. |
| **External** | **FirebaseAdmin** | Integration with Firebase services. |

-----

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

For detailed guidelines on reporting bugs and submitting code, please see **`CONTRIBUTING.md`**.

-----

## 📄 License

Distributed under the **[MIT License](https://www.google.com/search?q=LICENSE)**. See `LICENSE` for more information.

-----

## 📞 Support and Contact

If you have any questions, feel free to open a detailed **GitHub Issue**.
# ecommerce_api
