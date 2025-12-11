# Overview
Flash card application with persistent identity management and role-based authorization.

# Getting Started

## Prerequisites
- .NET 9.0 SDK
- SQL Server LocalDB (or SQL Server instance)

## Initial Setup

1. Clone this repository
```bash
git clone https://dev.azure.com/iw5-2025-team-xjanigd00/project/_git/project
```

2. Navigate to the API project directory
```bash
cd src/Api/Flippit.Api.App
```

3. Update the connection string in `appsettings.json` if needed (default uses LocalDB):
```json
"ConnectionStrings": {
  "IdentityConnection": "Server=(localdb)\\mssqllocaldb;Database=FlippitIdentity;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

4. Apply database migrations
```bash
dotnet ef database update --project ../../IdentityProvider/Flippit.IdentityProvider.DAL
```

5. Run the application
```bash
dotnet run
```

The application will automatically seed test users on startup.

## Test Accounts

The following test accounts are automatically seeded when the application starts (in Development/Production environments):

### Admin Account
- **Username**: `admin`
- **Password**: `Admin123!`
- **Email**: `admin@flippit.local`
- **Role**: Admin
- **Permissions**: Full access to all resources, including ability to delete/modify any user's collections and cards

### Standard User Account
- **Username**: `user`  
- **Password**: `User123!`
- **Email**: `user@flippit.local`
- **Role**: User
- **Permissions**: Can create/modify/delete own collections and cards only

## Authentication

### Register a New User
```bash
POST /api/auth/register
Content-Type: application/json

{
  "userName": "newuser",
  "password": "Password123!",
  "email": "newuser@example.com"
}
```

### Login
```bash
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Admin123!"
}
```

Returns a JWT token that should be included in subsequent requests:
```bash
Authorization: Bearer <token>
```

## Authorization Rules

The application enforces the following authorization rules:

1. **Anonymous Access**: Users can view all collections and cards without authentication
2. **Authenticated Access**: Creating collections and cards requires authentication
3. **Owner-Based Access**: Users can only update or delete their own collections and cards
4. **Admin Override**: Admin users can update or delete any collection or card

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Categories
```bash
# Run authorization tests
dotnet test --filter "FullyQualifiedName~AuthorizationTests"

# Run authentication tests  
dotnet test --filter "FullyQualifiedName~AuthControllerTests"
```

Note: Tests use an in-memory database and do not require SQL Server.

## Database Migrations

### Create a New Migration
```bash
cd src/IdentityProvider/Flippit.IdentityProvider.DAL
dotnet ef migrations add <MigrationName> --startup-project ../../Api/Flippit.Api.App
```

### Apply Migrations
```bash
dotnet ef database update --startup-project ../../Api/Flippit.Api.App
```

### Remove Last Migration
```bash
dotnet ef migrations remove --startup-project ../../Api/Flippit.Api.App
```

# Folder structure
```txt
Flippit.sln
README.md
src/
├── Api/
│   ├── Flippit.Api.App/                    # Web API application
│   ├── Flippit.Api.App.EndToEndTests/      # End-to-end tests for the API
│   ├── Flippit.Api.BL/                     # Business logic layer
│   ├── Flippit.Api.BL.UnitTests/           # Unit tests for business logic
│   ├── Flippit.Api.DAL.Common/             # Common data access layer components
│   ├── Flippit.Api.DAL.EF/                 # Entity Framework data access layer
│   ├── Flippit.Api.DAL.IntegrationTests/   # Integration tests for data access
│   ├── Flippit.Api.DAL.Memory/             # In-memory data access layer
├── Common/
│   ├── Flippit.Common/                     # Shared utilities and enums
│   ├── Flippit.Common.BL/                  # Shared business logic
│   ├── Flippit.Common.Models/              # Shared data models
```
