This project is a .NET 8.0 Web API using Entity Framework Core in the database-first approach. It implements a RESTful interface for managing devices and employees. The API supports full CRUD operations, device assignment history, and navigation through related objects such as device types, positions, and personal details. The application is structured as a single-project solution. Although it does not use class library separation, folders and naming conventions ensure clean architecture and maintainable code. The project has been configured to run on port 5300 and exposes its endpoints starting with /api/. The Swagger UI is available at https://localhost:5301/swagger.

## How to Configure `appsettings.json`

Create your own `appsettings.json` in the root directory and provide the following:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost,1433;Database=YourDatabase;User Id=sa;Password=YourStrong!Password;"
  }
}
```

Users must configure this file manually after cloning the repository. The DbContext is registered in Program.cs and all relationships between entities are explicitly defined in the OnModelCreating method to mirror the database structure. The API validates input models using data annotations such as [Required] and [StringLength]. HTTP methods and status codes are correctly implemented, and exceptions are handled globally to ensure consistent error responses. This documentation reflects all structural and functional requirements for the project submission.
