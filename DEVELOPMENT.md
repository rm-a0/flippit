# Flippit - Development Setup Guide

## Overview
This document provides instructions for running the Flippit application locally with the API and Web App.

## Prerequisites
- .NET 9.0 SDK
- Modern web browser with localStorage support

## Default Seed Users

The application automatically seeds test users when running in Development mode:

### Regular User
- **Username**: `user`
- **Password**: `User123!`
- **Email**: `user@flippit.local`
- **Role**: User
- **Collections**: Has 2 pre-seeded collections (Geography Basics, Math Fundamentals)

### Admin User
- **Username**: `admin`  
- **Password**: `Admin123!`
- **Email**: `admin@flippit.local`
- **Role**: Admin
- **Collections**: Has 1 pre-seeded collection (Admin's Collection)

## Running the Application

### 1. Start the API

```bash
cd src/Api/Flippit.Api.App
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5242`
- HTTPS: `https://localhost:7175`

You can test the API at: `http://localhost:5242/swagger` or `https://localhost:7175/swagger`

### 2. Start the Web App

```bash
cd src/Web/Flippit.Web.App
dotnet run
```

The Web App will start on:
- HTTP: `http://localhost:5240`
- HTTPS: `https://localhost:7267`

## Configuration

### API Configuration (`src/Api/Flippit.Api.App/appsettings.json`)

The API is configured with:
- **JWT Settings**:
  - Key: Development key (change for production!)
  - Issuer: `FlippitApi`
  - Audience: `FlippitClient`
  - Expiration: 60 minutes
  
- **CORS**: Allows requests from `https://localhost:7267` (Web App)

### Web App Configuration (`src/Web/Flippit.Web.App/wwwroot/appsettings.json`)

The Web App is configured to call the API at:
- **BaseUrl**: `http://localhost:5242`

> **Note**: The application is configured to use HTTP for local development to avoid certificate issues. For production deployments, use HTTPS with proper SSL certificates.

## Testing the Authentication Flow

### 1. Login
1. Navigate to the Web App at `https://localhost:7267`
2. Click "Log in"
3. Enter username: `user` and password: `User123!`
4. You should be logged in and see your collections

### 2. Register
1. Navigate to the Web App
2. Click "Sign up"
3. Fill in the registration form:
   - Name: Your name
   - Username: Choose a username
   - Password: Must meet requirements (min 6 chars, uppercase, lowercase, digit, special char)
   - Email: Optional
4. After successful registration, you'll be automatically logged in

### 3. View Collections
- Once logged in, your collections will be displayed on the home page
- The seed user has 2 collections pre-loaded
- New users will have no collections initially

## HTTPS Certificate Issues

If you encounter HTTPS certificate errors:

### For the API:
```bash
dotnet dev-certs https --trust
```

### Alternative: Use HTTP
You can configure the application to use HTTP instead:

1. Update `src/Web/Flippit.Web.App/wwwroot/appsettings.json`:
   ```json
   {
     "Api": {
       "BaseUrl": "http://localhost:5242"
     }
   }
   ```

2. Update `src/Api/Flippit.Api.App/Program.cs` CORS policy to allow `http://localhost:5240`:
   ```csharp
   policy.WithOrigins("http://localhost:5240")
   ```

3. Run both applications with HTTP only:
   ```bash
   # API
   dotnet run --urls "http://localhost:5242"
   
   # Web App  
   dotnet run --urls "http://localhost:5240"
   ```

## Architecture

### Authentication Flow
1. User logs in via `/api/auth/login`
2. API validates credentials and returns JWT token
3. Web App stores token in localStorage
4. All subsequent API requests include `Authorization: Bearer {token}` header
5. Token is automatically restored on page refresh

### Data Flow
1. Identity users are stored in in-memory Entity Framework (IdentityProviderDbContext)
2. Application data (Users, Collections, Cards) is stored in in-memory repositories
3. DataSeeder links Identity users with application Users using matching GUIDs

## Troubleshooting

### Login fails with "unauthorized"
- Verify the password meets the requirements
- Check the API logs for detailed error messages
- Ensure the API is running

### Collections not showing
- Verify you're logged in (check browser console for JWT token)
- Check browser developer tools Network tab for API request/response
- Ensure Authorization header is being sent with requests

### CORS errors
- Verify the Web App origin matches the CORS policy in API `Program.cs`
- Check that both protocols (HTTP/HTTPS) match between client and server

## Development Notes

- The application uses in-memory databases that reset on restart
- JWT tokens expire after 60 minutes
- LocalStorage persists across page refreshes but not browser sessions in incognito mode
- API clients are auto-generated from OpenAPI spec
