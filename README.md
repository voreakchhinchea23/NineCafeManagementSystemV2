# ☕ Nine Cafe Management System (V2)

An enterprise-grade cafe operations, financial ledger, and role-based administration backend built with **ASP.NET Core Web API (.NET 10)** and **Entity Framework Core (SQL Server)**.

Designed specifically for daily cafe operations—tracking bean/cup sales in Cambodian Riel (KHR), cash withdrawals, customer credit/debts, staff operational expenses, supplier directories, monthly profit/loss reporting, granular permission-based access control (RBAC), and automatic JSON audit logging.

---

## 📁 Repository Structure

```text
NineCafeManagementSystem/
├── NineCafeApi/
│   └── NineCafeApi/               # ASP.NET Core Web API (.NET 10)
│       ├── Authorization/         # Dynamic Policy Provider, Handler & Perm Constants
│       ├── Controllers/           # 11 RESTful API Controllers protected by [HasPermission]
│       ├── Data/                  # EF Core AppDbContext (with Audit Interceptor) & DatabaseSeeder
│       ├── DTOs/                  # Request & Response Data Transfer Objects
│       │   ├── AuditLogs/         # Audit log & JSON diff DTOs
│       │   ├── Auth/              # Login & JWT response DTOs
│       │   ├── Dashboard/         # Daily sales, withdrawals & summary card DTOs
│       │   ├── Debts/             # Customer credit & settlement DTOs
│       │   ├── Expenses/          # Operational expense DTOs
│       │   ├── Modules/           # Permission module category DTOs
│       │   ├── Permissions/       # Granular permission DTOs
│       │   ├── Prices/            # KHR pricing tier DTOs
│       │   ├── Reports/           # Monthly financial breakdown DTOs
│       │   ├── Roles/             # Role & permission assignment DTOs
│       │   ├── Suppliers/         # Vendor & supplier note DTOs
│       │   └── Users/             # Staff account & password management DTOs
│       ├── Migrations/            # EF Core Code-First SQL Server Migrations
│       ├── Models/                # 13 Domain & Relational Database Entities
│       └── Services/              # Decoupled Business Logic Layer (Interfaces + Implementations)
└── README.md
```

---

## ✨ Core Features & Modules

### 1. 📊 Daily Dashboard, Sales & Cash Withdrawals (`/api/dashboard`)
- **Daily Sales Tracking (`DailySale`):** Record daily sales quantities linked to KHR price tiers (`Price`), automatically snapshotting `UnitPrice` at the time of sale so historical revenue remains accurate even if unit prices change later.
- **Cash Withdrawals (`Withdrawal`):** Track owner/operational cash payouts with amount, reason, and date.
- **Real-Time KPI Aggregations:** Computes total quantity sold, gross sales (KHR), total expenses, total withdrawals, unpaid customer debts, and net remaining cash for any selected date or month.

### 2. 💳 Customer Debt & Credit Ledger (`/api/debts`)
- Track unpaid customer tabs (`CustomerName`, `Amount`, `Date`, `IsPaid`, `PaidAt`).
- Supports instant settlement toggling (`IsPaid = true` automatically stamps `PaidAt`) and summary totals for outstanding vs. settled credit.

### 3. 🧾 Operational Expenses (`/api/expenses`)
- Record daily cafe expenses (`Date`, `Amount`, `Description`, `Other`) attributed to the staff member (`UserId`) who logged the transaction.

### 4. 🏷️ KHR Pricing Tiers (`/api/prices`)
- Manage master unit pricing in Cambodian Riel (`PriceRiel`) referenced by daily sale records, with deletion protection (`DeleteBehavior.Restrict`) when a price tier is in use by historical sales.

### 5. 🚚 Supplier Directory (`/api/suppliers`)
- Lightweight vendor and supplier notebook (`Name`, `ContactInfo`, `Address`, `Product`, `Note`) to track coffee bean roasters, milk/syrup vendors, and delivery schedules without complex inventory overhead.

### 6. 📈 Monthly Financial Reports (`/api/reports`)
- Generates monthly financial summaries (`?year=YYYY&month=MM`) including total revenue, total expenses, net profit, total withdrawals, unpaid debt totals, and day-by-day breakdowns.

