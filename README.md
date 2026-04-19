# TaskFlow — Task Management Platform

A full-stack task management platform built with **ASP.NET Core 9** and **Angular 19**, featuring project-based task organization, role-based access control, real-time notifications, and time tracking.

---

## Features

### User Features
- JWT Authentication (Register/Login)
- Create, view, update, and delete personal tasks
- View projects you are a member of
- Mark tasks as Open or Done
- Time tracking with a start/stop timer that persists across browsers and devices
- Log time manually with optional notes and view full history
- Task types (Programming, Design, Bug Fix, Research, Meeting and more)
- Real-time notifications via SignalR
- Notification history with mark as read support

### Admin Features
- Manage users (view, activate, deactivate, reset password)
- Full project management (create, update, delete, activate/deactivate)
- Add and remove members from projects
- View and manage all tasks across all users
- Assign tasks to specific users
- Filter tasks by user and date range
- Manage task types (create, update, activate, deactivate)

---

## Tech Stack

### Backend
- ASP.NET Core 9 Web API
- Entity Framework Core 9 (Code First)
- SQL Server
- JWT Authentication
- SignalR (real-time notifications)
- MediatR (domain events)
- Serilog (structured logging)
- BCrypt (password hashing)
- Clean Architecture (Domain → Application → Infrastructure → API)

### Frontend
- Angular 19 (Standalone Components)
- Bootstrap 5
- SweetAlert2
- ngx-toastr
- SignalR Client

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

The database will be created automatically with all required tables, seeded task types, and one Admin user.

### 4 — Run the backend

```bash
cd TaskManagment
dotnet run
```

API will be available at `https://localhost:7145` and `http://localhost:5016`.

Swagger UI is available at `https://localhost:7145/swagger`.

### 5 — Frontend configuration

The API URL is configured in `frontend/src/environments/environment.ts`. Make sure the port matches your backend.

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

| Field    | Value                    |
|----------|--------------------------|
| Email    | admin@taskmanagement.com |
| Password | Admin@1234               |
| Role     | Admin                    |

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
│   │                                   # ProjectMember, TimeLog, ActiveTimer,
│   │                                   # Notification
│   ├── Events/                         # UserAddedToProject, TaskAssignedToUser,
│   │                                   # TaskMarkedAsDone
│   ├── Enums/                          # UserRole, TaskStatus, NotificationType
│   └── Primitives/                     # AggregateRoot, Entity, IDomainEvent
│
├── Application/                        # Use cases, DTOs, service interfaces
│   ├── Auth/                           # Register/Login DTOs
│   ├── Notifications/                  # Notification DTOs and event handlers
│   ├── Projects/                       # Project DTOs and interfaces
│   ├── Tasks/                          # Task DTOs and interfaces
│   ├── TaskTypes/                      # Task type DTOs and interfaces
│   └── Users/                          # User DTOs and interfaces
│
├── Infrastructure/                     # EF Core, JWT, repositories, services
│   ├── Hubs/                           # SignalR NotificationHub
│   ├── Persistence/                    # DbContext, migrations, configurations
│   └── Services/                       # All service implementations
│
├── TaskManagment/                      # ASP.NET Core Web API entry point
│   ├── Controllers/                    # All API controllers
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
        ├── features/                   # Auth, dashboard, projects, tasks, admin
        ├── layout/                     # Sidebar, topbar, main layout
        └── shared/                     # Reusable components
```

---

## Architecture

This project follows **Clean Architecture** principles:

```
Domain (minimal dependencies)
    ↑
Application (depends on Domain)
    ↑
Infrastructure (depends on Application)
    ↑
