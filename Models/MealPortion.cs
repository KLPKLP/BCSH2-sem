namespace BCSH2_SEM.Models
{
    public class MealPortion
    {
        public double Portion;
        public Recipe Recipe;

        public MealPortion(double portion, Recipe recipe)
        {
            Portion = portion;
            Recipe = recipe;
        }
    }
}