### 7. 🕵️ Automatic System Audit Trail (`/api/audit-logs`)
- **Zero-Boilerplate Interception:** `AppDbContext.SaveChangesAsync` automatically intercepts `Added` (`"create"`), `Modified` (`"update"`), and `Deleted` (`"delete"`) operations across all operational entities (`DailySale`, `Withdrawal`, `Debt`, `Expense`, `Price`, `Supplier`).
- **Before & After JSON Snapshots:** Captures serialized `BeforeData` and `AfterData` JSON payloads along with the acting user's identity resolved directly from the JWT `NameIdentifier` claim.
- **Preserved History:** Configured with `DeleteBehavior.SetNull` on `UserId` so historical audit logs remain intact even if a staff account is later deleted.

---

## 🛡️ Security & Architecture Highlights

### 1. Granular Role-Based Access Control (RBAC)
Implements a 6-table security model (`Users`, `Roles`, `Permissions`, `Modules`, `UserRoles`, `RolePermissions`):
- Every controller endpoint is guarded by `[HasPermission(Perm.MODULE_ACTION)]`.
- Custom `PermissionPolicyProvider` and `PermissionAuthorizationHandler` evaluate JWT `"permission"` claims dynamically at runtime.
- `DatabaseSeeder` automatically discovers all constants in `Perm.cs` via reflection, seeds missing `Modules` and `Permissions`, and assigns full privileges to the `Admin` role on startup.

### 2. Dual-ID Pattern (Enumeration & IDOR Protection)
- **Internal Database Key:** Uses `int Id` for fast clustered indexing and foreign key joins.
- **External Public Identifier:** Exposes `Guid PublicId` on `User` endpoints so sequential integer IDs are never exposed in URLs or payloads.

### 3. Stateless JWT Authentication + Instant Logout Blacklisting
- **JWT Bearer Tokens:** Signed with HMAC-SHA256 (`Jwt:Key`), valid for 8-hour shifts, embedding `NameIdentifier` (`PublicId`), `Name`, `Email`, `Role`, and all assigned `permission` claims.
- **BCrypt Password Hashing:** All user passwords are salted and hashed via `BCrypt.Net-Next`.
- **In-Memory Token Revocation (`ITokenBlacklistService`):** Logging out via `POST /api/auth/logout` blacklists the active JWT until its expiration, verified on every request via `JwtBearerEvents.OnTokenValidated`.

---

## 🗄️ Database Schema (13 Entities)

| Subsystem | Entity | Description |
| :--- | :--- | :--- |
| **RBAC & Security** | `User` | Staff & admin accounts (`Id`, `PublicId`, `Username`, `FullName`, `Email`, `PhoneNumber`, `PasswordHash`, `IsActive`) |
| **RBAC & Security** | `Role` | Job roles (`Admin`, `Manager`, `Cashier`, etc.) |
| **RBAC & Security** | `UserRole` | Associative bridge linking `User` $\leftrightarrow$ `Role` |
| **RBAC & Security** | `Module` | Logical feature grouping for permissions (`USER`, `ROLE`, `DASHBOARD`, `DEBT`, `EXPENSE`, `PRICE`, `SUPPLIER`, `REPORT`, `AUDIT`) |
| **RBAC & Security** | `Permission` | Granular permission code (`user.read`, `dashboard.create`, `audit.read`, etc.) |
| **RBAC & Security** | `RolePermission` | Associative bridge linking `Role` $\leftrightarrow$ `Permission` |
| **System Audit** | `AuditLog` | Tracks `UserId`, `Action`, `EntityType`, `EntityId`, `BeforeData` (JSON), `AfterData` (JSON), `CreatedAt` |
| **Cafe Operations** | `Price` | Master unit price in Riel (`PriceRiel`) |
| **Cafe Operations** | `DailySale` | Daily sales record (`SaleDate`, `Quantity`, `UnitPrice`, derived `Total`, `PriceId`) |
| **Cafe Operations** | `Expense` | Operational expense record (`Date`, `Amount`, `Description`, `Other`, `UserId`) |
| **Cafe Operations** | `Withdrawal` | Cash payout / withdrawal log (`Amount`, `Reason`, `Date`) |
| **Cafe Operations** | `Debt` | Customer credit ledger (`CustomerName`, `Amount`, `Date`, `IsPaid`, `PaidAt`) |
| **Cafe Operations** | `Supplier` | Vendor directory (`Name`, `ContactInfo`, `Address`, `Product`, `Note`) |

---

## 🔌 API Endpoints & Required Permissions

