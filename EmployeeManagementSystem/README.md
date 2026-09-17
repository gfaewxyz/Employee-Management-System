# Employee Management System

A simple C# console application demonstrating a layered architecture for employee management.

## Features

- View employees
- Add employees
- Edit employees
- Delete employees
- Search employees
- JSON file persistence
- Separate Models, Data Service, App Service, and Application projects
- Dependency injection with `Microsoft.Extensions.DependencyInjection`

## Requirements

- .NET 8 SDK
- Visual Studio 2022, VS Code, or another C# IDE

## Project Structure

```text
EmployeeManagementSystem/
├── EmployeeManagement/                 # Application / UI
├── EmployeeManagementAppService/       # Business logic
├── EmployeeManagementDataService/      # Data access
├── EmployeeManagementModels/           # Models
├── data/                               # JSON data
├── EmployeeManagement.sln
└── README.md
```

## Run

Open a terminal in the solution folder and run:

```bash
dotnet run --project EmployeeManagement
```

Or open `EmployeeManagement.sln` in Visual Studio and set **EmployeeManagement** as the startup project.

The application stores employee data in `data/Employees.json`.

## GitHub

```bash
git init
git add .
git commit -m "Initial commit"
```


