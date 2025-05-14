// File: Data/SeedData.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ST10303017_PROG7311_POE.Models; // Your ApplicationUser and Product models
using System;
using System.Collections.Generic; // For List
using System.Linq;
using System.Threading.Tasks;

namespace ST10303017_PROG7311_POE.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // We need context to add products

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("ST10303017_PROG7311_POE.Data.SeedData");

            logger.LogInformation("Starting extensive database seeding process.");

            // --- 1. Seed Roles ---
            logger.LogInformation("Seeding Roles...");
            string[] roleNames = { "Employee", "Farmer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    logger.LogInformation($"Role '{roleName}' created.");
                }
                else
                {
                    logger.LogInformation($"Role '{roleName}' already exists.");
                }
            }

            // --- 2. Seed Employees (5 total) ---
            logger.LogInformation("Seeding Employees...");
            var employeesToSeed = new List<(string Email, string Password)>
            {
                ("employee1@agrienergy.com", "Password123!"),
                ("employee2@agrienergy.com", "Password123!"),
                ("employee3@agrienergy.com", "Password123!"),
                ("employee4@agrienergy.com", "Password123!"),
                ("employee5@agrienergy.com", "Password123!")
            };

            foreach (var emp in employeesToSeed)
            {
                var employeeUser = await userManager.FindByEmailAsync(emp.Email);
                if (employeeUser == null)
                {
                    employeeUser = new ApplicationUser
                    {
                        UserName = emp.Email,
                        Email = emp.Email,
                        EmailConfirmed = true
                    };
                    var createResult = await userManager.CreateAsync(employeeUser, emp.Password);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(employeeUser, "Employee");
                        logger.LogInformation($"Employee '{emp.Email}' created and assigned 'Employee' role.");
                    }
                    else
                    {
                        foreach (var error in createResult.Errors) { logger.LogError($"Error creating employee '{emp.Email}': {error.Description}"); }
                    }
                }
                else
                {
                    logger.LogInformation($"Employee '{emp.Email}' already exists.");
                    if (!await userManager.IsInRoleAsync(employeeUser, "Employee")) // Ensure role if user exists
                    {
                        await userManager.AddToRoleAsync(employeeUser, "Employee");
                        logger.LogInformation($"Assigned 'Employee' role to existing user '{employeeUser.Email}'.");
                    }
                }
            }

            // --- 3. Seed Farmers (5 total) ---
            logger.LogInformation("Seeding Farmers...");
            var farmersToSeed = new List<(string Email, string Password, string FarmerName)> // Added FarmerName for potential display
            {
                ("farmer.john@example.com", "FarmerPass1!", "John's Farm"),
                ("farmer.jane@example.com", "FarmerPass2!", "Jane's Produce"),
                ("farmer.peter@example.com", "FarmerPass3!", "Peter's Patch"),
                ("farmer.susan@example.com", "FarmerPass4!", "Susan's Sustainable"),
                ("farmer.mike@example.com", "FarmerPass5!", "Mike's Meadows")
            };

            var seededFarmerIds = new List<string>(); // To store IDs of newly created farmers for product seeding

            foreach (var farm in farmersToSeed)
            {
                var farmerUser = await userManager.FindByEmailAsync(farm.Email);
                if (farmerUser == null)
                {
                    farmerUser = new ApplicationUser
                    {
                        UserName = farm.Email,
                        Email = farm.Email,
                        EmailConfirmed = true
                        // You could add a FullName property to ApplicationUser if you want to store farm.FarmerName
                    };
                    var createResult = await userManager.CreateAsync(farmerUser, farm.Password);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(farmerUser, "Farmer");
                        logger.LogInformation($"Farmer '{farm.Email}' ({farm.FarmerName}) created and assigned 'Farmer' role.");
                        seededFarmerIds.Add(farmerUser.Id); // Add ID for product seeding
                    }
                    else
                    {
                        foreach (var error in createResult.Errors) { logger.LogError($"Error creating farmer '{farm.Email}': {error.Description}"); }
                    }
                }
                else
                {
                    logger.LogInformation($"Farmer '{farm.Email}' ({farm.FarmerName}) already exists.");
                    if (!await userManager.IsInRoleAsync(farmerUser, "Farmer")) // Ensure role if user exists
                    {
                        await userManager.AddToRoleAsync(farmerUser, "Farmer");
                        logger.LogInformation($"Assigned 'Farmer' role to existing user '{farmerUser.Email}'.");
                    }
                    seededFarmerIds.Add(farmerUser.Id); // Add existing farmer's ID too if we want to ensure they have products
                }
            }

            // --- 4. Seed Products for each Farmer (5 products each) ---
            logger.LogInformation("Seeding Products for Farmers...");
            var productCategories = new List<string> { "Vegetables", "Fruits", "Dairy", "Grains", "Poultry", "Herbs", "Honey" };
            var random = new Random();

            // Ensure we only seed products for farmers we actually have IDs for
            foreach (var farmerId in seededFarmerIds.Distinct()) // Use Distinct in case an existing farmer was re-added to the list
            {
                // Check if this farmer already has products (to avoid re-seeding if run multiple times)
                // This check assumes FarmerID is the foreign key in your Product model
                if (!context.Products.Any(p => p.FarmerID == farmerId))
                {
                    var farmerInfo = await userManager.FindByIdAsync(farmerId); // Get farmer's email for logging
                    logger.LogInformation($"Seeding products for farmer: {farmerInfo?.Email ?? farmerId}");

                    for (int i = 1; i <= 5; i++)
                    {
                        var category = productCategories[random.Next(productCategories.Count)];
                        var productName = $"{category.Substring(0, Math.Min(category.Length, 4))} Product #{i} {farmerInfo?.UserName?.Split('@')[0] ?? "F"}"; // Example product name
                        var productionDate = DateTime.Today.AddDays(-random.Next(0, 180)); // Products from last 6 months

                        context.Products.Add(new Product
                        {
                            Name = productName,
                            Category = category,
                            ProductionDate = productionDate,
                            FarmerID = farmerId // This is crucial
                        });
                        logger.LogInformation($"Added product '{productName}' for farmer {farmerInfo?.Email ?? farmerId}.");
                    }
                }
                else
                {
                    var farmerInfo = await userManager.FindByIdAsync(farmerId);
                    logger.LogInformation($"Farmer {farmerInfo?.Email ?? farmerId} already has products. Skipping product seeding for this farmer.");
                }
            }

            // Save product changes to the database
            try
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Product seeding changes saved to database.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving products to database during seeding.");
            }

            logger.LogInformation("Extensive database seeding process completed.");
        }
    }
}