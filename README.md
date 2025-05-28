# Cookie-Based Authentication in .NET 9

This solution demonstrates cookie-based authentication in .NET 9 for both an API and a Web UI application.

## Project Structure

- **CookieAuth.API**: Backend API service that handles authentication
- **CookieAuth.Web**: Frontend MVC application that communicates with the API
- **CookieAuth.Shared**: Shared models used by both API and Web applications

## Features

- Cookie-based authentication across API and Web UI
- User registration and login functionality
- Protected routes and resources
- Remember me functionality
- Secure cookie configuration
- Cross-Origin Resource Sharing (CORS) configuration

## Running the Solution

You need to run both the API and Web applications to test the full solution:

1. Navigate to the solution directory:
   ```
   cd CookieAuth
   ```

2. Run the API application (in one terminal):
   ```
   cd CookieAuth.API
   dotnet run
   ```
   The API will be running at https://localhost:5001

3. Run the Web application (in another terminal):
   ```
   cd CookieAuth.Web
   dotnet run
   ```
   The Web UI will be running at https://localhost:5002

4. Open your browser and navigate to https://localhost:5002 to access the Web UI.

## Implementation Details

### API (CookieAuth.API)

The API project implements:
- Cookie authentication configuration with `AddCookie()`
- User service for handling registration, login, and user management
- Authentication controller with endpoints for register, login, logout, and user profile
- CORS configuration to allow requests from the Web UI

### Web UI (CookieAuth.Web)

The Web UI project implements:
- Cookie authentication configuration matching the API
- HttpClient service to communicate with the API
- Account controller for registration, login, and logout
- Protected Profile page that requires authentication
- Login status partial view in the navigation bar

### Shared Models (CookieAuth.Shared)

The Shared project contains:
- Login and registration request models
- User profile and authentication result models

## Security Considerations

- Cookies are configured as HttpOnly to prevent JavaScript access
- Secure policy is enabled to ensure cookies are only sent over HTTPS
- SameSite is set to Lax for development (should be Strict in production)
- Password hashing is implemented (though in a real application, use a more robust solution like BCrypt)
- CSRF protection is enabled by default in ASP.NET Core MVC
