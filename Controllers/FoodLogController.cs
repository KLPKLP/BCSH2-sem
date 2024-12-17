using BCSH2_SEM.Database;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class FoodLogController : Controller
    {
        private readonly DatabaseService _databaseService;

        public FoodLogController()
        {
            _databaseService = new DatabaseService();
        }

        public IActionResult LogMeal()
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

         
            var ingredients = _databaseService.Ingredients.FindAll().ToList();
            var recipes = _databaseService.Recipes.FindAll().ToList();

            ViewBag.Ingredients = ingredients;
            ViewBag.Recipes = recipes;

            return View();
        }

        [HttpPost]
        public IActionResult LogMeal(int? ingredientId, double? ingredientAmount, int? recipeId, double? recipeAmount, bool isRecipe)
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

            double consumedCalories = 0, consumedProteins = 0, consumedCarbs = 0, consumedFats = 0;

            if (ingredientId.HasValue)
            {
                var ingredient = _databaseService.Ingredients.FindOne(i => i.Id == ingredientId.Value);
                if (ingredient != null && ingredientAmount.HasValue)
                {
                    consumedCalories = ingredient.Calories * (ingredientAmount.Value / 100);
                    consumedProteins = ingredient.Proteins * (ingredientAmount.Value / 100);
                    consumedCarbs = ingredient.Carbs * (ingredientAmount.Value / 100);
                    consumedFats = ingredient.Fats * (ingredientAmount.Value / 100);
                }
            }
            else //if (recipeId.HasValue && isRecipe)
            {
                Console.WriteLine("jsem recept");
                var recipe = _databaseService.Recipes.FindOne(r => r.Id == recipeId.Value);
                if (recipe != null && recipeAmount.HasValue)
                {
                    consumedCalories = recipe.TotalCalories * (recipeAmount.Value / 100);
                    consumedProteins = recipe.TotalProteins * (recipeAmount.Value / 100);
                    consumedCarbs = recipe.TotalCarbs * (recipeAmount.Value / 100);
                    consumedFats = recipe.TotalFats * (recipeAmount.Value / 100);
                } else //upravit, jestli je zaškrtnut celý recept
                {
                    consumedCalories = recipe.TotalCalories;
                    consumedProteins = recipe.TotalProteins;
                    consumedCarbs = recipe.TotalCarbs;
                    consumedFats = recipe.TotalFats;
                }
            }

            // Aktualizace zkonzumovaných hodnot u uživatele
            user.ConsumedCalories += consumedCalories;
            user.ConsumedProteins += consumedProteins;
            user.ConsumedCarbs += consumedCarbs;
            user.ConsumedFats += consumedFats;

            Console.WriteLine("Calories: " + consumedCalories);
            Console.WriteLine("Blkoviny: " +consumedProteins);
            Console.WriteLine("Sacharid: " + consumedCarbs);
            Console.WriteLine("tuky: " + consumedFats);

            Console.WriteLine("-------po update--------");

            _databaseService.Users.Update(user);


            Console.WriteLine("Calories: " + user.ConsumedCalories);
            Console.WriteLine("Blkoviny: " + user.ConsumedProteins);
            Console.WriteLine("Sacharid: " + user.ConsumedCarbs);
            Console.WriteLine("tuky: " + user.ConsumedFats);

            return RedirectToAction("Index", "MainPage");
        }
    }
}
