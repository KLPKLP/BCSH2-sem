using BCSH2_SEM.Database;
using BCSH2_SEM.Methods;
using BCSH2_SEM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly CaloriesCalculator _caloriesCalculator;


        public AuthController()
        {
            _databaseService = new DatabaseService();
            _caloriesCalculator = new CaloriesCalculator();

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string login, string password)
        {
            
            var user = _databaseService.Users.FindOne(u => u.Login == login);

            if (user != null && user.VerifyPassword(password))
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString()); //uložení id do session
                return RedirectToAction("Index", "MainPage"); 
            }

            ViewBag.Error = "Neplatné přihlašovací údaje";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(string name, string login, string password,
            DateOnly dateOfBirth, Gender gender, double height, double weight, Goal goal)
        {

            bool userExist = _databaseService.Users.Exists(u => u.Login == login);
            if (userExist)
            {
                ViewBag.Error = "Uživatel s tímto loginem již existuje";
                return View();
            }

            var newUser = new User
            {
               
                Name = name,
                Login = login,
                DateOfBirth = dateOfBirth,
                Gender = gender,
                Height = height,
                Weight = weight,
                MyGoal = goal
            };



            var test = newUser.Id; //????

            newUser.SetPassword(password);
            _databaseService.Users.Insert(newUser);


            // Vypočítání BMR
            double bmr = _caloriesCalculator.BMR(newUser.Weight, newUser.Height, newUser.DateOfBirth.ToDateTime(TimeOnly.MinValue), newUser.Gender);

            // Vypočítání denního příjmu kalorií
            newUser.DailyCalories = _caloriesCalculator.CalorieIntake(bmr, newUser.MyGoal);

            // Vypočítání doporučených živin
            _caloriesCalculator.Nutritions(newUser.MyGoal, newUser.DailyCalories, out double protein, out double carbs, out double fats);

            // Nastavení denních živin
            newUser.DailyProteins = protein;
            newUser.DailyCarbs = carbs;
            newUser.DailyFats = fats;

            // Aktualizace dat uživatele v databázi
            _databaseService.Users.Update(newUser);


            return RedirectToAction("Login");

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Login", "Auth"); 
        }



    }
}
