# .NET Core 3.1 Web API

## Command

### Create new project

```
dotnet new webapi
```

### Entity Framework

- Packages

```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```

- Add migrations and update database

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Auto mapper

- Packages

```
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```
