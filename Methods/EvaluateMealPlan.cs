using BCSH2_SEM.Models;

namespace BCSH2_SEM.Methods
{
    public class EvaluateMealPlan : IEvaluateMealPlan
    {
        


        // TODO: Časem možná přidat vváhu (např. kalorie budou mít větší váhu než cukry)
        // todo - předělat na mealportion!!! (meal plan)
        double IEvaluateMealPlan.Evaluate(List<Recipe> dailyMeals, User user)
        {
            double consumedProteins = 0;
            double consumedFats = 0;
            double consumedCarbs  = 0;
            double consumedCalories = 0;

           

            foreach (Recipe recipe in dailyMeals)
            {
                consumedProteins += recipe.TotalProteins;
                consumedFats += recipe.TotalFats;
                consumedCarbs += recipe.TotalCarbs;
                consumedCalories += recipe.TotalCalories;
            }

           double  value = ((Math.Abs(user.DailyCalories -  consumedCalories)) / user.DailyCalories) +
                    ((Math.Abs(user.DailyCarbs - consumedCarbs)) / user.DailyCarbs) +
                    ((Math.Abs(user.DailyProteins - consumedProteins)) / user.DailyProteins) +
                    ((Math.Abs(user.DailyFats - consumedFats)) / user.DailyFats) ;

            return value;
            // Ideální hodnota = 0, nejhorší hodnota = 4
        }


        //Přidělat na jednotlivý recept, parametry = ideální hodnoty , do jiného rozhraní, a jak se to odchyluje
        public double EvaluateMeal(MealPortion mealPortion, double idealCalories, double idealCarbs, double idealProteins, double idealFats)
        {
            double value = (Math.Abs(idealCalories - mealPortion.Portion * mealPortion.Recipe.TotalCalories) / idealCalories) +
                (Math.Abs(idealCarbs - mealPortion.Portion * mealPortion.Recipe.TotalCarbs) / idealCarbs) +
                (Math.Abs(idealProteins - mealPortion.Portion * mealPortion.Recipe.TotalProteins) / idealProteins) +
                (Math.Abs(idealFats - mealPortion.Portion * mealPortion.Recipe.TotalFats) / idealFats);


            return value;
        }

        
    }
}
