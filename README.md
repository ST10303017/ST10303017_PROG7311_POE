# Agri-Energy Connect

## Project Overview
This MVC application serves as a functional prototype for the Agri-Energy Connect platform, developed in fulfillment of the requirements for Part 2 of the Programming 7311 Portfolio of Evidence. The prototype demonstrates a user management system employing distinct roles (Farmer and Employee) and showcases core functionalities pertaining to product management by farmers, alongside farmer and product administration by employees. The system utilizes ASP.NET Core Identity for robust authentication and authorization.

**Project Namespace:** `ST10303017_PROG7311_POE`

**GitHub Repository Link:** `https://github.com/ST10303017/ST10303017_PROG7311_POE.git`


## Implemented Key Features

*   **User Role Management & Secure Authentication:**
    *   Implementation of distinct "Farmer" and "Employee" user roles, managed via ASP.NET Core Identity.
    *   Secure login functionality is enforced for both roles; application data and features are accessible only to authenticated users based on their assigned roles.
*   **Employee-Specific Functionalities:**
    *   Self-registration capability for Employees; newly registered users are automatically assigned the "Employee" role.
    *   Authenticated Employees possess the ability to create and add new Farmer user profiles to the system.
    *   Employees can view a comprehensive list of products associated with any specific Farmer.
    *   Product listings are equipped with filtering capabilities based on date range and product type.
*   **Farmer-Specific Functionalities:**
    *   Farmer accounts are created by authorized Employees. Once logged in, Farmers can:
        *   Add new products (specifying name, category, and production date) to their individual profiles.
        *   View a consolidated list of their own product offerings.
*   **Database and Data Seeding:**
    *   The application utilizes **SQLite** as its database, managed through Entity Framework Core (employing a Code-First approach).
    *   The SQLite database file (`AgriEnergyConnect.db`) is included within the `LocalDatabase` directory of the project source and is configured to be copied to the build output directory.
    *   An initial data seeding process on application startup (after database schema creation) populates the database with:
        *   The "Employee" and "Farmer" roles.
        *   Five sample Employee accounts.
        *   Five sample Farmer accounts.
        *   Five distinct sample products for each of the five sample Farmers (totaling 25 pre-seeded products).
*   **User Interface and Design:**
    *   The user interface is styled using Bootstrap 5, augmented with custom CSS to achieve a modern, clean, and responsive user experience.

## Core Technologies Utilized
*   ASP.NET Core MVC (.NET 8.0), C#
*   Entity Framework Core 8
*   ASP.NET Core Identity
*   SQLite
*   Bootstrap 5, HTML, CSS

## System Prerequisites
*   .NET SDK 8.0
*   Visual Studio 2022 (with the "ASP.NET and web development" workload installed).

## Setup and Execution Instructions

1.  **Open the Solution:** Launch Visual Studio 2022 and open the `ST10303017_PROG7311_POE.sln` file located in the project's root directory.
2.  **Verify Connection String:**
    *   The application is pre-configured to use an included SQLite database file. The connection string in `appsettings.json` is:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Data Source=LocalDatabase\\AgriEnergyConnect.db"
        }
        ```
3.  **Apply Database Migrations:**
    *   Open the **Package Manager Console** within Visual Studio (View > Other Windows > Package Manager Console).
    *   Ensure that `ST10303017_PROG7311_POE` is selected as the "Default project" in the Package Manager Console dropdown.
    *   Execute the following command to create the database schema from migrations:
        ```powershell
        Update-Database
        ```
    *   This command will create the `AgriEnergyConnect.db` file in the appropriate output directory
4.  **Run the Application:**
    *   Press **Debug > Start Debugging** from the Visual Studio menu to build and run the application.
    *   The application will launch in your default web browser. The data seeding process (roles, users, products) will occur automatically.

## Initial User Accounts & Application Usage

The application includes pre-seeded user accounts for demonstration and testing:

**Employee Accounts:**
*   A total of five Employee accounts are seeded. The primary one for initial access is:
    *   **Email/Username:** `employee1@agrienergy.com`
    *   **Password:** `Password123!`
*   **Registration:** Additional Employees can register via the "Register Here" link on the homepage. They will automatically be assigned the "Employee" role.
*   **Primary Tasks:** Log in to add Farmer accounts, view all products, and filter product listings.

**Farmer Accounts (Pre-Seeded):**
*   Five Farmer accounts are pre-seeded with login credentials. Each farmer also has 5 sample products.
    1.  **Email/Username:** `farmer.john.doe@example.com`
        **Password:** `FarmerPass1!`
    2.  **Email/Username:** `farmer.jane.smith@example.com`
        **Password:** `FarmerPass2!`
    3.  **Email/Username:** `farmer.peter.fields@example.com`
        **Password:** `FarmerPass3!`
    4.  **Email/Username:** `farmer.susan.grower@example.com`
        **Password:** `FarmerPass4!`
    5.  **Email/Username:** `farmer.mike.valley@example.com`
        **Password:** `FarmerPass5!`
*   **Primary Tasks:** Log in to add new products to their profile or view their existing product list.


## Project Structure Overview
*   **`/Controllers`**: Contains the MVC controllers responsible for handling application logic.
*   **`/Data`**: Includes `ApplicationDbContext.cs` for Entity Framework Core and `SeedData.cs` for initial database population.
*   **`/Migrations`**: Stores Entity Framework Core database migration files specific to SQLite.
*   **`/Models`**: Defines C# entity classes (e.g., `ApplicationUser.cs`, `Product.cs`) and ViewModels.
*   **`/Views`**: Contains Razor (.cshtml) files for rendering the user interface.
    *   **`/Views/Shared`**: Houses shared layout components like `_Layout.cshtml`.
*   **`/Areas/Identity/Pages`**: Contains the scaffolded Razor Pages for ASP.NET Core Identity functionalities.
*   **`/wwwroot`**: Stores static client-side assets (CSS, JavaScript, images).
*   **`/LocalDatabase`**: Contains the `AgriEnergyConnect.db` SQLite database file, configured to be copied to the output directory.
*   **`appsettings.json`**: Central configuration file, including the database connection string.
*   **`Program.cs`**: The main application entry point, configuring services and the HTTP request pipeline.

**References:**
- Troelsen, A., & Japikse, P. (2022). Pro C# 10 with .NET 6: Foundational Principles and Practices in Programming. New York: Apress Media LLC.
- 
