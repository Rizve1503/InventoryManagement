# Inventory Management Web Application

A production-grade inventory management web application with customizable item templates and dynamic ID formats. This project allows users to define custom "inventories," specify unique fields for items within them, and manage data collaboratively. The system is built on a Clean Architecture using .NET 8, ASP.NET Core MVC, and SQL Server.

## Key Features

-   **User & Role Management:** Secure registration and login system with distinct roles (Admin, Authenticated User, Guest).
-   **Dynamic Inventories:** Users can create and manage their own inventories with custom titles, descriptions, and tags.
-   **Custom Fields Engine:** The first "killer feature" - inventory owners can define a set of custom fields for their items, including:
    -   Single-line text (up to 3)
    -   Multi-line text (up to 3)
    -   Numeric fields (up to 3)
    -   Boolean (checkbox) fields (up to 3)
    -   Document/Image links (up to 3)
-   **Custom ID Generation:** The second "killer feature" - a drag-and-drop interface to build unique ID formats for items within an inventory, using components like:
    -   Fixed Text & Emojis
    -   Random Numbers & GUIDs
    -   Sequential Counters
    -   Date/Time Stamps
-   **Full-Text Search:** A globally accessible search bar to find inventories and items.
-   **Optimistic Concurrency:** Prevents data loss from simultaneous edits by multiple users.
-   **Access Control:** Inventory owners can mark inventories as public or grant write access to specific users.

## Technology Stack

-   **Backend:** C# with .NET 8 & ASP.NET Core MVC
-   **Database:** SQL Server with Entity Framework Core 8
-   **Frontend:** HTML5, CSS3, Bootstrap 5, JavaScript
-   **Architecture:** Clean Architecture (Domain, Application, Infrastructure, Presentation layers)

## Project Structure

The solution is organized into four distinct projects, following the principles of Clean Architecture:

-   `InventoryManagement.Domain`: Contains the core business entities and domain logic, with no external dependencies.
-   `InventoryManagement.Application`: Contains the application logic, services, and interfaces that orchestrate the domain.
-   `InventoryManagement.Infrastructure`: Handles external concerns, primarily database access via EF Core and implementations of services defined in the Application layer.
-   `InventoryManagement.WebApp`: The ASP.NET Core MVC project, responsible for the user interface and presentation layer.

## Getting Started

To run this project locally, you will need:
-   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or another code editor
-   SQL Server (Express, Developer, or LocalDB)

**Steps to Run:**
1.  Clone the repository.
2.  Open the `InventoryManagement.sln` file in Visual Studio.
3.  Ensure the connection string in `InventoryManagement.WebApp/appsettings.json` points to your SQL Server instance.
4.  Open the **Package Manager Console** (`View -> Other Windows -> Package Manager Console`).
5.  Set the `Default project` to `InventoryManagement.Infrastructure` and run `Update-Database` to create the database schema.
6.  Set the `InventoryManagement.WebApp` as the startup project and run it (press F5).