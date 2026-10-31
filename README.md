# ☕ NineCafe Management System

A full-stack, enterprise-grade cafe management platform designed for point-of-sale (POS), order processing, inventory, and staff administration.

---

## 📁 Repository Structure

```text
NineCafeManagementSystem/
├── NineCafeApi/        # ASP.NET Core Web API (.NET 10) Backend
│   ├── Authorization/  # Dynamic Policy Provider & Custom Permissions
│   ├── Controllers/    # Thin REST API Controllers
│   ├── Data/           # EF Core AppDbContext & Database Seeder
│   ├── DTOs/           # Request and Response Data Transfer Objects (Auth, Users, Roles, etc.)
│   ├── Migrations/     # Entity Framework Core Code-First Migrations
│   ├── Models/         # Database Entities (User, Role, Permission, UserRole, RolePermission, Module)
│   └── Services/       # Business Logic Layer (Auth, Users, Roles, Permissions, Modules)
└── Frontend/           # Client-Side Application (Work In Progress)
```

---

## 🛡️ Architecture & Security Highlights

### 1. Standard 5-Table NIST RBAC Model
The backend implements the classic **5-table Role-Based & Permission-Based Access Control (RBAC/PBAC)** architecture:
- **`Users`**: Stores staff credentials, contacts, and state.
- **`Roles`**: Defines job functions (Admin, Manager, Cashier, Barista).
- **`Permissions`**: Granular operations (`order.create`, `user.read`, `role.assign_permission`).
- **`UserRoles`**: Relational junction mapping **Users ↔ Roles**.
- **`RolePermissions`**: Relational junction mapping **Roles ↔ Permissions** with dedicated surrogate keys and unique composite indexes.
- **`Modules`**: Logical domain categorizer grouping permissions for UI checklists.

Dynamic authorization is enforced via custom `[HasPermission(Perm.OPERATION)]` attributes backed by `IAuthorizationPolicyProvider` and `AuthorizationHandler`.

### 2. Dual-ID Pattern (Enumeration & IDOR Protection)
- **Internal Database Engine:** Uses auto-incrementing `int Id` for clustered primary keys, high-speed joins, and relational integrity.
- **Public API Contract:** Exposes a cryptographically non-sequential `Guid PublicId` (e.g., `c3b2f518-8a21-4f12-9c1a-7b3b4f98d123`).
- **Benefit:** Eliminates integer ID enumeration, guessing attacks, and unauthorized data scraping.

### 3. JWT Authentication & Server-Side Token Revocation
- **Stateless Tokens:** Signed with HMAC-SHA256, carrying user claims and permission sets for 8-hour staff shifts.
- **BCrypt Password Hashing:** Secure salting and hashing for all stored credentials.
- **Token Blacklisting (`ITokenBlacklistService`):** In-memory cache invalidation on `/api/auth/logout` that rejects revoked tokens instantly upon logout via `JwtBearerEvents.OnTokenValidated`.

### 4. Decoupled Service Layer & Data Transfer Objects (DTOs)
- **Thin Controllers:** Controllers only process HTTP concerns and delegate business rules to injected services (`IUserService`, `IRoleService`, `IAuthService`, etc.).
- **Strict DTO Mapping:** Sensitive columns (`PasswordHash`, internal database `Id`) are never leaked to client responses.

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
2. Verify the connection string in `appsettings.json`:
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
   - **Scalar API Documentation:** `https://localhost:<port>/scalar/v1`
   - **OpenAPI JSON Specification:** `https://localhost:<port>/openapi/v1.json`

---

## 🔑 Default Seed Administrator Credentials

On first application startup, `DatabaseSeeder` automatically populates the initial permissions, roles, and administrative user:

| Field | Value |
| :--- | :--- |
| **Username** | `admin` |
| **Password** | `Abcd1234%` |
| **Email** | `admin@ninecafe.local` |
| **Phone** | `012345678` |
| **Role** | `Admin` (All permissions assigned) |

---

## 👥 Contributors & Collaboration

- **Backend:** ASP.NET Core Web API (.NET 10) featuring complete RBAC, JWT Auth, and EF Core SQL Server data layer.
- **Frontend:** Coming soon in the `Frontend/` folder for client-side point-of-sale and staff portal.
