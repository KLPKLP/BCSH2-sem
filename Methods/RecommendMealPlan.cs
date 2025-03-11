namespace BCSH2_SEM.Methods
{
    using BCSH2_SEM.Database;
    using BCSH2_SEM.Models;
    public class RecommendMealPlan
    {
        private readonly DatabaseService _db;
        private readonly EvaluateMealPlan evaluateMealPlan;
        
        /*
         Snídaně: 20 %

        Dopolední svačina: 10 %

        Oběd: 35 %

        Odpolední svačina: 10 %

        Večeře: 25 %
         */
        private  double BreakfastPortion;
        private  double LunchPortion;
        private double DinnerPortion;
        private double FirstSnackPortion;
        private double SecondSnackPortion;

        private int NumberOfMeals;

        private readonly int IndexOfBreakfast = 0;
        private readonly int IndexOfLunch = 1;
        private readonly int IndexOfDinner = 2;
        private readonly int IndexOfFirstSnack = 3;
        private readonly int IndexOfSecondSnack = 4;


        public RecommendMealPlan(int numberOfMeals)
        {
            NumberOfMeals = numberOfMeals;
           
            if (NumberOfMeals == 3)
            {
                BreakfastPortion = 0.275;
                LunchPortion = 0.375;
                DinnerPortion = 0.35;
            }
            else if (NumberOfMeals == 4)
            {
                BreakfastPortion = 0.25;
                LunchPortion = 0.35;
                DinnerPortion = 0.3;
                FirstSnackPortion = 0.1;
            
            } else if(NumberOfMeals == 5)
            {
                BreakfastPortion = 0.25;
                LunchPortion = 0.3;
                DinnerPortion = 0.25;
                FirstSnackPortion = 0.1;
                SecondSnackPortion = 0.1;
            } else
            {
                throw new InvalidOperationException("Neplatný počet jídel v jídelníčku");
            }

                _db = new();
            evaluateMealPlan = new();
        }



        /*
         Postup doporučování: oběd - večeře - snídaně - (svačina) - (svačina)
         */
        public MealPlan RecommendPlan(MealPlan mealPlan, User user)
        {
            double remainingCalories = user.DailyCalories - user.ConsumedCalories;
            double remainingCarbs = user.DailyCarbs - user.ConsumedCarbs;
            double remainingFats = user.DailyFats - user.ConsumedFats;
            double remainingProteins = user.DailyProteins - user.ConsumedProteins;

            // Ingredient idealLunch = new Ingredient();
            //idealLunch.Calories = 0.35 * remainingCalories;
            //idealLunch.Carbs = 0.35 * remainingCarbs;
            //idealLunch.Proteins = 0.35 * remainingProteins;
            //idealLunch.Fats = 0.35 * remainingFats;

            // Oběd - jestli není null
            /*
            if (mealPlan.DaliyMeals[IndexOfLunch] == null)
            {
               var meal =  RecommendMeal(LunchPortion, remainingCalories, remainingCarbs, remainingProteins, remainingFats, "Lunch");
                mealPlan.SetMeal(IndexOfLunch, meal);
            }
            */

            List<int> orderOfReccomending = new() {IndexOfLunch, IndexOfDinner, IndexOfBreakfast }; // List [1, 2, 0, 3, 4]
            List<double> sizesOfPortions = new() {LunchPortion, DinnerPortion, BreakfastPortion };

            if (mealPlan.NumberOfMeals >= 4) //Plus jedna svačina
            {
                orderOfReccomending.Add(IndexOfFirstSnack);
                sizesOfPortions.Add(FirstSnackPortion);
            }

            if (mealPlan.NumberOfMeals == 5) //Plus dvě svačiny
            {
                orderOfReccomending.Add(IndexOfSecondSnack);
                sizesOfPortions.Add(SecondSnackPortion);
            }

            

            for (int i = 0; i < orderOfReccomending.Count(); i++)
            {
                if (mealPlan.DaliyMeals[orderOfReccomending[i]] == null)
                {
                    var meal = RecommendMeal(sizesOfPortions[i], remainingCalories, remainingCarbs, remainingProteins, remainingFats, "");
                    mealPlan.SetMeal(orderOfReccomending[i], meal);
                    remainingCalories -= meal.Recipe.TotalCalories * meal.Portion;
                     remainingCarbs -= meal.Recipe.TotalCarbs * meal.Portion;
                    remainingFats -= meal.Recipe.TotalFats * meal.Portion;
                    remainingProteins -= meal.Recipe.TotalProteins * meal.Portion;

                }
            }

            return mealPlan;

        }


        //Snídaně = vše co není vhodné jako oběd
        //Oběd = vše co je vhodné jako oběd
        //Svačina = vše co není vhodné jako oběd
        //Večeře = vše
        private MealPortion RecommendMeal(double portionOfDailyIntake, double remainingCalories, double remainingCarbs, double remainingProteins, double remainingFats, string nameOfMeal)
        {
            double idealCalories = portionOfDailyIntake * remainingCalories;
            double idealCarbs = portionOfDailyIntake * remainingCarbs;
            double idealProteins = portionOfDailyIntake * remainingProteins;
            double idealFats = portionOfDailyIntake * remainingFats;

            MealPortion bestMeal = null;
            double evaluationOfBestMeal = 4; // TODO: zavést jako konstantu

            List<Recipe> list = null;

            if (nameOfMeal == "Breakfast")
            {
                list = _db.Recipes.Find(r => !r.SuitableAsLunch).ToList();
            } else if(nameOfMeal == "Lunch")
            {
                list = _db.Recipes.Find(r => r.SuitableAsLunch).ToList();
            } else
            {
                list = _db.Recipes.FindAll().ToList();
            }

           


            foreach (var recipe in list)
            {
                double portion = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    MealPortion mealPortion = new(portion, recipe);
                    double result = evaluateMealPlan.EvaluateMeal(mealPortion, idealCalories, idealCarbs, idealProteins, idealFats);



                    if (result < evaluationOfBestMeal)
                    {
                        evaluationOfBestMeal = result;
                        bestMeal = mealPortion;
                    }

                    portion += 0.5;
                }
            }

            return bestMeal;
        }
    }
}
