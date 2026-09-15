# Sports Complex Reservation System

A backend API for booking time slots at a sports complex — futsal courts, a pool, a martial arts hall. Members register, top up a wallet, book a session, and cancel for a refund. Built in C# on .NET 8 with ASP.NET Core, EF Core and SQL Server, organised in Clean Architecture layers.

Built in a week in February 2026. It works end to end, and the gaps I know about are listed at the bottom rather than left to be found.

---

## Origin

This began as a group project in a backend program, built in teams of two or three. At the time I was behind the rest of my team and contributed little to the version we submitted. Afterwards I took the same specification and rebuilt the whole thing on my own, from an empty solution. That solo version is what's in this repository.

---

## What it does

**Register and log in.** Each user gets a per-user salt from `RandomNumberGenerator` and a hashed password; logging in returns a JWT carrying the user's id, name and role. A wallet is created alongside the account.

**Browse halls and slots.** Each `SportHall` has a type, a capacity and a price. Slots are generated a day at a time: 90-minute sessions from 08:00 to 22:00, with a 15-minute gap between them for turnover.

**Book a slot.** The wallet is debited, the slot's registration count goes up, and a confirmed booking is written with the price copied onto it — so if the hall's price changes later, past bookings still say what they actually cost.

**See your own bookings.** Projected straight into a DTO inside the query, so only the columns the response needs come back from the database.

**Cancel and get refunded.** Releasing the slot, refunding the wallet and marking the booking cancelled all happen inside one transaction with a rollback, because a cancellation that refunds the money but leaves the slot occupied is worse than one that fails outright.

**Admin endpoints** for creating and updating halls and generating slots, separated by role rather than by a flag on the user.

---

## Structure

| Project | Contents |
| --- | --- |
| `ReservationSportsComplex.Domain` | `User`, `Wallet`, `SportHall`, `TimeSlot`, `Booking`, enums |
| `ReservationSportsComplex.Application` | Service and repository interfaces, DTOs, AutoMapper profiles, `ReservationService` |
| `ReservationSportsComplex.Infrastructure` | `ApplicationDbContext`, EF configurations, migrations, repositories, JWT and password hashing |
| `ReservationSportsComplex.API` | Controllers, DI wiring, JWT and Swagger configuration |

`Domain` references nothing. Everything else points inward at it.

The application layer depends on **`IApplicationDbContext`** — an interface exposing the `DbSet`s, `SaveChangesAsync` and `BeginTransactionAsync` — rather than on `DbContext` itself. The interface lives in `Application`, the implementation in `Infrastructure`, so the dependency points the right way while each query still gets to write its own `Include`s instead of going through a generic `GetById` that would either hide them or return the whole graph.

Money columns are mapped to `decimal(18,2)` explicitly rather than left to EF's default.

---

## Two decisions worth explaining

**Authorization reads the user id from the token, never from the request.** Every endpoint that touches someone's data pulls the id out of the `NameIdentifier` claim. `CancelBooking` then filters on `b.Id == bookingId && b.UserId == userId` in the query itself, not in an `if` afterwards — so a booking belonging to someone else doesn't come back at all, rather than coming back and being rejected. Same result, smaller window to get wrong.

**Cancellation is transactional, booking isn't yet.** Cancelling touches three rows that have to agree with each other, so it's wrapped and rolled back on failure. Creating a booking touches the same number of rows and should be wrapped the same way — that's in the gaps below.

---

## Running it locally

**Prerequisites:** .NET 8 SDK and SQL Server.

1. Clone the repository and open the solution.
2. Put the connection string and the JWT secret in user secrets on the API project, under `ConnectionStrings:DefaultConnection` and `JwtSettings`.
3. Apply the EF Core migrations to create the database.
4. Run the API. Swagger comes up in Development — register, call `Auth/Login`, and paste the token into **Authorize** before using the protected endpoints.

---

## Known gaps

Listed on purpose. Some I found while writing this, some while building the project that came after.

**Passwords are hashed with SHA-256.** The salt is per-user and cryptographically generated, which is the part people usually get wrong — but SHA-256 is designed to be fast, and fast is exactly what a password hash must not be. This needs PBKDF2, bcrypt or Argon2. It's the first thing on the list.

**Capacity isn't enforced when booking.** `CheckAvailabilityAsync` exists and is correct, and `CreateBookingAsync` never calls it — only the wallet balance is checked. A full slot will still accept bookings. No exception, no error, just a wrong row.

**`IsReserved` is never set to true.** The column is written as `false` when slots are generated and set back to `false` on cancellation, and nothing in between ever flips it. It's a field that currently means nothing.

**`TimeSlot.Price` is ignored.** It exists on the entity, but booking charges `SportHall.Price`, so a slot-specific price would be silently dropped.

**Concurrent bookings can oversell a slot.** `CurrentRegistrations` is read, incremented and saved with no concurrency token, so two simultaneous requests can both start from the same value.

**Creating a booking isn't wrapped in a transaction,** unlike cancelling it.

**Rules live in services and controllers, not in the entities.** `Booking` and `TimeSlot` are property bags; nothing stops code elsewhere from putting them into a state that shouldn't exist. In the project I built next I moved the rules into the entities and made the child methods `internal`, so the application layer physically cannot bypass the aggregate root — enforced by the compiler instead of by a reviewer noticing.

**Failures throw bare `Exception`,** so a caller can't tell "slot not found" from "insufficient balance" without reading the message text.

**`DateTime.Now` rather than `DateTime.UtcNow`** for booking timestamps and token expiry.

**No tests.** Slot generation and the availability check are pure enough to test without a database, which makes their absence harder to excuse rather than easier.

---

## The follow-up

Most of the architectural items above are addressed in the project I built next — an API for an interest-free community loan fund, on .NET 10, with CQRS, business rules that live inside the domain entities, and unit tests over the arithmetic where being quietly wrong costs someone money:

**[QarzAlHasana Management System](https://github.com/mhfaraji13/QarzAlHasana-Management-System)**
