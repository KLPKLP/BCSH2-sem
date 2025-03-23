using BCSH2_SEM.Database;
using Microsoft.AspNetCore.Mvc;
using BCSH2_SEM.Methods;
using BCSH2_SEM.Models;

namespace BCSH2_SEM.Controllers
{
    public class MealPlanController : Controller
    {
        private readonly DatabaseService _databaseService;
        public MealPlanController()
        {
            _databaseService = new DatabaseService();
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(int numberOfMeals)
        {
            string userId = HttpContext.Session.GetString("UserId");

            var user = _databaseService.Users.FindOne(u => u.Id == new LiteDB.ObjectId(userId));

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

          //  user.ResetValues(); //Pak odstranit

            RecommendMealPlan recomender = new(numberOfMeals);
            MealPlan mealPlan = new(numberOfMeals);

            //DEBUG!!
            mealPlan.SetMeal(0, new MealPortion(1, _databaseService.Recipes.FindById(1)));

            MealPlan generatedPlan = recomender.RecommendPlanAnnealing(mealPlan, user);

            //if (generatedPlan == null)
            //{
            //    ViewBag.Error = "Nepodařilo se vygenerovat jídelníček.";
            //    return View("MealPlanView");
            //}

            return View("MealPlanView", generatedPlan);
        }

    }
}
