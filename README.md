# Eventra

Eventra is a full-stack event management web application built with ASP.NET Core MVC (.NET 10) and Entity Framework Core. It supports three distinct user roles — Guest, Organizer, and Admin — each with a dedicated set of features for discovering, managing, and reviewing events.

---

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 10)
- **ORM:** Entity Framework Core
- **Database:** SQL Server LocalDB
- **Authentication:** Custom session-based (no ASP.NET Identity middleware)
- **Password Hashing:** `PasswordHasher<User>` from `Microsoft.AspNetCore.Identity`
- **QR Codes:** QRCoder library
- **Architecture:** Repository + Service pattern

---

## Prerequisites

Before running the project, make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **SQL Server LocalDB** — included with Visual Studio 2022 (any edition including Community)
- **Visual Studio 2022** (recommended) or **Visual Studio Code** with the C# Dev Kit extension

---

## Running the Project

1. Clone or download the repository.
2. Open the solution in Visual Studio 2022.
3. Check that the connection string in `appsettings.json` points to your LocalDB instance (default: `(localdb)\\MSSQLLocalDB`).
4. Run the project (`F5` or `dotnet run`).
5. On first launch, Entity Framework Core automatically applies all migrations and the `DbSeeder` populates the database with demo data — no manual steps required.

---

## Seeded Demo Accounts

The following accounts are created automatically on first run. Password changes and password reset are disabled for all demo accounts.

### Admin

| Field    | Value                  |
|----------|------------------------|
| Email    | admin@eventra.com      |
| Username | admin                  |
| Password | Admin2026              |

### Organizers

| Account            | Email                   | Username           | Password   |
|--------------------|-------------------------|--------------------|------------|
| Eventra Studios    | eventra@gmail.com       | EventraStudios     | Eventra1   |
| The Lobby Restaurant | thelobby@gmail.com    | TheLobbyRestaurant | Thelobby1  |
| Mayfair 39         | mayfair@gmail.com       | Mayfair39          | Mayfair1   |

### Guest

| Field    | Value                      |
|----------|----------------------------|
| Email    | studentbriceag@test.com    |
| Username | StudentBriceag             |
| Password | Student1                   |

> Additional guest accounts seeded for demo data: `andreea`, `radu`, `bianca` — all with password `Test1234`.

---

## Features

### Authentication

- **Sign In** with email or username.
- **Register** as a Guest (immediately active) or as an Organizer (requires admin approval before the account is activated).
- Organizers who have not yet been approved are redirected to a "Pending Approval" page on login.
- Organizers whose request was rejected are redirected to a "Rejected Account" page.
- **Forgot Password / Reset Password** flow — looks up the account by email or username and allows setting a new password. Disabled for seeded demo accounts.
- **Logout** clears the session.

---

### Home Page

- Displays featured events and a testimonials section with approved organizer reviews.

---

### Event Browsing (All Users)

- **Browse all approved events** on the Events listing page.
- **Filter** by category, city, free/paid entry.
- **Search** by event title keyword.
- **Event detail page** shows full information: date, time, location, price, available seats, organizer, and the current registration status of the logged-in user.
- **Favorite count** is displayed per event and updates live via AJAX when toggled.

---

### Event Registration (Guests)

- Register for any approved event from the detail page. Requires being signed in.
- If the event is full, the guest is automatically added to the **waiting list** at the next available position.
- If a spot becomes available, the next person on the waiting list is promoted automatically.
- **Cancel registration** from the profile page or the event detail page.
- **Cancel waiting list** from the profile page or the event detail page.
- Past or in-progress events cannot be registered for.

---

### Favorites (Guests & Organizers)

- Toggle any event as a favorite using the heart button on the listing page or detail page.
- The favorite count updates instantly without a full page reload (AJAX).
- Favorite events appear on the guest's profile page.
- Unauthenticated users are redirected to sign in when attempting to favorite.

---

### Event Management (Organizers)

- **Create Event**: Fill in title, description, date, start/end time, city, location, address, capacity, price, currency, category, and upload a cover image. Submitted events are set to `PendingApproval` and must be approved by the admin before they appear publicly. A confirmation page is shown after submission.
- **Edit Event**: Update any field or replace the cover image. Editing re-submits the event for approval.
- **Delete Event**: Permanently removes the event and all associated registrations.
- Only the organizer who owns the event can edit or delete it.
- Pending (unapproved) organizer accounts cannot create events.

---

### QR Check-In (Organizers)

