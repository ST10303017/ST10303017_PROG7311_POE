// File: Areas/Identity/Pages/Account/Register.cshtml.cs
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; // Make sure this is present if you use IEmailSender
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ST10303017_PROG7311_POE.Models; // Your ApplicationUser model

namespace ST10303017_PROG7311_POE.Areas.Identity.Pages.Account
{
    [AllowAnonymous] // Allow access to the Register page without being logged in
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender; // Keep if you plan to implement email sending
        private readonly RoleManager<IdentityRole> _roleManager; // Inject RoleManager

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender, // Keep if you plan to implement email sending
            RoleManager<IdentityRole> roleManager) // Add RoleManager to constructor
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender; // Keep if you plan to implement email sending
            _roleManager = roleManager; // Assign RoleManager
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                // You can set other ApplicationUser properties here if needed before creating
                // user.EmailConfirmed = true; // For prototype, you might auto-confirm if not sending emails

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // --- OPTION A: Auto-assign "Employee" role to newly registered users ---
                    // Ensure the "Employee" role exists (could also be done in SeedData.cs for robustness)
                    if (!await _roleManager.RoleExistsAsync("Employee"))
                    {
                        // Log this, as ideally roles are pre-seeded
                        _logger.LogWarning("Role 'Employee' did not exist. Creating it now. Consider pre-seeding roles.");
                        await _roleManager.CreateAsync(new IdentityRole("Employee"));
                    }
                    // Also ensure "Farmer" role exists if not already (for SeedData consistency, though not assigned here)
                    if (!await _roleManager.RoleExistsAsync("Farmer"))
                    {
                        _logger.LogWarning("Role 'Farmer' did not exist. Creating it now. Consider pre-seeding roles.");
                        await _roleManager.CreateAsync(new IdentityRole("Farmer"));
                    }

                    await _userManager.AddToRoleAsync(user, "Employee");
                    _logger.LogInformation($"User {user.UserName} was assigned the 'Employee' role.");
                    // --- End of OPTION A ---

                    var userId = await _userManager.GetUserIdAsync(user);

                    // Email Confirmation Logic (keep or modify based on your project's needs)
                    // If options.SignIn.RequireConfirmedAccount = true in Program.cs, this path will be taken.
                    // If false, the 'else' block for direct sign-in will be executed.
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        // For a prototype where you might not have a real email sender configured:
                        _logger.LogInformation($"Generated email confirmation URL for {Input.Email}: {callbackUrl}");
                        // You would typically send this URL in an email:
                        // await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // If RequireConfirmedAccount is false (as set in our Program.cs for prototype simplicity),
                        // sign the user in directly after registration.
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation($"User {user.UserName} signed in automatically after registration.");
                        // Redirect to a page appropriate for a new Employee, e.g., Employee dashboard or ListFarmers
                        // If ListFarmers is the intended page for new employees:
                        return LocalRedirect(Url.Action("ListFarmers", "Employee", new { area = "" }) ?? returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}