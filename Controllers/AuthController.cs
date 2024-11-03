using BCSH2_SEM.Database;
using BCSH2_SEM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BCSH2_SEM.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseService _databaseService;

        public AuthController()
        {
            _databaseService = new DatabaseService();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string  login, string password) 
        {
            var user = _databaseService.Users.FindOne(u => u.Login == login);

            if (user != null && user.VerifyPassword(password))
            {
                return RedirectToAction("Index", "Home"); //zmenit casem na hlavni stranka / muj ucet
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
            DateOnly dateOfBirth, Gender gender, double height, double weight)
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
                Weight = weight
            };

            newUser.SetPassword(password);
            _databaseService.Users.Insert(newUser);

            return RedirectToAction("Login");

        }

            /*

            // GET: AuthController
            public ActionResult Index()
            {
                return View();
            }

            // GET: AuthController/Details/5
            public ActionResult Details(int id)
            {
                return View();
            }

            // GET: AuthController/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: AuthController/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(IFormCollection collection)
            {
                try
                {
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }

            // GET: AuthController/Edit/5
            public ActionResult Edit(int id)
            {
                return View();
            }

            // POST: AuthController/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(int id, IFormCollection collection)
            {
                try
                {
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }

            // GET: AuthController/Delete/5
            public ActionResult Delete(int id)
            {
                return View();
            }

            // POST: AuthController/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Delete(int id, IFormCollection collection)
            {
                try
                {
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            */
        }
}
