using BCSH2_SEM.Models;

namespace BCSH2_SEM.Methods
{
    public class EvaluateMealPlan : IEvaluateMealPlan
    {
        


        // TODO: Časem možná přidat vváhu (např. kalorie budou mít větší váhu než cukry)
        // todo - předělat na mealportion!!! (meal plan)
       public double Evaluate(List<MealPortion> dailyMeals, User user)
        {
            double consumedProteins = 0;
            double consumedFats = 0;
            double consumedCarbs  = 0;
            double consumedCalories = 0;

           

            foreach (var meal in dailyMeals)
            {
                consumedProteins += meal.Portion * meal.Recipe.TotalProteins;
                consumedFats += meal.Portion * meal.Recipe.TotalFats;
                consumedCarbs += meal.Portion * meal.Recipe.TotalCarbs;
                consumedCalories += meal.Portion * meal.Recipe.TotalCalories;
            }

           double  value = ((Math.Abs(user.DailyCalories -  consumedCalories)) / user.DailyCalories) +
                    ((Math.Abs(user.DailyCarbs - consumedCarbs)) / user.DailyCarbs) +
                    ((Math.Abs(user.DailyProteins - consumedProteins)) / user.DailyProteins) +
                    ((Math.Abs(user.DailyFats - consumedFats)) / user.DailyFats) ;

            return value;
            // Ideální hodnota = 0, nejhorší hodnota = 4
        }

        public double EvaluateMealPlanAnnealing(List<MealPortion> dailyMeals, double remianingCalories, double remainingCarbs, double remainingProteins, double remainigFats)
        {
            double consumedProteins = 0;
            double consumedFats = 0;
            double consumedCarbs = 0;
            double consumedCalories = 0;

            foreach (var meal in dailyMeals)
            {
                consumedProteins += meal.Portion * meal.Recipe.TotalProteins;
                consumedFats += meal.Portion * meal.Recipe.TotalFats;
                consumedCarbs += meal.Portion * meal.Recipe.TotalCarbs;
                consumedCalories += meal.Portion * meal.Recipe.TotalCalories;
            }

            double value = ((Math.Abs(remianingCalories - consumedCalories)) / remianingCalories) +
                  ((Math.Abs(remainingCarbs - consumedCarbs)) / remainingCarbs) +
                2*  ((Math.Abs(remainingProteins - consumedProteins)) / remainingProteins) + //Přidáno 2*
                  ((Math.Abs(remainigFats - consumedFats)) / remainigFats);

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
