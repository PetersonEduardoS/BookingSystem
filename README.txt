Room Reservation System

This project is a complete room reservation system built with ASP.NET Core and SQLite, featuring a web interface using HTML, CSS, and vanilla JavaScript. It allows the management of rooms, reservations, and users, with functionality differentiated between regular users and administrators.

Features

Regular User:
- Register a new room
- Make reservations
- View list of rooms
- View list of reservations
- Logout

Administrator:
- All functions available to regular users
- Manage users (edit and delete)
- Edit or delete rooms

The dashboard dynamically adapts based on the JWT token role (`admin` or `usuario`).

Technologies Used

- ASP.NET Core 6
- Entity Framework Core
- SQLite
- JWT Authentication
- HTML5 + CSS3 + Vanilla JavaScript

How to Run the Project

1. Prerequisites:
- Visual Studio 2022 or newer
- .NET 6 SDK

2. Clone the repository:
```bash
git clone https://github.com/your-username/room-reservation-system.git
cd room-reservation-system
```

3. Apply database migrations:
In the Package Manager Console:
```bash
Update-Database
```

4. Run the project:
Press `Ctrl + F5` or click "Run".

5. Access in your browser:
```
https://localhost:7204
```

Authentication & Authorization

- Authentication is handled using JWT tokens.
- The token payload includes the user's `role` (`admin` or `usuario`).
- Sensitive routes (such as user and room management) are protected using `[Authorize(Roles = "admin")]`.

Project Structure

```
BookingSystem/
├── wwwroot/
│   ├── cadastro.html
│   ├── cadastro-sala.html
│   ├── editar-reserva.html
│   ├── editar-sala.html
│   ├── editar-salas.html
│   ├── editar-usuario.html
│   ├── gerenciar-usuarios.html
│   ├── home.html
│   ├── index.html
│   ├── lista-de-salas.html
│   ├── reserva.html
│   └── reservas.html
├── Controllers/
│   ├── LoginController.cs
│   ├── ReservasController.cs
│   ├── SalasController.cs
│   └── UsuariosController.cs
├── Models/
│   ├── Usuario.cs
│   ├── Sala.cs
│   ├── Reserva.cs
│   ├── LoginRequest.cs
│   └── UsuarioRequest.cs
├── Data/
│   └── AppDbContext.cs
├── Migrations/
├── appsettings.json
└── Program.cs
```

Author

- Developed by Peterson Eduardo S. Silva