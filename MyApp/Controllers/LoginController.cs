using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Migrations;
using MyApp.Models;
using System.Security.Cryptography;
using static MyApp.Data.MyAppContext;

namespace MyApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult login()
        {
            return View("login");
        }

        [HttpPost]
        public IActionResult newuser(string Username, string Password, string confirmpassword)
        {
            if (Password != confirmpassword)
            {
                // Show an error message in the View
                ViewBag.ErrorMessage = "Password and Confirm Password do not match or are empty.";
                return View("login");
            }

            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(confirmpassword) || string.IsNullOrEmpty(Username))
            {
                ViewBag.ErrorMessage = "Insert a valid Input.";
                return View("login");
            }

            // Hash the password before saving
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            // Generate a secure token (using random bytes)
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)); // 32 bytes = 256-bit

            var newUser = new UsersModel { Username = Username, Password = hashedPassword, 
                                          Token = token, Role = "user",};
            _context.Users.Add(newUser);
            _context.SaveChanges(); // Save to SQL Server

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult signup(string Username, string Password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == Username);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("login");
            }

            // Verify the hashed password
            if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("login");
            }
            // Store user details in Session
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Token", user.Token);

            return Redirect("/Product");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears all session data
            ViewBag.Message = "Logout successful."; // Fix: Set a valid ViewBag property
            return View("login");
        }
    }
}
