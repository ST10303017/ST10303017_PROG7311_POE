# Agri-Energy Connect - Part 2 Prototype

## Project Overview
This ASP.NET Core 8 MVC application is a prototype for the Agri-Energy Connect platform, fulfilling Part 2 of the POE. It features a user management system with distinct "Farmer" and "Employee" roles, product management for farmers, and administrative functions for employees, all secured with ASP.NET Core Identity.

**Project Namespace:** `ST10303017_PROG7311_POE`
**GitHub Repository Link:** `https://github.com/ST10303017/ST10303017_PROG7311_POE.git`

## Core Implemented Features
*   **User Roles & Secure Authentication (ASP.NET Core Identity):**
    *   Distinct "Farmer" and "Employee" roles.
    *   Secure login required for access to application features.
*   **Employee Functionality:**
    *   Self-registration (users automatically become "Employees").
    *   Add new Farmer user profiles.
    *   View products from any Farmer, with filtering by date range and product type.
*   **Farmer Functionality:**
    *   Accounts created by Employees.
    *   Add new products (name, category, production date).
    *   View their own product list.
*   **Database (SQLite & EF Core):**
    *   Uses SQLite, managed via Entity Framework Core (Code-First).
    *   Database file (`AgriEnergyConnect.db`) included in `/LocalDatabase` (copied to output on build).
    *   Initial seeding:
        *   "Employee" and "Farmer" roles.
        *   5 sample Employee accounts.
        *   5 sample Farmer accounts.
        *   5 sample products for each sample Farmer.
*   **UI:** Styled with Bootstrap 5 and custom CSS for a modern, responsive design.

## Technologies
*   ASP.NET Core MVC (.NET 8.0), C#
*   Entity Framework Core 8
*   ASP.NET Core Identity
*   SQLite
*   Bootstrap 5, HTML, CSS

## Prerequisites
*   .NET SDK 8.0
*   Visual Studio 2022 (with "ASP.NET and web development" workload)

## Setup and Execution

1.  **Open Solution:** Open `ST10303017_PROG7311_POE.sln` in Visual Studio 2022.
2.  **Connection String:** Verified in `appsettings.json` for the included SQLite DB:
    `"DefaultConnection": "Data Source=LocalDatabase\\AgriEnergyConnect.db"`
3.  **Apply Migrations:**
    *   In Package Manager Console (Default project: `ST10303017_PROG7311_POE`):
        ```powershell
        Update-Database
        ```
    *   This creates/updates the `AgriEnergyConnect.db` schema in the output directory.
4.  **Run Application:**
    *   Press F5 or Debug > Start Debugging.
    *   Data seeding occurs on first run after database setup.

## Initial User Accounts & Usage

**Employee Accounts:**
*   5 seeded. Primary: `employee1@agrienergy.com` (Password: `Password123!`)
*   New employees can register via the homepage link.
*   **Tasks:** Add Farmers, view/filter all products.

**Farmer Accounts (Pre-Seeded):**
*   5 seeded, each with 5 sample products.
    1.  `farmer.john.doe@example.com` (Password: `FarmerPass1!`)
    2.  `farmer.jane.smith@example.com` (Password: `FarmerPass2!`)
    3.  `farmer.peter.fields@example.com` (Password: `FarmerPass3!`)
    4.  `farmer.susan.grower@example.com` (Password: `FarmerPass4!`)
    5.  `farmer.mike.valley@example.com` (Password: `FarmerPass5!`)
*   **Tasks:** Add/view their own products.

## Key Project Folders
*   **`/Controllers`**: MVC controllers.
*   **`/Data`**: `ApplicationDbContext.cs`, `SeedData.cs`.
*   **`/Models`**: Entities & ViewModels.
*   **`/Views`**: Razor UI files.
*   **`/Areas/Identity`**: Identity UI pages.
*   **`/wwwroot`**: Static assets (CSS, images).
*   **`/LocalDatabase`**: Contains `AgriEnergyConnect.db`.
*   **`Program.cs` / `appsettings.json`**: Core configuration.

**References:**
- Troelsen, A., & Japikse, P. (2022). Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming. New York: Apress Media LLC.
- OpenAI. (2024, March 21). ChatGPT. Retrieved from ChatGPT: https://chat.openai.com/
- Disclosure of Al Usage in my Assignment
• Section(s) within the assignment in which generative Al was used, e.g.
Question: Readme,
• Tool used is ChatGPT
• Purpose was for brainstorming and structuring only
• Used on 11 May 2024
• https://chat.openai.com/
