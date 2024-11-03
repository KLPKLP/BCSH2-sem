using System.Security.Cryptography;
using System.Text;

namespace BCSH2_SEM.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public double Height { get; set; }
        public double Weight { get; set; }

      
        
        public int DailyCalories { get; set; }
        public int DailyProteins { get; set; }
        public int DailyCarbs { get; set; }
        public int DailyFats { get; set; }


        public void SetPassword(string password)
        {
            PasswordHash = CreatePasswordHash(password);
        }

        private string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Encoding.UTF8.GetString(hashBytes);
            }
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == CreatePasswordHash(password); 
        }

    }

    public enum Gender
    {
        Male,
        Female
    }
}
