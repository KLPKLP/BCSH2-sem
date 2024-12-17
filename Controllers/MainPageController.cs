using BCSH2_SEM.Database;
using BCSH2_SEM.Methods;
using BCSH2_SEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class MainPageController : Controller
    {
        private readonly DatabaseService _databaseService;

        public MainPageController()
        {
            _databaseService = new DatabaseService();
        }

        

        public IActionResult Index()
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



            ViewBag.ConsumedCalories = user.ConsumedCalories;
            ViewBag.ConsumedProteins = user.ConsumedProteins;
            ViewBag.ConsumedCarbs = user.ConsumedCarbs;
            ViewBag.ConsumedFats = user.ConsumedFats;

            ViewBag.DailyCalories = user.DailyCalories;
            ViewBag.DailyProteins = user.DailyProteins;
            ViewBag.DailyCarbs = user.DailyCarbs;
            ViewBag.DailyFats = user.DailyFats;

            return View();
        }


        public IActionResult ResetValues()
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
            
            user.ResetValues();
            _databaseService.Users.Update(user);

            return RedirectToAction("Index"); 
        }

        [HttpPost]
        public IActionResult AddBurnedCalories(int burnedCalories)
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

            
            user.DailyCalories += burnedCalories;

           
            var calculator = new CaloriesCalculator();
            calculator.Nutritions(user.MyGoal, user.DailyCalories, out double protein, out double carbs, out double fats);

            user.DailyProteins = protein;
            user.DailyCarbs = carbs;
            user.DailyFats = fats;

          
            _databaseService.Users.Update(user);

            return RedirectToAction("Index");
        }



    }
}
