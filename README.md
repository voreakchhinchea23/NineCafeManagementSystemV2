# ☕ NineCafe Management System

A full-stack cafe management platform designed for point-of-sale, order management, inventory, and staff administration.

---

## 📁 Repository Structure

```text
NineCafeManagementSystem/
├── NineCafeApi/        # ASP.NET Core Web API (.NET 10) Backend
│   ├── Authorization/  # Dynamic Policy Provider & Custom Permissions
│   ├── Controllers/    # Thin REST API Controllers
│   ├── Data/           # EF Core AppDbContext & Database Seeder
│   ├── DTOs/           # Request and Response Data Transfer Objects
│   ├── Migrations/     # Entity Framework Core Code-First Migrations
│   ├── Models/         # Database Entities (User, Role, Permission, Module, RolePermission)
│   └── Services/       # Business Logic Layer (Auth, Users, Roles, Permissions, Modules)
└── Frontend/           # Client-Side Application (To be implemented)
```

---

## 🚀 Backend Features (`NineCafeApi`)

- **Fine-Grained Role & Permission-Based Access Control (RBAC/PBAC):**
  - Modular permissions architecture (`Module` → `Permission` → `RolePermission` → `Role` → `User`).
  - Dynamic `[HasPermission(...)]` authorization attribute powered by `IAuthorizationPolicyProvider` and `AuthorizationHandler`.
- **JWT Authentication & Token Revocation:**
  - Secure token generation with user claims and assigned permissions.
  - BCrypt password hashing for secure credential storage.
  - In-Memory Token Blacklist service (`ITokenBlacklistService`) for instant token invalidation upon logout.
- **Data Integrity & Relational Architecture:**
  - SQL Server database with Entity Framework Core migrations.
  - Surrogate key on `RolePermission` join table with unique composite constraints.
  - Automatic timestamps (`CreatedAt`, `UpdatedAt`) and strict phone number validation.
- **Clean Architecture & Decoupled DTOs:**
  - Strict separation between Entities, DTOs, and Services to prevent overposting and circular reference loops.
- **Interactive Documentation:**
  - Built-in **Scalar** and **OpenAPI** documentation (`/scalar/v1`).

---

## 🛠️ Getting Started (Backend)

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/sql-server/)
- Visual Studio 2022/2025 or VS Code

### Setup & Run
1. Navigate to the API directory:
   ```bash
   cd NineCafeApi/NineCafeApi
   ```
2. Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=NineCafeDb;Trusted_Connection=True;TrustServerCertificate=true;"
   }
   ```
3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the API:
   ```bash
   dotnet run
   ```
5. Open your browser and navigate to:
   - Scalar Documentation: `https://localhost:<port>/scalar/v1`

---

## 🔑 Default Seed Admin Credentials

When the API launches, `DatabaseSeeder` automatically populates the initial permissions, roles, and administrative user:

| Field | Value |
| :--- | :--- |
| **Username** | `admin` |
| **Password** | `Abcd1234%` |
| **Email** | `admin@ninecafe.local` |
| **Phone** | `012441593` |
| **Role** | `Admin` (All permissions assigned) |

---

## 👥 Contributors & Collaboration

- **Backend:** ASP.NET Core Web API (RBAC, Authentication, Database).
- **Frontend:** Work in progress in the `Frontend/` directory.
