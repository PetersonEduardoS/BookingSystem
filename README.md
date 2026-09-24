# Booking System

Room booking app built with ASP.NET Core 8 and a plain HTML, CSS and JavaScript front end. Users sign up, log in with a JWT and book meeting rooms; administrators manage rooms and users.

## Features

**Every logged-in user**
- Sign up and log in (passwords hashed with BCrypt, JWT authentication)
- View the room list and create new rooms
- Book a room, edit or cancel their own bookings
- Booking rules: the end date must be after the start date, the room must exist and the same room cannot be booked twice for overlapping times

**Administrators**
- See, edit and cancel every booking
- Edit and delete rooms
- Manage users (edit details, reset passwords, change roles, delete)

## Security

- Role-based authorization on every endpoint (`user` and `admin`)
- The booking owner always comes from the JWT, never from the request body
- Public sign-up always creates a regular user; only admins can change roles
- Password hashes are never returned by the API and passwords are never logged
- The JWT signing key is not stored in `appsettings.json`; a dev-only key lives in `appsettings.Development.json` and real keys go in user-secrets or environment variables

## Tech Stack

| Layer | Technologies |
|---|---|
| Back end | C#, .NET 8, ASP.NET Core Web API, Entity Framework Core 8, SQLite, JWT, BCrypt |
| Front end | HTML, CSS, vanilla JavaScript (Fetch API) |
| Tooling | Swagger, EF Core migrations |

## Getting Started

**Prerequisite:** .NET 8 SDK

```bash
git clone https://github.com/PetersonEduardoS/BookingSystem.git
cd BookingSystem
dotnet run
```

- App: https://localhost:7204
- Swagger: https://localhost:7204/swagger

On first run the SQLite database is created, migrations are applied and demo data is seeded: five rooms and an admin account (`admin@bookingsystem.local` / `Admin123!`, from `appsettings.Development.json`).

To use your own JWT key or admin account:

```bash
dotnet user-secrets set "Jwt:Key" "<a random string of at least 32 characters>"
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:Password" "<strong password>"
```

## API Endpoints

| Method | Endpoint | Access |
|---|---|---|
| POST | `/api/Login` | Public |
| POST | `/api/Users` | Public (sign-up) |
| GET, PUT, DELETE | `/api/Users`, `/api/Users/{id}` | Admin |
| GET | `/api/Rooms`, `/api/Rooms/{id}` | Public |
| POST | `/api/Rooms` | Logged-in user |
| PUT, DELETE | `/api/Rooms/{id}` | Admin |
| GET | `/api/Bookings` | Own bookings (admin: all) |
| GET, PUT, DELETE | `/api/Bookings/{id}` | Owner or admin |
| POST | `/api/Bookings` | Logged-in user |

## Project Structure

```
BookingSystem/
├── Controllers/    LoginController, UsersController, RoomsController, BookingsController
├── Models/         User, Room, Booking and request DTOs
├── Data/           AppDbContext and DbSeeder
├── Migrations/     EF Core migrations
├── wwwroot/        Front-end pages (login, register, rooms, bookings, admin pages)
├── appsettings.json
└── Program.cs
```

## Roadmap

- Unit tests for the booking rules and authorization
- GitHub Actions CI

## Author

**Peterson Eduardo Sampaio Silva**
[LinkedIn](https://www.linkedin.com/in/peterson-eduardo-silva) · [GitHub](https://github.com/PetersonEduardoS)
