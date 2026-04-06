# Eventra

Eventra is a database-driven ASP.NET Core web application for discovering, creating, and managing events. Users can browse events, manage their profiles, and access personalized features, while organizers can create and publish their own events.

## Tech Stack

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server
- HTML
- CSS
- JavaScript

## Application Functionality

The application is designed to include the following functionality:

- user registration and authentication
- role-based access for Guests and Organizers
- admin approval for organizer accounts before publishing events
- event browsing and discovery using filters such as date, location, city, category, or popularity
- dedicated event details pages with title, date, location, capacity, entry type, and description
- event registration for guests
- waiting list system for full events, with automatic queue progression when spots become available
- personalized user profile with editable personal information
- QR code generation for each registered user, used as a digital ticket
- QR-based event check-in for organizers
- event creation and publishing for approved organizers
- notifications related to registrations, waiting list status, and future events
- admin management for organizers and published events

## Running the Project

To run this project locally, make sure you have:
- .NET installed
- Visual Studio with the ASP.NET and web development workload
- Microsoft SQL Server Management Studio (SSMS) installed
- Microsoft SQL Server installed and configured locally

Then:
1. Clone the repository
2. Open the solution in Visual Studio
3. Update the database connection string if needed
4. Run the database migrations
5. Run the application

## Testing

You can test the platform using one of the demo accounts seeded in `DbSeeder`.  
The account credentials are also available in the `Accounts.txt` file included in the project repository.

You can also create your own account and add events to your local database for a more complete testing experience.

## Current Status

The project follows the same core structure as the frontend version, with backend functionality added progressively.  

---
Developed as part of a web application project.
