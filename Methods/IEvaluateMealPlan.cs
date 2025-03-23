using BCSH2_SEM.Models;

namespace BCSH2_SEM.Methods
{
    public interface IEvaluateMealPlan
    {
        public double Evaluate(List<MealPortion> dailyMeals, User user);

        public double EvaluateMeal(MealPortion mealPortion, double idealCalories, double idealCarbs, double idealProteins, double idealFats);    }
}
