Video for user roles : https://youtu.be/Y6DCP-yH-9Q
# VetAppointmentManager API

This ASP.NET Core Web API project is designed to manage veterinary appointments. It includes functionality for creating, viewing, editing, canceling, and declining appointments.

## Features

- **CRUD Operations**: Create, read, update, and delete veterinary appointments.
- **Cancel Appointments**: Specific endpoint to cancel appointments.
- **Decline Appointments**: Specific endpoint to decline appointments with a reason.

## Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or later (recommended for development)
- SQL Server (LocalDB or SQL Express for development)

## Setup

1. **Clone the repository**:

2. **Navigate to the project directory**:

3. **Restore dependencies**:

4. **Update the database**:

5. **Run the application**:


## Configuration

Ensure that the `appsettings.json` file contains the correct connection string for your SQL Server instance:


## Usage

The API provides the following endpoints:

- `GET /api/VetAdmin`: Retrieve all appointments.
- `GET /api/VetAdmin/{id}`: Retrieve a specific appointment by ID.
- `POST /api/VetAdmin`: Create a new appointment.
- `PUT /api/VetAdmin/{id}`: Update an existing appointment.
- `DELETE /api/VetAdmin/{id}`: Delete an appointment.
- `POST /api/VetAdmin/Cancel/{id}`: Cancel an appointment.
- `POST /api/VetAdmin/Decline/{id}`: Decline an appointment with a reason.

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Learn about ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0)

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open issues to improve the documentation or code.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Controller Class for ASP.NET Core with Entity Framework
In ASP.NET Core, when you are working with Entity Framework for handling database operations in a web application, you typically use a controller derived from Controller or ControllerBase. The choice between these two depends on whether you are building an MVC application with views or a Web API.

Here are the typical use cases:

Controller (for MVC applications): If your application serves HTML pages (or Razor Views), you should inherit from Controller. This class provides support for views, so it's suitable for applications that return HTML along with handling API requests.
ControllerBase (for API applications): If your application is a Web API used for backend services typically consumed by client-side applications (like Angular, React, or mobile apps), you should inherit from ControllerBase. This class is optimized for building APIs and does not support returning views.
Example: VetAppointment API Controller
If you are creating a Web API to manage veterinary appointments, you would start by creating a controller that interacts with the database using Entity Framework. Here’s a basic example of what this controller might look like:



using System.Threading.Tasks;
using YourNamespace.Data;  // Adjust the namespace to where your DbContext is located
using YourNamespace.Models; // Adjust the namespace to where your VetAppointment model is located

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VetAppointmentsController : ControllerBase
    {
        private readonly YourDbContext _context;
        public VetAppointmentsController(YourDbContext context)
        {
            _context = context;
        }

        // GET: api/VetAppointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VetAppointment>>> GetAppointments()
        {
            return await _context.VetAppointments.ToListAsync();
        }

        // GET: api/VetAppointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VetAppointment>> GetAppointment(int id)
        {
            var appointment = await _context.VetAppointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // POST: api/VetAppointments
        [HttpPost]
        public async Task<ActionResult<VetAppointment>> PostAppointment(VetAppointment appointment)
        {
            _context.VetAppointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.Id }, appointment);
        }

        // Other methods for PUT, DELETE, etc.
    }
}
Key Points:
DbContext Injection: The controller uses dependency injection to obtain an instance of the DbContext (YourDbContext).
Routing: The [Route] attribute specifies the URL pattern that the controller handles.
CRUD Operations: The controller includes methods for CRUD operations: Create (POST), Read (GET), Update (PUT), and Delete (DELETE).
Asynchronous Programming: All database operations are asynchronous, using async and await for scalability.
