using BCSH2_SEM.Methods;
using LiteDB;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace BCSH2_SEM.Models
{
    public class User
    {
        //private readonly CaloriesCalculator _caloriesCalculator;

        //public User()
        //{
        //    _caloriesCalculator = new CaloriesCalculator();
        //}

        public LiteDB.ObjectId Id { get; set; } //
        public string Name { get; set; }
        public string Login { get; set; } //jedinečné id?
        public string PasswordHash { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public double Height { get; set; }
        public double Weight { get; set; }



        public double DailyCalories { get; set; }
        public double DailyProteins { get; set; }
        public double DailyCarbs { get; set; }
        public double DailyFats { get; set; }


        public double ConsumedCalories { get; set; }
        public double ConsumedProteins { get; set; }
        public double ConsumedCarbs { get; set; }
        public double ConsumedFats { get; set; }


        public Goal MyGoal { get; set; }


        public void SetPassword(string password)
        {
            PasswordHash = CreatePasswordHash(password);
        }

        private string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == CreatePasswordHash(password);
        }


        //public void AddCalories(double calories)
        //{
        //    ConsumedCalories += calories;
        //}

        //public void AddProteins(double proteins)
        //{
        //    ConsumedProteins += proteins;
        //}

        //public void AddCarbs(double carbs)
        //{
        //    ConsumedCarbs += carbs;
        //}

        //public void AddFats(double fats)
        //{
        //    ConsumedFats += fats;
        //}
        public void ResetValues()
        {
            ConsumedCalories = 0;
            ConsumedProteins = 0;
            ConsumedCarbs = 0;
            ConsumedFats = 0;

            var _caloriesCalculator = new CaloriesCalculator();

            // Vypočítání BMR
            double bmr = _caloriesCalculator.BMR(Weight, Height, DateOfBirth.ToDateTime(TimeOnly.MinValue), Gender);

            // Vypočítání denního příjmu kalorií
           DailyCalories = _caloriesCalculator.CalorieIntake(bmr, MyGoal);

            // Vypočítání doporučených živin
            _caloriesCalculator.Nutritions(MyGoal, DailyCalories, out double protein, out double carbs, out double fats);

            // Nastavení denních živin
            DailyProteins = protein;
            DailyCarbs = carbs;
            DailyFats = fats;


        }

    }

    public enum Gender
    {
       
        Male,

        
        Female
    }

    public enum Goal
    {
       
        Lose,

        
        Maintain,

     
        Gain
    }
}
