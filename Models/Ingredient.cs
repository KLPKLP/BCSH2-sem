namespace BCSH2_SEM.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // hodnoty jsou vztažené na 100 gramů
        public double Calories { get; set; }
        public double Proteins { get; set; }

        public double Carbs { get; set; }

        public double Fats { get; set; }




    }
}
