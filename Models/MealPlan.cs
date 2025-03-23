namespace BCSH2_SEM.Models

{
    using BCSH2_SEM.Models;
    // Třída, která v sobě  uchovává informace  o uživatelském jídelníčku (již sněděné i navrhované recepty)
    public class MealPlan
    {
        //Breakfast - index = 0
        //Lunch - index = 1
        //Dinner - index = 2
        //First Snack - index = 3
        //Second Snack - index = 4

       

        public int NumberOfMeals { get; } // KOlik chce uživatel porcí denně

        public List<MealPortion> DailyMeals;

        public int NumberOfConsumedMeals { get; set; }

        public MealPlan(int numberOfMeals)
        {
            NumberOfMeals = numberOfMeals;
            DailyMeals = new List<MealPortion>(new MealPortion[numberOfMeals]);
        }




        //Nastav recept - na index nastav recept
        // Bude to užitečné při vyhledávání (na změnu např. svačiny)   
        public void SetMeal(int mealIndex, MealPortion recipe)
        {
            //TODO: Dodělat podmínku
            //if (mealIndex + 1 < NumberOfConsumedMeals || mealIndex + 1 >= NumberOfMeals ) //nesmí být menší než počet snědených jídel
            //{
            //   throw new ArgumentOutOfRangeException("Neplatný index pro zadávání jídla"); //TODO: Časem rozdělit na dvě výjimky
            //}

            DailyMeals[mealIndex] = recipe;
        }

       
        
    }

    
}
