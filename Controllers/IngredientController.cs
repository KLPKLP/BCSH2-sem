using BCSH2_SEM.Database;
using BCSH2_SEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class IngredientController : Controller
    {
        private readonly DatabaseService _dbService;

        public IngredientController()
        {
            _dbService = new DatabaseService();
        }

        
        public ActionResult Index()
        {
            var ingredients = _dbService.Ingredients.FindAll().OrderBy(i => i.Name);
            return View(ingredients); 
        }


        public ActionResult Add()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Add(Ingredient ingredient)
        {
            // DEBUG! Vypsání chybové hlášky
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(ingredient);
            }

            if (ModelState.IsValid)
            {
                _dbService.Ingredients.Insert(ingredient);
                return RedirectToAction("Index");
            }


            return View(ingredient);
        }



        //------------ POKUS O PŘIDÁNÍ URL -------------


        //public ActionResult Add(string returnUrl)
        //{
        //    Console.WriteLine($"returnUrl: {returnUrl}");
        //    ViewBag.ReturnUrl = returnUrl ?? Url.Action("Create", "Recipes"); 
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Add(Ingredient ingredient, string returnUrl)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    var errors = ModelState.Values.SelectMany(v => v.Errors);
        //    //    foreach (var error in errors)
        //    //    {
        //    //        Console.WriteLine(error.ErrorMessage);
        //    //    }
        //    //    return View(ingredient);
        //    //}
        //    Console.WriteLine("return url z post: " + returnUrl);

        //    if (ModelState.IsValid)
        //    {
        //        _dbService.Ingredients.Insert(ingredient);

        //        if (!string.IsNullOrEmpty(returnUrl))
        //        {
        //            return Redirect(returnUrl);
        //        }
        //        else
        //        {
        //            //return Redirect(Url.Action("Index", "Recipe"));
        //            return RedirectToAction("Index", "Recipes");
        //        }


        //       // return RedirectToAction("Index");
        //    }


        //    return View(ingredient);
        //}



        public ActionResult Edit(int id)
        {
            var ingredient = _dbService.Ingredients.FindById(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost]
        public ActionResult Edit(Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                _dbService.Ingredients.Update(ingredient);
                return RedirectToAction("Index");
            }

            return View(ingredient);
        }

        public ActionResult Delete(int id)
        {
            var ingredient = _dbService.Ingredients.FindById(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            _dbService.Ingredients.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
