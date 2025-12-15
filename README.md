# TaskApp - Task Management API

A RESTful API for managing tasks built with .NET Core 6, Entity Framework Core, and SQLite.

## Prerequisites

Following tools are required to run this project:

### Required Tools

1. **.NET 6 SDK** (version 6.0 or higher)`

### Optional Tools
- **Entity Framework Core CLI Tools** (Needed if there are issues with database)
   ```bash
   dotnet tool install --global dotnet-ef
   ```
- **REST Client** (VS Code extension) - For using .http files

## Project Structure
The project is divided into four layers:
1. Api
2. Domain
3. Infrastructure
4. Tests

## Database Setup

The application uses SQLite, so no separate database server installation is needed. `Tasks.db` SQLite databases file is included. If there are any issues, refer to the troubleshooting information.


## Running the Application Using Visual Studio

1. Open `TaskApp.sln`
2. Set `TaskApp.Api` as the startup project
3. Build project
4. Hit Run

The API will be available at:
- **HTTPS**: `https://localhost:7213`
- **HTTP**: `http://localhost:5090`
- **Swagger UI**: `https://localhost:7213/swagger`

## Running Tests

```bash
dotnet test src/TaskApp.Tests/TaskApp.Tests.csproj --logger "console;verbosity=detailed"
```

## Using the .http File

The project includes a `tasks.api.http` file for quick API testing. To use it:

1. If using VS Code, install the REST Client extension
2. Open `tasks.api.http`
3. Click "Send Request" above any HTTP request

## Troubleshooting

### Database Issues

If you encounter database errors:

```bash
# Remove existing database
rm src/TaskApp.Api/Tasks.db

# Recreate database
dotnet ef database update --project ./src/TaskApp.Infrastructure --startup-project ./src/TaskApp.Api
```

