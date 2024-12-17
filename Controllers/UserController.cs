using BCSH2_SEM.Database;
using BCSH2_SEM.Methods;
using BCSH2_SEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly CaloriesCalculator _caloriesCalculator;

        public UserController()
        {
            _databaseService = new DatabaseService();
            _caloriesCalculator = new CaloriesCalculator();
        }

        public IActionResult Profile()
        {
            string userId = HttpContext.Session.GetString("UserId");

            if(string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth"); //přesměrování na login když není přihlášen

            }

            var user = _databaseService.Users.FindOne(u => u.Id == new LiteDB.ObjectId(userId));

            if(user == null )
            {
                return RedirectToAction("Login", "Auth");
            }

            //// Vypočítání BMR
            //double bmr = _caloriesCalculator.BMR(user.Weight, user.Height, user.DateOfBirth.ToDateTime(TimeOnly.MinValue), user.Gender);

            //// Vypočítání denního příjmu kalorií
            //user.DailyCalories = _caloriesCalculator.CalorieIntake(bmr, user.MyGoal);

            //// Vypočítání doporučených živin
            //_caloriesCalculator.Nutritions(user.MyGoal, user.DailyCalories, out double protein, out double carbs, out double fats);

            //// Nastavení denních živin
            //user.DailyProteins = protein;
            //user.DailyCarbs = carbs;
            //user.DailyFats = fats;

            //// Aktualizace dat uživatele v databázi
            //_databaseService.Users.Update(user);

            return View(user);
        }

        public IActionResult Edit()
        {
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth"); 

            }

            var user = _databaseService.Users.FindOne(u => u.Id == new LiteDB.ObjectId(userId));

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser)
        {
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");

            }

            var currentUser = _databaseService.Users.FindOne(u => u.Id == new LiteDB.ObjectId(userId));

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            currentUser.Name = updatedUser.Name;
            currentUser.DateOfBirth = updatedUser.DateOfBirth;
            currentUser.Gender = updatedUser.Gender;
            currentUser.Height = updatedUser.Height;
            currentUser.Weight = updatedUser.Weight;
            currentUser.MyGoal = updatedUser.MyGoal;


            // Vypočítání BMR
            double bmr = _caloriesCalculator.BMR(currentUser.Weight, currentUser.Height, currentUser.DateOfBirth.ToDateTime(TimeOnly.MinValue), currentUser.Gender);

            // Vypočítání denního příjmu kalorií
            currentUser.DailyCalories = _caloriesCalculator.CalorieIntake(bmr, currentUser.MyGoal);

            // Vypočítání doporučených živin
            _caloriesCalculator.Nutritions(currentUser.MyGoal, currentUser.DailyCalories, out double protein, out double carbs, out double fats);

            // Nastavení denních živin
            currentUser.DailyProteins = protein;
            currentUser.DailyCarbs = carbs;
            currentUser.DailyFats = fats;

            // Aktualizace dat uživatele v databázi
            _databaseService.Users.Update(currentUser);

            return RedirectToAction("Profile");

        }
    }
}