| Module | Endpoints | Permission Constants (`Perm.cs`) |
| :--- | :--- | :--- |
| **Auth** | `POST /api/auth/login`<br>`POST /api/auth/logout` | Public (`[AllowAnonymous]`)<br>Authenticated (`[Authorize]`) |
| **Dashboard** | `GET /api/dashboard`<br>`POST /api/dashboard/sales`<br>`PUT /api/dashboard/sales/{id}`<br>`DELETE /api/dashboard/sales/{id}`<br>`POST /api/dashboard/withdrawals`<br>`PUT /api/dashboard/withdrawals/{id}`<br>`DELETE /api/dashboard/withdrawals/{id}` | `dashboard.read`<br>`dashboard.create`<br>`dashboard.update`<br>`dashboard.delete`<br>`dashboard.create`<br>`dashboard.update`<br>`dashboard.delete` |
| **Prices** | `GET /api/prices`<br>`POST /api/prices`<br>`PUT /api/prices/{id}`<br>`DELETE /api/prices/{id}` | `price.read`<br>`price.create`<br>`price.update`<br>`price.delete` |
| **Expenses** | `GET /api/expenses`<br>`POST /api/expenses`<br>`PUT /api/expenses/{id}`<br>`DELETE /api/expenses/{id}` | `expense.read`<br>`expense.create`<br>`expense.update`<br>`expense.delete` |
| **Debts** | `GET /api/debts`<br>`POST /api/debts`<br>`PUT /api/debts/{id}`<br>`DELETE /api/debts/{id}` | `debt.read`<br>`debt.create`<br>`debt.update`<br>`debt.delete` |
| **Suppliers** | `GET /api/suppliers`<br>`POST /api/suppliers`<br>`PUT /api/suppliers/{id}`<br>`DELETE /api/suppliers/{id}` | `supplier.read`<br>`supplier.create`<br>`supplier.update`<br>`supplier.delete` |
| **Reports** | `GET /api/reports/monthly?year=YYYY&month=MM` | `report.read` |
| **Audit Logs** | `GET /api/audit-logs?limit=100&entityType=...&action=...` | `audit.read` |
| **Users** | `GET /api/users`<br>`POST /api/users`<br>`PUT /api/users/{publicId}`<br>`PUT /api/users/{publicId}/change-password`<br>`DELETE /api/users/{publicId}` | `user.read`<br>`user.create`<br>`user.update`<br>`user.change_password`<br>`user.delete` |
| **Roles** | `GET /api/roles`<br>`POST /api/roles`<br>`PUT /api/roles/{id}`<br>`PUT /api/roles/{id}/permissions`<br>`DELETE /api/roles/{id}` | `role.read`<br>`role.create`<br>`role.update`<br>`role.assign_permission`<br>`role.delete` |
| **Permissions** | `GET /api/permissions` (`POST`, `PUT`, `DELETE`) | `permission.read`, `permission.create`, `permission.update`, `permission.delete` |
| **Modules** | `GET /api/modules` (`POST`, `PUT`, `DELETE`) | `module.read`, `module.create`, `module.update`, `module.delete` |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB, Express, or Developer Edition)
- Visual Studio 2022/2026 or VS Code

### Setup & Run
1. **Clone the repository:**
   ```bash
   git clone https://github.com/voreakchhinchea23/NineCafeManagementSystemV2.git
   cd NineCafeManagementSystemV2/NineCafeApi/NineCafeApi
   ```
2. **Configure Database & JWT in `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=NineCafeDb;Trusted_Connection=True;TrustServerCertificate=true;"
   }
   ```
3. **Apply EF Core Migrations:**
   ```bash
   dotnet ef database update
   ```
   *(Or run `Update-Database` in Visual Studio Package Manager Console.)*
4. **Start the API Server:**
   ```bash
   dotnet run
   ```
5. **Explore Interactive API Documentation:**
   - **Scalar API Reference UI:** `https://localhost:<port>/scalar/v1`
   - **OpenAPI JSON Spec:** `https://localhost:<port>/openapi/v1.json`

---

## 🔑 Default Seed Administrator Credentials

On startup, `DatabaseSeeder` automatically seeds all modules, permissions, the `Admin` role, and the default administrator account:

| Field | Value |
| :--- | :--- |
| **Username** | `admin` |
| **Password** | `Abcd1234%` |
| **Full Name** | `System Admin` |
| **Email** | `admin@ninecafe.local` |
| **Phone** | `012345678` |
| **Role** | `Admin` (Automatically granted all permissions in `Perm.cs`) |
