// File: Controllers/EmployeeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ST10303017_PROG7311_POE.Data;
using ST10303017_PROG7311_POE.Models;
using ST10303017_PROG7311_POE.Models.ViewModels; // For AddFarmerViewModel
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ST10303017_PROG7311_POE.Controllers
{
    [Authorize(Roles = "Employee")] // This controller is only accessible to users in the "Employee" role
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employee/AddFarmer
        public IActionResult AddFarmer()
        {
            return View(new AddFarmerViewModel());
        }

        // POST: Employee/AddFarmer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(AddFarmerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Assign the "Farmer" role to the newly created user
                    await _userManager.AddToRoleAsync(user, "Farmer");
                    TempData["SuccessMessage"] = $"Farmer account for {model.Email} created successfully.";
                    return RedirectToAction(nameof(ListFarmers)); // Or to a success page
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Employee/ListFarmers
        public async Task<IActionResult> ListFarmers()
        {
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            return View(farmers.OrderBy(f => f.UserName).ToList());
        }


        // GET: Employee/ViewFarmerProducts
        public async Task<IActionResult> ViewFarmerProducts(string farmerId, DateTime? startDate, DateTime? endDate, string productType)
        {
            // Populate dropdown for selecting a farmer
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            ViewBag.Farmers = new SelectList(farmers.OrderBy(f => f.UserName), "Id", "UserName", farmerId);

            // Populate dropdown for product types (categories)
            var categories = await _context.Products
                                           .Select(p => p.Category)
                                           .Distinct()
                                           .OrderBy(c => c)
                                           .ToListAsync();
            ViewBag.ProductTypes = new SelectList(categories, productType);

            IQueryable<Product> productsQuery = _context.Products.Include(p => p.Farmer); // Eager load Farmer info

            if (!string.IsNullOrEmpty(farmerId))
            {
                productsQuery = productsQuery.Where(p => p.FarmerID == farmerId);
            }
            else
            {
                // If no farmer is selected, display no products.
                // This prevents showing all products by default.
                productsQuery = productsQuery.Where(p => false);
            }

            if (startDate.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.ProductionDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                // To include the end date, query up to the end of that day
                productsQuery = productsQuery.Where(p => p.ProductionDate < endDate.Value.AddDays(1));
            }
            if (!string.IsNullOrEmpty(productType))
            {
                productsQuery = productsQuery.Where(p => p.Category == productType);
            }

            var products = await productsQuery.OrderByDescending(p => p.ProductionDate).ToListAsync();

            // Retain filter values for the view
            ViewBag.SelectedFarmerId = farmerId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd"); // Format for date input
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");   // Format for date input
            ViewBag.SelectedProductType = productType;

            return View(products);
        }
    }
}