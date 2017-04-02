using Microsoft.AspNetCore.Mvc;
using SimpleRules;
using SimpleRulesEngineSample.Entities;
using SimpleRulesEngineSample.Models;
using System;

namespace SimpleRulesEngineSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly SimpleRulesEngine _rulesEngine;

        public HomeController(SimpleRulesEngine rulesEngine)
        {
            _rulesEngine = rulesEngine;
        }

        public IActionResult Index()
        {
            var model = new ValidationResultsModel();
            var user = new User
                {
                    Username = "kanant",
                    Password = "password123",
                    ConfirmPassword = "password1234",
                    EmailAddress = "kanant",
                    PhoneNumber = "12345"
                };
            model.UserValidationResults = _rulesEngine.Validate<User>(user).Errors;
            var registration = new Registration
                {
                    StartDate = DateTime.Now.AddDays(-3),
                    EndDate = DateTime.Now.AddDays(-6)
                };
            model.RegistrationValidationResults = _rulesEngine.Validate<Registration>(registration).Errors;
            var activity = new Activity
                {
                    StartDate = DateTime.Now.AddDays(-3),
                    EndDate = DateTime.Now.AddDays(-6),
                    Capacity = 25
                };
            model.ActivityValidationResults = _rulesEngine.Validate<Activity>(activity).Errors;
            var student = new Student
            {
                Id = 1,
                Ssn = "12345",
                EmailAddress = "jdoe",
                RegistrationDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(-1),
                DateOfBirth = DateTime.Now,
                EnrolledCount = 5,
                Contact = "98412",
                EndDate = DateTime.Now.AddDays(3)
            };
            var validationResult = _rulesEngine.Validate<Student>(student);
            model.StudentValidationResults = validationResult.Errors;
            model.Key = (int)validationResult.Key;

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
