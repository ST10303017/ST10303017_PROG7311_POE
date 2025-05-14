/*
Calwyn Govender
ST10303017
PROG7311
(OpenAI, 2025)
(Troelsen & Japikse, 2022)
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST10303017_PROG7311_POE.Data;
using ST10303017_PROG7311_POE.Models;
using ST10303017_PROG7311_POE.Models.ViewModels; // <--- ADD THIS USING DIRECTIVE
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ST10303017_PROG7311_POE.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FarmerController> _logger;

        public FarmerController(ApplicationDbContext context, ILogger<FarmerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Farmer/MyProducts (No change needed here from previous full version)
        public async Task<IActionResult> MyProducts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized attempt to access MyProducts - User ID not found.");
                TempData["ErrorMessage"] = "You must be logged in to view your products.";
                return RedirectToAction("Login", "Account", new { Area = "Identity" });
            }

            _logger.LogInformation($"Fetching products for Farmer ID: {userId}");
            var products = await _context.Products
                                         .Where(p => p.FarmerID == userId)
                                         .OrderByDescending(p => p.ProductionDate)
                                         .ToListAsync();

            _logger.LogInformation($"Found {products.Count} products for Farmer ID: {userId}");
            return View(products);
        }

        // GET: Farmer/AddProduct - UPDATED
        public IActionResult AddProduct()
        {
            _logger.LogInformation("GET AddProduct page accessed.");
            // Pass an instance of the ViewModel to the view
            return View(new AddProductViewModel { ProductionDate = DateTime.Today });
        }

        // POST: Farmer/AddProduct - UPDATED
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddProductViewModel viewModel) // Accept AddProductViewModel
        {
            _logger.LogInformation($"POST AddProduct action started. Submitted Name: {viewModel.Name}"); // Log submitted data

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                _logger.LogError("CRITICAL: User ID not found in AddProduct POST for an authenticated user.");
                TempData["ErrorMessage"] = "Authentication error. Your session might have expired. Please log in again.";
                return RedirectToAction("Login", "Account", new { Area = "Identity" });
            }

            // ModelState will now only validate properties of AddProductViewModel
            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is valid for AddProductViewModel.");

                // Create and populate the Product entity from the ViewModel
                Product product = new Product
                {
                    Name = viewModel.Name,
                    Category = viewModel.Category,
                    ProductionDate = viewModel.ProductionDate,
                    FarmerID = userId // Set FarmerID here
                };

                _logger.LogInformation($"Attempting to add Product entity for Farmer ID: {userId}. Product Name: {product.Name}");
                _context.Add(product);
                try
                {
                    _logger.LogInformation("Calling SaveChangesAsync.");
                    int recordsAffected = await _context.SaveChangesAsync();
                    _logger.LogInformation($"SaveChangesAsync completed. Records affected: {recordsAffected}");

                    if (recordsAffected > 0)
                    {
                        _logger.LogInformation($"Product '{product.Name}' added successfully for Farmer ID: {userId}.");
                        TempData["SuccessMessage"] = "Product added successfully!";
                        return RedirectToAction(nameof(MyProducts));
                    }
                    else
                    {
                        _logger.LogWarning($"SaveChangesAsync completed for product '{product.Name}' but recordsAffected was 0. Farmer ID: {userId}.");
                        TempData["ErrorMessage"] = "Product could not be saved as no changes were detected in the database.";
                        ModelState.AddModelError(string.Empty, "Product could not be saved as no changes were detected.");
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, $"DbUpdateException saving product '{product.Name}' for Farmer ID: {userId}. Inner Exception: {dbEx.InnerException?.Message}");
                    foreach (var entry in dbEx.Entries) { _logger.LogError($"Entity of type '{entry.Entity.GetType().Name}' in state '{entry.State}' could not be saved."); }
                    ModelState.AddModelError(string.Empty, "Unable to save product due to a database error. Please try again.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"General error adding product '{product.Name}' for Farmer ID: {userId}.");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred while adding the product.");
                }
            }
            else // ModelState.IsValid is FALSE for AddProductViewModel
            {
                _logger.LogWarning($"ModelState is invalid for AddProduct POST (ViewModel). Farmer ID: {userId}.");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    if (modelStateVal.Errors.Any())
                    {
                        _logger.LogWarning($"Validation Error - Key: {modelStateKey}");
                        foreach (var error in modelStateVal.Errors) { _logger.LogWarning($"  Error: {error.ErrorMessage}"); if (error.Exception != null) { _logger.LogWarning($"  Exception: {error.Exception.Message}"); } }
                    }
                }
                TempData["ErrorMessage"] = "Please correct the highlighted errors below and try again.";
            }

            // If ModelState was invalid or save failed, return view with the ViewModel
            _logger.LogInformation("Returning to AddProduct view due to invalid ModelState or save error, using ViewModel.");
            return View(viewModel); // Return the ViewModel to the view
        }
    }
}