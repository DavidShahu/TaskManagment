# Task Management Platform

A full-stack task management platform built with **ASP.NET Core 9** and **Angular 19**, featuring project-based task organization, role-based access control, and time tracking.

---

## Features

### User Features
- JWT Authentication (Register/Login)
- Create, view, update, and delete tasks
- View projects you are a member of
- Mark tasks as Open or Done
- Time tracking with start/stop timer (persists across devices)
- Log time manually with notes and view full history
- Task types (Programming, Design, Bug Fix, Research, Meeting)

### Admin Features
- Manage users (view, activate, deactivate)
- Full project management (create, update, delete, activate)
- Add/remove members from projects
- View and manage all tasks across all users
- Assign tasks to specific users
- Manage task types (create, update, activate, deactivate)

---

## Tech Stack

### Backend
- ASP.NET Core 9 Web API
- Entity Framework Core 9 (Code First)
- SQL Server
- JWT Authentication
- Serilog (structured logging)
- BCrypt (password hashing)
- Clean Architecture (Domain → Application → Infrastructure → API)

### Frontend
- Angular 19 (Standalone Components)
- Bootstrap 5
- SweetAlert2
- ngx-toastr

### DevOps
- GitHub Actions CI/CD
- Docker (coming soon)

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Server Express)
- [Node.js 20+](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) — `npm install -g @angular/cli`

---

## Setup & Configuration

### 1 — Clone the repository

```bash
git clone https://github.com/DavidShahu/TaskManagment.git
cd TaskManagment
```

### 2 — Backend configuration

Create `appsettings.Development.json` inside the `TaskManagment` API project. This file is excluded from git for security reasons.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_MINIMUM_32_CHARACTERS"
  }
}
```

> The secret key must be at least 32 characters long.

### 3 — Run database migrations

Open Package Manager Console in Visual Studio, set `Infrastructure` as the default project and run:

```powershell
Update-Database -StartupProject TaskManagment
```

Or via CLI:

```bash
dotnet ef database update --project Infrastructure --startup-project TaskManagment
```

The database will be created automatically with all required tables and one seeded Admin user.

### 4 — Run the backend

```bash
cd TaskManagment
dotnet run
```

API will be available at `https://localhost:7145` and `http://localhost:5016`.

Swagger UI is available at `https://localhost:7145/swagger`.

### 5 — Frontend configuration

The API URL is configured in `frontend/src/app/core/auth/auth.ts` and the service files under `frontend/src/app/core/services/`. Make sure the port matches your backend.

### 6 — Run the frontend

```bash
cd frontend
npm install
ng serve
```

Frontend will be available at `http://localhost:4200`.

---

## Default Credentials

After running migrations the following admin account is seeded automatically:

| Field    | Value                        |
|----------|------------------------------|
| Email    | admin@taskmanagement.com     |
| Password | Admin@1234                   |
| Role     | Admin                        |

> Change these credentials before deploying to production.

---

## Project Structure

```
TaskManagment/
├── .github/
│   └── workflows/
│       └── ci.yml                      # GitHub Actions CI pipeline
│
├── Domain/                             # Business logic, entities, value objects
│   ├── Entities/                       # User, Project, TaskItem, TaskType,
│   │                                   # ProjectMember, TimeLog, ActiveTimer
│   ├── Enums/                          # UserRole, TaskStatus
│   └── Primitives/                     # AggregateRoot, Entity, ValueObject
│
├── Application/                        # Use cases, DTOs, service interfaces
│   ├── Auth/                           # Register/Login DTOs and interfaces
│   ├── Projects/                       # Project DTOs and interfaces
│   ├── Tasks/                          # Task DTOs and interfaces
│   ├── TaskTypes/                      # Task type DTOs and interfaces
│   └── Users/                          # User DTOs and interfaces
│
├── Infrastructure/                     # EF Core, JWT, repositories, services
│   ├── Persistence/                    # DbContext, migrations, configurations
│   └── Services/                       # AuthService, ProjectService, TaskService, etc.
│
├── TaskManagment/                      # ASP.NET Core Web API entry point
│   ├── Controllers/                    # AuthController, ProjectsController, etc.
│   ├── Middleware/                     # Exception handling, correlation IDs
│   └── Extensions/                     # ClaimsPrincipal extensions
│
├── UnitTests/                          # xUnit unit tests
│   ├── Domain/                         # Entity and value object tests
│   ├── Services/                       # Service layer tests with Moq
│   └── Tasks/                          # TaskItem domain tests
│
└── frontend/                           # Angular 19 SPA
    └── src/app/
        ├── core/                       # Auth, interceptors, guards, services
        ├── features/                   # Auth, dashboard, projects, tasks
        ├── layout/                     # Sidebar, topbar, main layout
        └── shared/                     # Reusable components
```

---

## Architecture

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

Key principles:
- Domain models never reference external libraries
- Business logic lives in the Domain layer
- Infrastructure implements interfaces defined in Application
- Controllers are thin — they only receive requests and return responses

