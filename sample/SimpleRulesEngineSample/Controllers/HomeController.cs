using Microsoft.AspNetCore.Mvc;
using SimpleRules;
using SimpleRulesEngineSample.Entities;
using System.Collections.Generic;
using System.Linq;

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
            var users = new List<User>
            {
                new User
                {
                    Username = "kanant",
                    Password = "password123",
                    ConfirmPassword = "password1234",
                    EmailAddress = "kanant",
                    PhoneNumber = "12345"
                }
            };
            var results = _rulesEngine.Validate<User>(users).ToList();
            return View(results.SelectMany(r => r.Errors).ToList());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
