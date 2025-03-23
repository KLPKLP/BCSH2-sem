using BCSH2_SEM.Database;
using BCSH2_SEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class RecipesController : Controller
    {
        private readonly DatabaseService _db;

        public RecipesController()
        {
            _db = new DatabaseService();
        }

        public IActionResult Create()
        {

            var ingredients = _db.Ingredients.FindAll().OrderBy(i => i.Name).ToList();
            return View(ingredients); 
        }

        [HttpPost]
        public IActionResult Create(string name, string description, List<int> selectedIngredients, Dictionary<int, double> quantities, 
            bool suitableAsBreakfast = false, bool suitableAsSnack = false, bool suitableAsLunch = false, bool suitableAsDinner = false) // Výchozí hodnota pro bool suitableAsLunch
        {
          
            var recipe = new Recipe
            {
                Name = name,
                Description = description,
                Ingredients = selectedIngredients.Select(id => new RecipeIngredient
                {
                    Ingredient = _db.Ingredients.FindById(id),
                    Quantity = quantities.ContainsKey(id) ? quantities[id] : 0 // Použití zadaného množství
                }).ToList(),
                SuitableAsBreakfast= suitableAsBreakfast,
                SuitableAsSnack = suitableAsSnack,
                SuitableAsLunch = suitableAsLunch,
                SuitableAsDinner = suitableAsDinner,
            };

            
            _db.Recipes.Insert(recipe);

            return RedirectToAction("Index"); 
        }


        
        [HttpGet]
        public IActionResult Index()
        {
            var recipes = _db.Recipes.FindAll().OrderBy(i => i.Name).ToList();
            return View(recipes);
        }

        
        [HttpGet]
        public IActionResult Details(int id, double portion = 1.0)
        {
            var recipe = _db.Recipes.FindById(id);
            if (recipe == null)
            {
                return NotFound();
            }
            MealPortion mealPortion = new(portion, recipe);

            //ViewBag.Portion = portion; //TODO: najít lepší řešení než viewbag - možná předělat model na portion?
            Console.WriteLine(portion);
            //return View(recipe);
            return View(mealPortion);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var recipe = _db.Recipes.FindById(id);
            if (recipe == null)
            {
                return NotFound();
            }

            ViewBag.Ingredients = _db.Ingredients.FindAll().OrderBy(i => i.Name).ToList();
            return View(recipe);
        }

        [HttpPost]
        public IActionResult Edit(int id, string name, string description, List<int> selectedIngredients, Dictionary<int, double> quantities,
            bool suitableAsBreakfast = false, bool suitableAsSnack = false, bool suitableAsLunch = false, bool suitableAsDinner = false)
        {
            var recipe = _db.Recipes.FindById(id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Aktualizace názvu a popisu
            recipe.Name = name;
            recipe.Description = description;

            // Aktualizace ingrediencí
            recipe.Ingredients = selectedIngredients.Select(ingredientId => new RecipeIngredient
            {
                Ingredient = _db.Ingredients.FindById(ingredientId),
                Quantity = quantities.ContainsKey(ingredientId) ? quantities[ingredientId] : 0
            }).ToList();

            recipe.SuitableAsLunch = suitableAsLunch;
            recipe.SuitableAsBreakfast = suitableAsBreakfast;
            recipe.SuitableAsSnack = suitableAsSnack;
            recipe.SuitableAsDinner = suitableAsDinner;


            // Uložení změn
            _db.Recipes.Update(recipe);

            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult Delete(int id)
        {
            var recipe = _db.Recipes.FindById(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _db.Recipes.Delete(id);
            return RedirectToAction("Index");
        }




    }
}