---

## Time Tracking

Each task supports time tracking:
- Set estimated hours when creating or editing a task
- Start and stop a timer that persists across browsers and devices
- Log time manually with an optional note
- View the full time log history per task
- Progress bar showing logged hours vs estimated hours
- Active timer is visible in the topbar from any page in the app

---

## Security

- Passwords hashed with BCrypt
- JWT tokens with configurable expiry (default 24 hours)
- Role-based authorization (Admin / User)
- Global exception handling — stack traces never exposed to clients
- Correlation IDs on every request for tracing
- Rate limiting on auth endpoints (5 requests per minute per IP)

---

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage"
```

Tests cover:
- Domain entity validation (User, Project, TaskItem)
- AuthService (register, login, edge cases)
- ProjectService (CRUD, member management)
- TaskService (CRUD, permissions, time logging)
- TaskTypeService (CRUD, activate/deactivate)

---

## API Endpoints

### Auth
| Method | Endpoint              | Description       | Auth   |
|--------|-----------------------|-------------------|--------|
| POST   | `/api/auth/register`  | Register new user | Public |
| POST   | `/api/auth/login`     | Login             | Public |

### Projects
| Method | Endpoint                                    | Description      | Auth       |
|--------|---------------------------------------------|------------------|------------|
| GET    | `/api/projects`                             | Get projects     | Required   |
| GET    | `/api/projects/{id}`                        | Get by ID        | Required   |
| POST   | `/api/projects`                             | Create project   | Admin only |
| PUT    | `/api/projects/{id}`                        | Update project   | Admin only |
| DELETE | `/api/projects/{id}`                        | Delete project   | Admin only |
| PATCH  | `/api/projects/{id}/activate`               | Activate project | Admin only |
| POST   | `/api/projects/{id}/members/{userId}`       | Add member       | Admin only |
| DELETE | `/api/projects/{id}/members/{userId}`       | Remove member    | Admin only |

### Users
| Method | Endpoint                  | Description         | Auth       |
|--------|---------------------------|---------------------|------------|
| GET    | `/api/users`              | Get all users       | Admin only |
| GET    | `/api/users/{id}`         | Get user by ID      | Admin only |
| DELETE | `/api/users/{id}`         | Deactivate user     | Admin only |
| PATCH  | `/api/users/{id}/status`  | Update user status  | Admin only |

### Tasks
| Method | Endpoint                          | Description           | Auth       |
|--------|-----------------------------------|-----------------------|------------|
| GET    | `/api/tasks`                      | Get all or own tasks  | Required   |
| GET    | `/api/tasks/my`                   | Get my tasks          | Required   |
| GET    | `/api/tasks/{id}`                 | Get task by ID        | Required   |
| GET    | `/api/tasks/project/{projectId}`  | Get tasks by project  | Required   |
| POST   | `/api/tasks`                      | Create task           | Required   |
| PUT    | `/api/tasks/{id}`                 | Update task           | Required   |
| DELETE | `/api/tasks/{id}`                 | Delete task           | Required   |
| PATCH  | `/api/tasks/{id}/done`            | Mark as done          | Required   |
| PATCH  | `/api/tasks/{id}/open`            | Mark as open          | Required   |
| POST   | `/api/tasks/{id}/log-time`        | Log time              | Required   |
| GET    | `/api/tasks/timer`                | Get active timer      | Required   |
| POST   | `/api/tasks/{id}/timer/start`     | Start timer           | Required   |
| POST   | `/api/tasks/timer/stop`           | Stop timer            | Required   |

### Task Types
| Method | Endpoint                          | Description          | Auth       |
|--------|-----------------------------------|----------------------|------------|
| GET    | `/api/tasktypes`                  | Get all task types   | Required   |
| POST   | `/api/tasktypes`                  | Create task type     | Admin only |
| PUT    | `/api/tasktypes/{id}`             | Update task type     | Admin only |
| DELETE | `/api/tasktypes/{id}`             | Deactivate task type | Admin only |
| PATCH  | `/api/tasktypes/{id}/activate`    | Activate task type   | Admin only |

---

## CI/CD Pipeline

GitHub Actions runs automatically on every push to main:

1. Restore NuGet packages
2. Build the solution
3. Run all unit tests
4. Build the Angular frontend

---

## Roadmap

- [x] JWT Authentication
- [x] Project management
- [x] Role-based access control
- [x] Task CRUD with time tracking
- [x] Task types (admin managed)
- [x] Real-time timer persisted per user
- [x] Clean Architecture
- [x] Unit tests
- [x] CI/CD pipeline
- [ ] SignalR real-time notifications
- [ ] Email notifications
- [ ] Docker support
- [ ] Admin panel UI

---

## Author

**David Shahu**
- Email: david.shahu@gmail.com
- LinkedIn: [David Shahu](https://linkedin.com/in/david-shahu)