API/Presentation (depends on Infrastructure)
```

Key principles:
- Domain models have no infrastructure dependencies
- Business logic lives in the Domain and Application layers
- Infrastructure implements interfaces defined in Application
- Controllers are thin — they only receive requests and return responses
- Domain events decouple side effects (notifications) from core business logic

### Architecture Notes

Domain references MediatR solely for the `INotification` marker interface on `IDomainEvent`. This is a deliberate pragmatic decision — MediatR's `INotification` is a zero-dependency marker interface with no runtime behavior, making its inclusion in Domain an acceptable tradeoff over introducing unnecessary wrapper complexity.

---

## Domain Events

Notifications are triggered via domain events using MediatR, keeping services decoupled from notification logic:

| Event | Trigger | Notifies |
|-------|---------|----------|
| `UserAddedToProject` | Admin adds user to project | The added user |
| `TaskAssignedToUser` | Admin assigns task to user | The assigned user |
| `TaskMarkedAsDone` | User completes a task | The task creator (admin) |

---

## Time Tracking

Each task supports full time tracking:
- Set estimated hours when creating or editing a task
- Start and stop a timer that persists across browsers and devices
- Timer is user-specific — switching accounts shows the correct timer
- Log time manually with an optional note
- View the full time log history per task
- Progress bar showing logged hours vs estimated hours
- Active timer is visible in the topbar from any page

---

## Real-time Notifications

Notifications are delivered instantly via SignalR:
- Bell icon in the topbar with unread count badge
- Dropdown panel showing the last 50 notifications
- Toast popup on receiving a new notification
- Click a notification to navigate to the related task or project
- Mark all as read in one click
- Notifications persist in the database across sessions

---

## Security

- Passwords hashed with BCrypt
- JWT tokens with configurable expiry (default 24 hours)
- Role-based authorization (Admin / User)
- Global exception handling — stack traces never exposed to clients
- Correlation IDs on every request for tracing
- Rate limiting on auth endpoints (5 requests per minute per IP)
- Admin password reset for users who cannot access their account

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
| Method | Endpoint             | Description       | Auth   |
|--------|----------------------|-------------------|--------|
| POST   | `/api/auth/register` | Register new user | Public |
| POST   | `/api/auth/login`    | Login             | Public |

### Projects
| Method | Endpoint                              | Description      | Auth       |
|--------|---------------------------------------|------------------|------------|
| GET    | `/api/projects`                       | Get projects     | Required   |
| GET    | `/api/projects/{id}`                  | Get by ID        | Required   |
| POST   | `/api/projects`                       | Create project   | Admin only |
| PUT    | `/api/projects/{id}`                  | Update project   | Admin only |
| DELETE | `/api/projects/{id}`                  | Delete project   | Admin only |
| PATCH  | `/api/projects/{id}/activate`         | Activate project | Admin only |
| POST   | `/api/projects/{id}/members/{userId}` | Add member       | Admin only |
| DELETE | `/api/projects/{id}/members/{userId}` | Remove member    | Admin only |

### Users
| Method | Endpoint                       | Description        | Auth       |
|--------|--------------------------------|--------------------|------------|
| GET    | `/api/users`                   | Get all users      | Admin only |
| GET    | `/api/users/{id}`              | Get user by ID     | Admin only |
| DELETE | `/api/users/{id}`              | Deactivate user    | Admin only |
| PATCH  | `/api/users/{id}/status`       | Update user status | Admin only |
| POST   | `/api/users/{id}/reset-password` | Reset password   | Admin only |

### Tasks
| Method | Endpoint                         | Description          | Auth     |
|--------|----------------------------------|----------------------|----------|
| GET    | `/api/tasks`                     | Get all or own tasks | Required |
| GET    | `/api/tasks/my`                  | Get my tasks         | Required |
| GET    | `/api/tasks/{id}`                | Get task by ID       | Required |
| GET    | `/api/tasks/project/{projectId}` | Get tasks by project | Required |
| POST   | `/api/tasks`                     | Create task          | Required |
| PUT    | `/api/tasks/{id}`                | Update task          | Required |
| DELETE | `/api/tasks/{id}`                | Delete task          | Required |
| PATCH  | `/api/tasks/{id}/done`           | Mark as done         | Required |
| PATCH  | `/api/tasks/{id}/open`           | Mark as open         | Required |
| POST   | `/api/tasks/{id}/log-time`       | Log time             | Required |
| GET    | `/api/tasks/timer`               | Get active timer     | Required |
| POST   | `/api/tasks/{id}/timer/start`    | Start timer          | Required |
| POST   | `/api/tasks/timer/stop`          | Stop timer           | Required |

### Task Types
| Method | Endpoint                       | Description          | Auth       |
|--------|--------------------------------|----------------------|------------|
| GET    | `/api/tasktypes`               | Get all task types   | Required   |
| POST   | `/api/tasktypes`               | Create task type     | Admin only |
| PUT    | `/api/tasktypes/{id}`          | Update task type     | Admin only |
| DELETE | `/api/tasktypes/{id}`          | Deactivate task type | Admin only |
| PATCH  | `/api/tasktypes/{id}/activate` | Activate task type   | Admin only |

### Notifications
| Method | Endpoint                          | Description        | Auth     |
|--------|-----------------------------------|--------------------|----------|
| GET    | `/api/notifications`              | Get my notifications | Required |
| GET    | `/api/notifications/unread-count` | Get unread count   | Required |
| PATCH  | `/api/notifications/mark-all-read`| Mark all as read   | Required |
| PATCH  | `/api/notifications/{id}/read`    | Mark one as read   | Required |

---

## CI/CD Pipeline

GitHub Actions runs automatically on every push to main:

1. Restore NuGet packages
2. Build the solution
3. Run all unit tests
4. Build the Angular frontend

---


## Docker

Run the entire stack with one command:

```bash
docker-compose up --build
```

Services available at:
- Frontend: http://localhost:4500
- Swagger: http://localhost:4500/swagger/index.html

Default credentials:
- Email: admin@taskmanagement.com
- Password: Admin@1234

To stop:
```bash
docker-compose down
```

To stop and remove database:
```bash
docker-compose down -v
```



## Roadmap

- [x] JWT Authentication
- [x] Project management
- [x] Role-based access control
- [x] Task CRUD with time tracking
- [x] Task types (admin managed)
- [x] Real-time timer persisted per user
- [x] Real-time notifications via SignalR
- [x] Domain events with MediatR
- [x] Clean Architecture
- [x] Unit tests
- [x] CI/CD pipeline
- [x] Docker support 

---

## Author

**David Shahu**
- Email: david.shahu@gmail.com
- LinkedIn: [David Shahu](https://linkedin.com/in/david-shahu)
