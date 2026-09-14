
# Sports Complex Reservation System

A backend API for booking facilities at a sports complex. Built in C# with ASP.NET Core, EF Core and SQL Server, laid out in Clean Architecture layers.

This was a one-week build in February 2026 — my first project structured this way. The scope is deliberately small: authentication, and the booking flow. It is not a finished product, and this README says what it does and does not do rather than implying more.

---

## Origin

This started as a group project in a backend program, built in teams of two or three. I was behind the rest of my team at the time and didn't contribute much to the team version. Afterwards I took the same specification and built the whole thing again on my own, start to finish. That second version is what's in this repository.

---

## What it does

- **Accounts** — registration and login
- **Authentication** — JWT-based, with authorization on protected endpoints
- **Booking** — [TODO: describe the actual reservation flow in 2–3 lines. e.g. what can be booked — halls? courts? time slots? Can a user see their own bookings? Can they cancel? Is there an admin who manages the facilities?]

---

## Structure

Four projects, dependencies pointing inward:

| Project | Contents |
| --- | --- |
| `ReservationSportsComplex.Domain` | Entities and enums |
| `ReservationSportsComplex.Application` | Service interfaces, DTOs, business logic |
| `ReservationSportsComplex.Infrastructure` | EF Core, repositories, data access |
| `ReservationofSportsComplexProject` | API layer — controllers and configuration |

**Repository pattern** for data access, so the application layer talks to interfaces rather than to `DbContext` directly.

**DTOs with AutoMapper** between the domain entities and the API contracts, so the shape a client sees is not tied to the shape of the database.

**JWT** for authentication. [TODO: one line — are there roles (admin/user), or is every authenticated user the same?]

---

## Running it locally

**Prerequisites:** .NET [TODO: which version — 8? 9?] SDK, SQL Server.

1. Clone the repository and open the solution.
2. Set the connection string and JWT signing key. [TODO: say where — `appsettings.json` or user secrets. If they are currently committed in `appsettings.json`, move them to user secrets before anyone reads this.]
3. Apply migrations.
4. Run, and open Swagger to try the endpoints.

---

## What I'd do differently now

Worth writing down, because most of it I only understood by getting it wrong here first.

**Business rules ended up in the service layer.** The entities in `Domain` are mostly properties — the rules about what makes a booking valid live outside them. That works at this size, but it means nothing stops a caller from putting an entity into an invalid state. In my next project I moved those rules into the entities themselves and made the child methods `internal`, so the application layer physically cannot bypass the aggregate root.

**A generic repository was the wrong default.** Every read path here drags back whole entities whether or not it needs them. Projecting to a DTO inside the repository — selecting only the columns the screen actually uses — turned out to be both simpler and faster.

**No tests.** There is no test project. The booking rules are exactly the kind of thing that should have been pinned down by unit tests, and they weren't.

**`master` instead of `main`,** and an open pull request that was never resolved. Small things, but they are the first thing anyone sees.

I wrote a follow-up project that addresses most of this — an interest-free loan fund API on .NET 10 with Clean Architecture, CQRS and a tested domain layer:
**[QarzAlHasana Management System](https://github.com/mhfaraji13/QarzAlHasana-Management-System)**

---

## Known gaps

- No automated tests
- Business rules are not enforced at the domain level
- [TODO: anything else you know is missing — validation? error handling? pagination? Name it here. Listing your own gaps reads as confidence, not weakness.]
