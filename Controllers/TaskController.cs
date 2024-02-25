using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;
using TimeTrackApp.Data;
using TimeTrackApp.Models;

namespace TimeTrackApp.Controllers
{
    public class TaskController : Controller
    {
        private readonly AppDbContext _dbConntection;
        public TaskController(AppDbContext appDbContext)
        {
            _dbConntection = appDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> StaffDashboard()
        {

            var userId = HttpContext.Session.GetInt32("UserId");

            var tasks = await _dbConntection.Task
        .Where(u => u.TaskAssignedUserId == userId)
        .ToListAsync();




            return View(tasks);

        }




        public async Task<IActionResult> AllTask()
        {


            var tasks = await _dbConntection.Task.ToListAsync();

            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            ViewBag.Users = users.Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList();

            return View(tasks);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewTask()
        {
            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            // Create a SelectList for the dropdown, specifying the display text and value
            var usersSelectList = new SelectList(users, "UserId", "UserName");

            // Set the ViewBag property to the SelectList
            ViewBag.Users = usersSelectList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTask(TaskModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.TaskCreateBy = HttpContext.Session.GetString("UserName");
                await _dbConntection.Task.AddAsync(model);
                await _dbConntection.SaveChangesAsync();



                return RedirectToAction("AllTask");
            }

            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            // Create a SelectList for the dropdown, specifying the display text and value
            var usersSelectList = new SelectList(users, "UserId", "UserName");

            // Set the ViewBag property to the SelectList
            ViewBag.Users = usersSelectList;


            return View(model);
        }

        public async Task<IActionResult> StaffExport()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                var tasks = await _dbConntection.Task
                    .Where(u => u.TaskAssignedUserId == userId)
                    .ToListAsync();

                // Create a StringBuilder to build the text content
                var textContent = new StringBuilder();

                // Headers
                textContent.AppendLine("Task Subject\tTask Created Date\tTask Last Modified Date");

                // Data
                foreach (var task in tasks)
                {
                    textContent.AppendLine($"{task.TaskSubject}\t{task.TaskCreatedDateAndTime:yyyy-MM-dd HH:mm:ss}\t{task.TaskLastModifiedDateAndTime:yyyy-MM-dd HH:mm:ss}");
                }

                // Save the text content to a file
                var fileName = "Tasks_Report.txt";
                var filePath = $@"D:\{fileName}"; // Change the path as needed
                await System.IO.File.WriteAllTextAsync(filePath, textContent.ToString());

                // Download the file
                return RedirectToAction("AllTask");

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                return BadRequest($"Error: {ex.Message}");
            }
        }


        public async Task<IActionResult> StaffReport()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var tasks = await _dbConntection.Task
        .Where(u => u.TaskAssignedUserId == userId)
        .ToListAsync();




            return View(tasks);
        }




        private async Task<string> FindUserEmail(TaskModel task)
        {
            var userId = task.TaskAssignedUserId;

            var userDetail = await _dbConntection.User.FirstOrDefaultAsync(u => u.UserId == userId);

            // Check if userDetail is not null before accessing the UserEmail property
            return userDetail?.UserEmail;
        }


        private async Task SendEmailNotification(TaskModel task)
        {
            // Use an email service (SMTP) to send the notification
            // Replace the placeholders with your email configuration and content

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("nilutheepan01@gmail.com", "wgvehguiluipbtic"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("battikaran6@gmail.com"),
                Subject = "New Task Added",
                Body = $"A new task '{task.TaskSubject}' has been added by {task.TaskCreateBy}.",
            };

            var assignedUserEmail = await FindUserEmail(task); // Await the asynchronous operation

            mailMessage.To.Add(assignedUserEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        [HttpGet]
        public async Task<IActionResult> EditTask(int? Id)
        {

            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var taskData = await _dbConntection.Task.FindAsync(Id);

            if (taskData == null)
            {
                return NotFound();
            }

            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            // Create a SelectList for the dropdown, specifying the display text and value
            var usersSelectList = new SelectList(users, "UserId", "UserName");

            // Set the ViewBag property to the SelectList
            ViewBag.Users = usersSelectList;

            return View(taskData);
        }


        [HttpPost]
        public async Task<IActionResult> EditTask(TaskModel updatedTask)
        {


            // Check if the task with the given ID exists in the database
            var existingTask = await _dbConntection.Task.FindAsync(updatedTask.TaskId);

            if (existingTask == null)
            {
                return NotFound();
            }



            // Update the common properties
            existingTask.TaskSubject = updatedTask.TaskSubject;
            existingTask.TaskStatus = updatedTask.TaskStatus;

            // Update additional properties
            existingTask.TaskDescription = updatedTask.TaskDescription;
            existingTask.TaskAssignedUserId = updatedTask.TaskAssignedUserId;
            existingTask.TaskCreatedDateAndTime = updatedTask.TaskCreatedDateAndTime;

            // Save the changes to the database
            await _dbConntection.SaveChangesAsync();

            return RedirectToAction("AllTask");


            //// Retrieve the list of users from the database
            //var users = await _dbConntection.User.ToListAsync();

            //// Create a SelectList for the dropdown, specifying the display text and value
            //var usersSelectList = new SelectList(users, "UserId", "UserName");

            //// Set the ViewBag property to the SelectList
            //ViewBag.Users = usersSelectList;

            //return View(updatedTask);

        }




        public async Task<IActionResult> AddNewTaskToCreate(TaskModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                await _dbConntection.Task.AddAsync(model);
                await _dbConntection.SaveChangesAsync();

                return RedirectToAction("AllTask");
            }

            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            // Create a SelectList for the dropdown, specifying the display text and value
            var usersSelectList = new SelectList(users, "UserId", "UserName");

            // Set the ViewBag property to the SelectList
            ViewBag.Users = usersSelectList;


            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ApprovalTask()
        {


            var tasks = await _dbConntection.Task.Where(app => app.TaskAdminApprove == false).ToListAsync();

            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            ViewBag.Users = users.Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList();

            return View(tasks);
        }


        [HttpGet]
        public async Task<IActionResult> ApprovalTaskAdmin(int? Id)
        {


            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            var taskData = await _dbConntection.Task.FindAsync(Id);

            if (taskData == null)
            {
                return NotFound();
            }

            if (taskData.TaskAdminApprove == false)
            {
                taskData.TaskAdminApprove = true;

                await _dbConntection.SaveChangesAsync();

                // Send email notification
                await SendEmailNotification(taskData);

                RedirectToAction("ApprovalTask");
            }

            var tasks = await _dbConntection.Task.Where(app => app.TaskAdminApprove == false).ToListAsync();

            // Retrieve the list of users from the database
            var users = await _dbConntection.User.ToListAsync();

            ViewBag.Users = users.Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.UserName }).ToList();

            return View(tasks);

        }
    }
}
