using System.IO;
using System.Linq;
using Group_WebApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Group0WebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsersController(AppDbContext context, ILogger<UsersController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }



        [HttpPost]
        public IActionResult AddUser([FromForm] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context?.Users?.FirstOrDefault(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    ViewData["ErrorMessage"] = "User Name already taken";
                    return View("Signup", user);
                }

                _context?.Users?.Add(user);
                _context?.SaveChanges();
                string uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", $"{user.Username}.{user.FullName}");
                Directory.CreateDirectory(uploadsFolder);

                return RedirectToAction("Index");
            }
            else
            {
                return View("Signup", user);
            }
        }
        public IActionResult userv()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login([FromForm] User user)
        {
            var matchedUser = _context?.Users?.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (matchedUser != null)
            {
                var userInDatabase = _context?.Users?.FirstOrDefault(u => u.Username == user.Username);

                // Ensure that userInDatabase is not null
                if (userInDatabase != null)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "Uploads", $"{user.Username}.{userInDatabase.FullName}");

                    // Store user information in cookies
                    Response.Cookies.Append("UserID", userInDatabase.UserID.ToString());
                    Response.Cookies.Append("Username", userInDatabase.Username);
                    Response.Cookies.Append("FullName", userInDatabase.FullName);

                    return RedirectToAction("ManagementFiles", "Files", new { username = userInDatabase.Username, fullname = userInDatabase.FullName, userID = userInDatabase.UserID, folderPath });
                }
                else
                {
                    ViewData["ErrorMessage"] = "User not found.";
                    return View("Index");
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Invalid username or password.";
                return View("Index");
            }
        }

    }
}


    