- Each registration generates a unique QR token stored with the registration record.
- Organizers access a **Check-In panel** for any of their events.
- Paste or scan a QR token to mark a guest as checked in.
- The panel shows the running list of all attendees who have been checked in, including their name and check-in time.
- Invalid, already-used, or unrelated QR tokens are rejected with a clear error message.

---

### Profile Page (All Roles)

- View and **edit profile**: first name, last name, email, phone number, bio, and profile photo upload.
- **Change password** (disabled for demo accounts).
- **QR code** generation: each user has a QR code that links to their public profile page.

#### Guest Profile Sections

- **Notifications** — list of all notifications with individual delete and "delete all" options.
- **My Reviews** — reviews the guest has submitted, with star rating, comment, submission date, approval status badge (Approved / Pending / Flagged), and an Edit button.
- **Upcoming Events** — future events the guest is registered for, with a Cancel Registration button.
- **Waiting List** — events the guest is waiting for, showing queue position and a Cancel button.
- **Past Events** — last 6 attended events (events that have already passed).
- **Favorites** — events saved as favorites.

#### Organizer Profile Sections

- **Notifications** — same as guest.
- **My Events** — all events created by the organizer with their current status (Approved / Pending).
- **Reviews** — approved reviews left by guests for this organizer.

---

### Public Profile View

- Any user's profile can be viewed publicly at `/Profile/View/{id}`.
- When an organizer views another user's profile, they can see which of their own upcoming events that user is registered for, on the waiting list for, or already checked in to.

---

### Reviews (Guests Only)

- Guests can submit a review for any event they are registered for.
- Admins and organizers cannot submit reviews.
- **Auto-approval**: reviews that do not contain flagged words are approved immediately and appear publicly. Reviews containing inappropriate words are marked as flagged and held for admin moderation.
- **Edit review**: guests can update a submitted review at any time from the My Reviews section on their profile.
- After submitting or editing a review, the corresponding "Leave a Review" notification is automatically removed.

---

### Notifications

Notifications appear on the profile page and are created automatically in the following cases:

| Type                  | Trigger                                                                 |
|-----------------------|-------------------------------------------------------------------------|
| `EventApproved`       | Admin approves an organizer's event — sent to the organizer.            |
| `ReviewReminder`      | Guest visits their profile after attending an event they have not reviewed yet — created once per event. |
| `NewEventFromOrganizer` | A new event is published by an organizer the guest has interacted with. |

Guests can delete individual notifications or clear all at once.

---

### Admin Panel

The admin panel is accessible only to the Admin account at `/Admin`.

#### Dashboard

Displays live counts for:
- Pending organizer approval requests
- Pending events awaiting approval
- Pending reviews (clean but unmoderated)
- Flagged reviews (contain inappropriate content)

#### Organizer Approval

- Lists all organizer registration requests with submitted business details.
- **Approve**: activates the organizer account so they can log in and create events.
- **Reject**: marks the request as rejected; the organizer sees a rejection page on next login attempt.

#### Event Approval

- Lists all events submitted with `PendingApproval` status.
- **Approve**: publishes the event so it appears in the public listing.
- **Reject**: permanently removes the event.

#### Review Moderation

- Lists all pending (clean but not yet approved) and flagged reviews separately.
- **Approve**: makes the review publicly visible on the organizer's profile.
- **Delete**: permanently removes the review.

#### Admin Profile Settings

- Update the admin email address.
- Update the admin password.

---

## Project Structure

Controllers/       — AccountController, AdminController, EventsController, ProfileController, ReviewsController
Data/              — ApplicationDbContext, DbSeeder
Middleware/        — Custom request pipeline middleware
Models/            — Entity models and ViewModels
Repositories/      — Generic Repository<T> base + specific interfaces and implementations
Services/          — Business logic layer (IEventService, IProfileService, IReviewService, etc.)
Views/             — Razor views per controller
wwwroot/           — Static files: CSS, images, uploaded event covers and profile photos



---

## Database

The database is created and seeded automatically on first run via `DbSeeder.SeedAsync`. The seeder is idempotent — running the application multiple times will not create duplicate records.

Seeded demo data includes:
- 3 approved organizer accounts with profile photos
- 1 pending organizer request (TestOrganizer) for admin approval demo
- 4 guest accounts with registrations, favorites, waiting list entries, and notifications
- 17 events across 9 categories (Food & Drinks, Technology, Conference, Exhibition, Market, Workshop, Festival, Networking, Social)
- 1 pending event (Test Event) for admin approval demo
- 1 pending review (StudentBriceag for April Startup Night) for admin moderation demo
- Organizer notifications (EventApproved) and guest notifications (ReviewReminder, NewEventFromOrganizer)