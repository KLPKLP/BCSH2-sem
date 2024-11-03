namespace BCSH2_SEM.Database
{
    using BCSH2_SEM.Models;
    using LiteDB;
    using System.Security.Cryptography.X509Certificates;

    public class DatabaseService
    {
        private readonly LiteDatabase _database;
        public DatabaseService() { 
        
            _database = new LiteDatabase("FileName=NutritionalBalance.db; Connection=shared");
        }

        public ILiteCollection<User> Users => _database.GetCollection<User>("users");

        public ILiteCollection<Recipe> Recipes => _database.GetCollection<Recipe>("recipes");

        public ILiteCollection<Ingredient> Ingredients => _database.GetCollection<Ingredient>("ingredients");

    }
}
