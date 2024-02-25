using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackApp.Data;
using TimeTrackApp.Models;

namespace TimeTrackApp.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _dbConntection;
        public UserController(AppDbContext appDbContext)
        {
            _dbConntection = appDbContext;
        }
        public async Task<IActionResult> AllUser()
        {
            var users = await _dbConntection.User.ToListAsync();

            return View(users);
        }

        [HttpGet]

        public IActionResult AddNewUser()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> AddNewUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                await _dbConntection.User.AddAsync(model);
                await _dbConntection.SaveChangesAsync();



                return RedirectToAction("AllUser");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var userData = await _dbConntection.User.FindAsync(Id);

            if (userData == null)
            {
                return NotFound();
            }



            return View(userData);
        }


        [HttpPost]
        public async Task<IActionResult> EditUser(UserModel updatedUser)
        {


            // Check if the task with the given ID exists in the database
            var existingTask = await _dbConntection.User.FindAsync(updatedUser.UserId);

            if (existingTask == null)
            {
                return NotFound();
            }


            if (updatedUser.UserEmail == null || updatedUser.UserPassword == null || updatedUser.UserPassword == null)
            {
                return View(updatedUser);
            }
            else
            {
                // Update the common properties
                existingTask.UserName = updatedUser.UserName;
                existingTask.UserPassword = updatedUser.UserPassword;

                // Update additional properties
                existingTask.UserEmail = updatedUser.UserEmail;
                existingTask.UserLevel = updatedUser.UserLevel;
                existingTask.UserActive = updatedUser.UserActive;

                // Save the changes to the database
                await _dbConntection.SaveChangesAsync();

                return RedirectToAction("AllUser");
            }

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel model)
        {
            var user = await _dbConntection.User.FirstOrDefaultAsync(u => u.UserName == model.UserName && u.UserPassword == model.UserPassword && u.UserActive == true);

            if (user != null)
            {
                // Store user information in session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("UserLevel", user.UserLevel);

                if (user.UserLevel == "Admin")
                {
                    return RedirectToAction("Dashboard", "Task");
                }

                return RedirectToAction("StaffDashboard", "Task");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View("Login");
            }
        }

        public IActionResult Logout()
        {
            // Clear session data on logout
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}
