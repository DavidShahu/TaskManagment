# Task Management Platform

A full-stack task management platform built with **ASP.NET Core 9** and **Angular 19**, featuring project-based task organization, role-based access control, and real-time notifications.

\---

## &#x20;Features

### User Features

* &#x20;JWT Authentication (Register/Login)
* &#x20;Create, view, update, and delete tasks
* &#x20;View projects you are a member of
* &#x20;Mark tasks as Open or Done
* &#x20;Real-time notifications via SignalR

### Admin Features

* &#x20;Manage users (view, activate, deactivate)
* &#x20;Full project management (create, update, delete, activate)
* &#x20;Add/remove members from projects
* &#x20;View all tasks across all projects

\---

## &#x20;Tech Stack

### Backend

* ASP.NET Core 9 Web API
* Entity Framework Core 9 (Code First)
* SQL Server
* JWT Authentication
* SignalR (real-time notifications)
* Serilog (structured logging)
* BCrypt (password hashing)
* Clean Architecture (Domain → Application → Infrastructure → API)

### Frontend

* Angular 19 (Standalone Components)
* Bootstrap 5
* SweetAlert2
* ngx-toastr
* SignalR Client

### DevOps

* GitHub Actions CI/CD
* Docker (coming soon)

\---

## &#x20;Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
* [Node.js 20+](https://nodejs.org/)
* [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)

\---

## 🔧 Setup \& Configuration

### 1 — Clone the repository

```bash
git clone https://github.com/DavidShahu/TaskManagment.git

cd task-management
```

### 2 — Backend configuration

Create `appsettings.Development.json` inside the `TaskManagment` API project:

```json
{

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "YOUR\_SQL\_SERVER\_CONNECTION\_STRING"

&#x20; },

&#x20; "JwtSettings": {

&#x20;   "SecretKey": "YOUR\_SECRET\_KEY\_MIN\_32\_CHARACTERS"

&#x20; }

}```

>  The secret key must be at least 32 characters long.

### 3 — Run database migrations

Open Package Manager Console in Visual Studio, set `Infrastructure` as the default project and run:

```powershell
Update-Database -StartupProject TaskManagment
```

Or via CLI:

```bash
dotnet ef database update --project Infrastructure --startup-project TaskManagment
```

The database will be created automatically with:

* All required tables
* One seeded **Admin** user

### 4 — Run the backend

```bash
cd TaskManagment
dotnet run
```

API will be available at:

```
https://localhost:7145
http://localhost:5016
```

Swagger UI:

```
https://localhost:7145/swagger
```

### 5 — Frontend configuration

The API URL is configured in `frontend/src/app/core/auth/auth.ts` and `frontend/src/app/core/services/\*.ts`. Make sure the port matches your backend.

### 6 — Run the frontend

```bash
cd frontend
npm install
ng serve
```

Frontend will be available at:

```
http://localhost:4200
```

\---

## &#x20;Default Credentials

After running migrations the following admin account is seeded automatically:

|Field|Value|
|-|-|
|Email|admin@taskmanagement.com|
|Password|Admin@1234|
|Role|Admin|

> ⚠️ Change these credentials in production.

\---

## &#x20;Project Structure

```
TaskManagment/
├── .github/
│   └── workflows/
│       └── ci.yml                 # GitHub Actions CI pipeline
│
├── src/
│   ├── Domain/                    # Business logic, entities, value objects
│   │   ├── Entities/              # User, Project, TaskItem, ProjectMember
│   │   ├── Enums/                 # UserRole, TaskStatus
│   │   └── Primitives/            # AggregateRoot, Entity, ValueObject
│   │
│   ├── Application/               # Use cases, DTOs, service interfaces
│   │   ├── Auth/                  # Register/Login DTOs and interfaces
│   │   ├── Projects/              # Project DTOs and interfaces
│   │   ├── Tasks/                 # Task DTOs and interfaces
│   │   └── Users/                 # User DTOs and interfaces
│   │
│   ├── Infrastructure/            # EF Core, JWT, repositories, services
│   │   ├── Persistence/           # DbContext, migrations, configurations
│   │   └── Services/              # AuthService, ProjectService, JwtService
│   │
│   └── TaskManagment/             # ASP.NET Core Web API
│       ├── Controllers/           # AuthController, ProjectsController, etc.
│       ├── Middleware/            # Exception handling, correlation IDs
│       └── Extensions/            # ClaimsPrincipal extensions
│
├── tests/
│   └── UnitTests/                 # xUnit unit tests
│       ├── Domain/                # Entity and value object tests
│       └── Services/              # Service layer tests with Moq
│
└── frontend/                      # Angular 19 SPA
    └── src/app/
        ├── core/                  # Auth, interceptors, guards, services
        ├── features/              # Auth, dashboard, projects, tasks
        ├── layout/                # Sidebar, topbar, main layout
        └── shared/                # Reusable components
```

\---

## &#x20;Architecture

This project follows **Clean Architecture** principles:

```
Domain (no dependencies)
    ↑
Application (depends on Domain)
    ↑
Infrastructure (depends on Application)
    ↑
API/Presentation (depends on Infrastructure)
```

**Key principles:**

* Domain models never reference external libraries
* Business logic lives in the Domain layer
* Infrastructure implements interfaces defined in Application
* Controllers are thin — they only receive requests and return responses

\---



## &#x20;Security

* Passwords hashed with **BCrypt**
* JWT tokens with configurable expiry (default 24 hours)
* Role-based authorization (`Admin` / `User`)
* Global exception handling — stack traces never exposed to clients
* Correlation IDs on every request for tracing
* Rate limiting on auth endpoints (5 requests/minute)
* HTTP-only approach for sensitive operations

\---

## 

## 

## &#x20;Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

Tests cover:

* Domain entity validation (User, Project)
* AuthService (register, login, edge cases)
* ProjectService (CRUD, member management)

\---

## 

## &#x20;API Endpoints

### Auth

|Method|Endpoint|Description|Auth|
|-|-|-|-|
|POST|`/api/auth/register`|Register new user|Public|
|POST|`/api/auth/login`|Login|Public|

### Projects

|Method|Endpoint|Description|Auth|
|-|-|-|-|
|GET|`/api/projects`|Get all (Admin) or own (User)|🔐|
|GET|`/api/projects/{id}`|Get project by ID|🔐|
|POST|`/api/projects`|Create project|👑 Admin|
|PUT|`/api/projects/{id}`|Update project|👑 Admin|
|DELETE|`/api/projects/{id}`|Delete project|👑 Admin|
|PATCH|`/api/projects/{id}/activate`|Activate project|👑 Admin|
|POST|`/api/projects/{id}/members/{userId}`|Add member|👑 Admin|
|DELETE|`/api/projects/{id}/members/{userId}`|Remove member|👑 Admin|

### Users

|Method|Endpoint|Description|Auth|
|-|-|-|-|
|GET|`/api/users`|Get all users|👑 Admin|
|GET|`/api/users/{id}`|Get user by ID|👑 Admin|
|DELETE|`/api/users/{id}`|Deactivate user|👑 Admin|
|PATCH|`/api/users/{id}/status`|Update user status|👑 Admin|

\---

## 

## &#x20;CI/CD Pipeline

GitHub Actions runs automatically on every push:

1. &#x20;Restore NuGet packages
2. &#x20;Build solution
3. &#x20;Run all unit tests
4. &#x20;Build Angular frontend

\---

## 

## &#x20;Roadmap

* \[x] JWT Authentication
* \[x] Project management
* \[x] Role-based access control
* \[x] Clean Architecture
* \[x] Unit tests
* \[x] CI/CD pipeline
* \[ ] Task CRUD
* \[ ] SignalR notifications
* \[ ] Email notifications
* \[ ] Docker support
* \[ ] Admin panel

\---

## 

## &#x20;Author

**David Shahu**

* Email: david.shahu@gmail.com
* LinkedIn: [David Shahu](https://linkedin.com/in/david-shahu)

