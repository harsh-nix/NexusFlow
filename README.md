# NexusFlow

**An enterprise project management and team collaboration platform** — built solo as a summer training / portfolio project, combining the core ideas behind Jira, Trello, and Microsoft Planner into a single, purpose-built application.

NexusFlow lets teams plan projects, assign and track tasks through a real workflow (not just a status dropdown), hold task-level discussions, and see every change to a task recorded in an audit trail.

---

## Why this project

Most student/CRUD project management demos stop at "create, edit, delete." NexusFlow goes further: a task isn't just a row you edit — it has a lifecycle. An assignee explicitly **accepts** it, can **request clarification** from the creator, status changes carry an optional **work log** note, and every action is written to an **audit log** that powers a visible **activity timeline**. That loop — assign → accept → clarify → work → complete, with a record of who did what and when — is the part of real project-management tools that a plain CRUD app doesn't capture.

## Tech Stack

**Frontend**
- Angular 20 (standalone components, signals)
- Angular Material
- Chart.js (dashboard analytics)
- TypeScript

**Backend**
- ASP.NET Core (.NET) / C#
- Entity Framework Core
- JWT Authentication
- Repository Pattern + Unit of Work
- AutoMapper
- FluentValidation
- Swagger / OpenAPI

**Database**
- SQL Server
- Code-first migrations

**Architecture**
- Clean Architecture (Domain → Application → Infrastructure → API)
- Dependency Injection throughout
- Global validation pipeline (FluentValidation via a custom `ValidationFilter`)

## Repository Structure

```
NexusFlow/
├── NexusFlow/          # ASP.NET Core backend (Clean Architecture)
│   ├── NexusFlow.API/            # Controllers, DI wiring, Swagger
│   ├── NexusFlow.Application/    # Services, DTOs, Validators, AutoMapper profiles
│   ├── NexusFlow.Domain/         # Entities, enums, interfaces
│   └── NexusFlow.Infrastructure/ # EF Core, repositories, migrations
├── nexusflow-ui/        # Angular 20 frontend
│   └── src/app/
│       ├── core/                 # services, models, guards
│       ├── features/             # auth, dashboard, projects, tasks
│       ├── layout/                # shell (toolbar, nav, notifications)
│       └── shared/                # reusable components (dialogs, etc.)
└── Documents/           # SRS, diagrams, and other submission documentation
```

## Core Modules

| Module | Status |
|---|---|
| Authentication (JWT, register/login) | ✅ |
| Project Management (CRUD, members, progress tracking) | ✅ |
| Task Management (CRUD, priority, due dates, assignees) | ✅ |
| **Task Workflow** — Accept, Status transitions with work-log notes | ✅ |
| **Clarification flow** — request/respond on a task, role-checked | ✅ |
| **Activity Timeline** — per-task audit trail | ✅ |
| Comments / Discussion threads | ✅ |
| Notifications (incl. click-through to the related task) | ✅ |
| Dashboard analytics (Chart.js) | ✅ |
| My Tasks (cross-project, assigned-to-me view) | ✅ |
| Reports, Admin Panel, File Manager, Meetings | 🔜 planned |

## Getting Started

### Backend
```bash
cd NexusFlow/NexusFlow.API
dotnet restore
dotnet ef database update --project ../NexusFlow.Infrastructure
dotnet run
```
Swagger UI is available at `/swagger` once running.

### Frontend
```bash
cd nexusflow-ui
npm install
ng serve
```
The app expects the API URL configured in `src/environments/environment.ts`.

## Project Status

This repository reflects an in-progress solo build, developed and reviewed iteratively. The core project/task/collaboration loop described above is implemented end-to-end across backend and frontend. Broader modules from the original scope (Organization Management, File Manager, Meetings, Reports, full Admin Panel, Analytics) are on the roadmap and not yet built.

## Author

**Harsh Mishra** ([@harsh-nix](https://github.com/harsh-nix))
