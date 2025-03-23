namespace BCSH2_SEM.Methods
{
    using BCSH2_SEM.Database;
    using BCSH2_SEM.Models;
    public class RecommendMealPlan
    {
        private readonly DatabaseService _db;
        private readonly EvaluateMealPlan evaluateMealPlan;
        private readonly Dictionary<int, Func<IEnumerable<Recipe>>> mealFilters;
        
        /*
         Snídaně: 20 %

        Dopolední svačina: 10 %

        Oběd: 35 %

        Odpolední svačina: 10 %

        Večeře: 25 %
         */
        private double BreakfastPortion;
        private double LunchPortion;
        private double DinnerPortion;
        private double FirstSnackPortion;
      private double SecondSnackPortion; // TODO: Vymazat?

        private int NumberOfMeals;

        private readonly int IndexOfBreakfast = 0;
        private readonly int IndexOfLunch = 1;
        private readonly int IndexOfDinner = 2;
        private readonly int IndexOfFirstSnack = 3;
        private readonly int IndexOfSecondSnack = 4;

        List<int> mealTypes = new();


        public RecommendMealPlan(int numberOfMeals)
        {
            NumberOfMeals = numberOfMeals;
           
            if (NumberOfMeals == 3)
            {
                BreakfastPortion = 0.275;
                LunchPortion = 0.375;
                DinnerPortion = 0.35;

                mealTypes.Add(IndexOfBreakfast);
                mealTypes.Add(IndexOfLunch);
                mealTypes.Add(IndexOfDinner);
            }
            else if (NumberOfMeals == 4)
            {
                BreakfastPortion = 0.25;
                LunchPortion = 0.35;
                DinnerPortion = 0.3;
                FirstSnackPortion = 0.1;
            
                mealTypes.Add(IndexOfBreakfast);
                mealTypes.Add(IndexOfFirstSnack);
                mealTypes.Add(IndexOfLunch);
                mealTypes.Add(IndexOfDinner);

            }
            else if (NumberOfMeals == 5)
            {
                BreakfastPortion = 0.25;
                LunchPortion = 0.35;
                DinnerPortion = 0.3;
                FirstSnackPortion = 0.05;
                SecondSnackPortion = 0.05; //TODO: Ještě zjistit

                mealTypes.Add(IndexOfBreakfast);
                mealTypes.Add(IndexOfFirstSnack);
                mealTypes.Add(IndexOfLunch);
                mealTypes.Add(IndexOfSecondSnack);
                mealTypes.Add(IndexOfDinner);
            }
            else
            {
                throw new InvalidOperationException("Neplatný počet jídel v jídelníčku");
            }

                _db = new();
            evaluateMealPlan = new();
            mealFilters = new()
            {
                //{ IndexOfBreakfast, () => _db.Recipes.Find(r => !r.SuitableAsLunch) },  // Snídaně
                //{ IndexOfFirstSnack, () => _db.Recipes.Find(r => !r.SuitableAsLunch) }, // První svačina
                //{ IndexOfSecondSnack, () => _db.Recipes.Find(r => !r.SuitableAsLunch) }, // Druhá svačina
                //{ IndexOfLunch, () => _db.Recipes.Find(r => r.SuitableAsLunch) }, // Oběd
                //{ IndexOfDinner, () => _db.Recipes.FindAll() } // Večeře


                { IndexOfBreakfast, () => _db.Recipes.Find(r => r.SuitableAsBreakfast) },  // Snídaně
                { IndexOfFirstSnack, () => _db.Recipes.Find(r => r.SuitableAsSnack) }, // První svačina
                { IndexOfSecondSnack, () => _db.Recipes.Find(r => r.SuitableAsSnack) }, // Druhá svačina
                { IndexOfLunch, () => _db.Recipes.Find(r => r.SuitableAsLunch) }, // Oběd
                { IndexOfDinner, () => _db.Recipes.Find(r => r.SuitableAsDinner) } // Večeře

            };
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

            List<int> orderOfReccomending = new() { IndexOfLunch, IndexOfDinner, IndexOfBreakfast }; // List [1, 2, 0, 3, 4]
            List<double> sizesOfPortions = new() { LunchPortion, DinnerPortion, BreakfastPortion };

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
                int mealIndex = orderOfReccomending[i];

                if (mealPlan.DailyMeals[mealIndex] == null)
                {
                    var meal = RecommendMeal(sizesOfPortions[i], remainingCalories, remainingCarbs, remainingProteins, remainingFats, mealFilters[mealIndex]);
                    mealPlan.SetMeal(mealIndex, meal);
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
        private MealPortion RecommendMeal(double portionOfDailyIntake, double remainingCalories, double remainingCarbs, double remainingProteins, double remainingFats, Func<IEnumerable<Recipe>> getRecipes)
        {
            double idealCalories = portionOfDailyIntake * remainingCalories;
            double idealCarbs = portionOfDailyIntake * remainingCarbs;
            double idealProteins = portionOfDailyIntake * remainingProteins;
            double idealFats = portionOfDailyIntake * remainingFats;

            MealPortion bestMeal = null;
            double evaluationOfBestMeal = double.MaxValue;

            List<Recipe> listOfRecipes = getRecipes().ToList();

            //if (nameOfMeal == "Breakfast")
            //{
            //    list = _db.Recipes.Find(r => !r.SuitableAsLunch).ToList();
            //} else if(nameOfMeal == "Lunch")
            //{
            //    list = _db.Recipes.Find(r => r.SuitableAsLunch).ToList();
            //} else
            //{
            //    list = _db.Recipes.FindAll().ToList();
            //}

           


            foreach (var recipe in listOfRecipes)
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

        public MealPlan RecommendPlanAnnealing(MealPlan mealPlan, User user)
        {
            double remainingCalories = user.DailyCalories - user.ConsumedCalories;
            double remainingCarbs = user.DailyCarbs - user.ConsumedCarbs;
            double remainingFats = user.DailyFats - user.ConsumedFats;
            double remainingProteins = user.DailyProteins - user.ConsumedProteins;



            List<MealPortion> currentPlan = new();
            Random rand = new();

            //Generování náhodného jídelníčku na začátek
            for (int i = 0; i < mealTypes.Count; i++) 
            {
                int mealIndex = mealTypes[i];
                var listOfRecipes = mealFilters[mealIndex]().ToList();
                if (listOfRecipes.Count == 0)
                {
                    return null;
                }

                if (mealPlan.DailyMeals[i] == null)
                {

                }

                //1 + rand 6 / 2d
                double portion = rand.Next(1,7) / 2d; //číslo od 1 do 6, děleno 2 (porce 0,5 až 3)

           //     double portion = (TriangularDistribution(rand, 0.5, 3.0, 1.0) * 2) / 2.0; // preferuje 1.0

                currentPlan.Add(new MealPortion(portion, listOfRecipes[rand.Next(listOfRecipes.Count)]));
            }

            double temperature = 500.0;
            double coolingRate = 0.95;
            int iterations = 500;

            for (int i = 0; i < iterations; i++)
            {
                List<MealPortion> newPlan = new(currentPlan);

        

                int randomIndex = rand.Next(newPlan.Count);//vyberu, jaké jídlo změním

                var listOfRecipes = mealFilters[mealTypes[randomIndex]](); //typ jídla na indexu (potřebuju se zeptat, na jaký 
               
                
                //Všechny hodnoty stejná pravděpodobnost
               double portion = rand.Next(1, 7) / 2d; //Problém - vygeneruje se např 4, to je index svačiny, ale v newPlan je to večeře, protože tam je to v pořadí


              



                newPlan[randomIndex] = new MealPortion(portion, listOfRecipes.ElementAt(rand.Next(listOfRecipes.Count())));

                var testReceptu = newPlan[randomIndex];

                double currentScore = evaluateMealPlan.EvaluateMealPlanAnnealing(currentPlan, remainingCalories, remainingCarbs, remainingProteins, remainingFats);
                double newScore = evaluateMealPlan.EvaluateMealPlanAnnealing(newPlan, remainingCalories, remainingCarbs, remainingProteins, remainingFats);


                //Console.WriteLine(newScore);

                //Penalizace duplicit:
                var recipeIds = newPlan.Select(mp => mp.Recipe.Id);
                int totalCount = recipeIds.Count();
                int uniqueCount = recipeIds.Distinct().Count();
                int duplicateCount = totalCount - uniqueCount;

                
                double duplicityPenalty = duplicateCount * 0.5;
                




                double portionPenalty = 0;
                ////Penalizace velikosti porce
                //for (int j = 0; j < newPlan.Count; j++)
                //{
                //    double expectedPortion = 0;

                //    if (mealTypes[j] == IndexOfBreakfast)
                //    {
                //        expectedPortion = remainingCalories * BreakfastPortion;
                //    }
                //    else if (mealTypes[j] == IndexOfLunch)
                //    {
                //        expectedPortion = remainingCalories * LunchPortion;
                //    }
                //    else if (mealTypes[j] == IndexOfDinner)
                //    {

                //        expectedPortion = remainingCalories * DinnerPortion;
                //    }
                //    else if (mealTypes[j] == IndexOfFirstSnack || mealTypes[j] == IndexOfSecondSnack)
                //    {

                //        expectedPortion = remainingCalories * FirstSnackPortion;
                //    }

                //    //double portionDifference = Math.Abs((newPlan[randomIndex].Recipe.TotalCalories * newPlan[randomIndex].Portion) - expectedPortion); //Asi ne - počítám přece jen s tím jedním receptem

                //    double portionDifference = Math.Abs((newPlan[j].Recipe.TotalCalories * newPlan[j].Portion) - expectedPortion); //Asi ne - počítám přece jen s tím jedním receptem
                //    portionPenalty += portionDifference * 0.0001;
                //}




                double expectedPortion = 0;

                if (mealTypes[randomIndex] == IndexOfBreakfast)
                {
                    expectedPortion = remainingCalories * BreakfastPortion;
                }
                else if (mealTypes[randomIndex] == IndexOfLunch)
                {
                    expectedPortion = remainingCalories * LunchPortion;
                }
                else if (mealTypes[randomIndex] == IndexOfDinner)
                {

                    expectedPortion = remainingCalories * DinnerPortion;
                }
                else if (mealTypes[randomIndex] == IndexOfFirstSnack || mealTypes[randomIndex] == IndexOfSecondSnack)
                {

                    expectedPortion = remainingCalories * FirstSnackPortion;
                }


                    double portionDifference = Math.Abs((newPlan[randomIndex].Recipe.TotalCalories * newPlan[randomIndex].Portion) - expectedPortion); //Asi ne - počítám přece jen s tím jedním receptem
                    portionPenalty += portionDifference * 0.01; //0.001

              //  Console.WriteLine("Očekávaná porce " + expectedPortion + " opravdová porce" + newPlan[randomIndex].Recipe.TotalCalories * newPlan[randomIndex].Portion + " penalta " + portionPenalty);


                newScore += duplicityPenalty + portionPenalty;

                if (newScore < currentScore || Math.Exp((currentScore - newScore) / temperature) > rand.NextDouble())
                {
                    currentPlan = newPlan;
                }

                temperature *= coolingRate;
            }

         







      



            for (int i = 0; i < mealTypes.Count; i++)
            {
                mealPlan.SetMeal(i, currentPlan[i]);
            }


            Console.WriteLine($"Snídaně: {remainingCalories * BreakfastPortion} svačina: {remainingCalories * FirstSnackPortion} oběd:  {remainingCalories * LunchPortion} svačina: {remainingCalories * FirstSnackPortion} večeře: {remainingCalories * DinnerPortion}");

            return mealPlan;
        }








        public MealPlan RecommendPlanAnnealingZaloha(MealPlan mealPlan, User user)
        {
            double remainingCalories = user.DailyCalories - user.ConsumedCalories;
            double remainingCarbs = user.DailyCarbs - user.ConsumedCarbs;
            double remainingFats = user.DailyFats - user.ConsumedFats;
            double remainingProteins = user.DailyProteins - user.ConsumedProteins;

            //   List<int> orderOfReccomending = new() { IndexOfLunch, IndexOfDinner, IndexOfBreakfast }; // List [1, 2, 0, 3, 4] nepotřebujem pořadí
            //   List<double> sizesOfPortions = new() { LunchPortion, DinnerPortion, BreakfastPortion };

            //if (mealPlan.NumberOfMeals >= 4) //Plus jedna svačina
            //{
            //    //orderOfReccomending.Add(IndexOfFirstSnack);
            //    sizesOfPortions.Add(FirstSnackPortion);
            //}

            //if (mealPlan.NumberOfMeals == 5) //Plus dvě svačiny
            //{
            ////    orderOfReccomending.Add(IndexOfSecondSnack);
            //    sizesOfPortions.Add(SecondSnackPortion);
            //}


            List<MealPortion> currentPlan = new();
            Random rand = new();

            //Generování náhodného jídelníčku na začátek
            foreach (int mealIndex in mealTypes)
            {
                var listOfRecipes = mealFilters[mealIndex]().ToList();
                if (listOfRecipes.Count == 0)
                {
                    return null;
                }


                //1 + rand 6 / 2d
                double portion = rand.Next(1, 7) / 2d; //číslo od 1 do 6, děleno 2 (porce 0,5 až 3)

                //     double portion = (TriangularDistribution(rand, 0.5, 3.0, 1.0) * 2) / 2.0; // preferuje 1.0

                currentPlan.Add(new MealPortion(portion, listOfRecipes[rand.Next(listOfRecipes.Count)]));
            }

            double temperature = 500.0;
            double coolingRate = 0.95;
            int iterations = 500;

            for (int i = 0; i < iterations; i++)
            {
                List<MealPortion> newPlan = new(currentPlan);

                //int randomIndex = rand.Next(newPlan.Count);//vyberu, jaké jídlo změním

                //var listOfRecipes = mealFilters[randomIndex](); //typ jídla na indexu (potřebuju se zeptat, na jaký 
                //double portion = rand.Next(1, 7) / 2d; //Problém - vygeneruje se např 4, to je index svačiny, ale v newPlan je to večeře, protože tam je to v pořadí
                //newPlan[randomIndex] = new MealPortion(portion, listOfRecipes.ElementAt(rand.Next(listOfRecipes.Count())));

                int randomIndex = rand.Next(newPlan.Count);//vyberu, jaké jídlo změním

                var listOfRecipes = mealFilters[mealTypes[randomIndex]](); //typ jídla na indexu (potřebuju se zeptat, na jaký 


                //Všechny hodnoty stejná pravděpodobnost
                double portion = rand.Next(1, 7) / 2d; //Problém - vygeneruje se např 4, to je index svačiny, ale v newPlan je to večeře, protože tam je to v pořadí


                //Vážený seznam:

                /*
                List<double> weightedPortions = new()
                        {
                            1.0, 1.0, 1.0, 1.0, 1.0,// 4×
                            1.5, 1.5, 1.5, 1.5,     // 3×
                            0.5, 0.5, 0.5,         // 2×
                            2.0, 2.0,              // 1×
                            2.5, 3.0            // 1× každá
                        };

                // Náhodný výběr s preferencí nižších porcí
                double portion = weightedPortions[rand.Next(weightedPortions.Count)];
                */


                //Trojúhelníkové rozdělení:
                // double portion = (TriangularDistribution(rand, 0.5, 3.0, 1.0)* 2 ) / 2.0; // preferuje 1.0



                newPlan[randomIndex] = new MealPortion(portion, listOfRecipes.ElementAt(rand.Next(listOfRecipes.Count())));

                double currentScore = evaluateMealPlan.EvaluateMealPlanAnnealing(currentPlan, remainingCalories, remainingCarbs, remainingProteins, remainingFats);
                double newScore = evaluateMealPlan.EvaluateMealPlanAnnealing(newPlan, remainingCalories, remainingCarbs, remainingProteins, remainingFats);

                if (newScore < currentScore || Math.Exp((currentScore - newScore) / temperature) > rand.NextDouble())
                {
                    currentPlan = newPlan;
                }

                temperature *= coolingRate;
            }









            //  Kontrola duplicity receptů
            var uniqueRecipes = currentPlan.Select(mp => mp.Recipe.Id).Distinct();


            //Výpis do konzole:
            /*
            //------------------------------------
            // Výpis všech ID receptů v aktuálním plánu
            Console.WriteLine("Všechna ID receptů v aktuálním plánu:");
            foreach (var id in currentPlan.Select(mp => mp.Recipe.Id))
            {
                Console.WriteLine($"Recept ID: {id}");
            }

            // Výpis pouze unikátních ID receptů
            Console.WriteLine("Unikátní ID receptů:");
            foreach (var id in uniqueRecipes)
            {
                Console.WriteLine($"Recept ID: {id}");
            }

            // Pro kontrolu - je tam duplicita?
            if (uniqueRecipes.Count() != currentPlan.Count)
            {
                Console.WriteLine("Duplicitní recepty detekovány.");
            }
            else
            {
                Console.WriteLine("Všechny recepty jsou unikátní.");
            }
            ///-----------------------------------
            ///*/

            if (uniqueRecipes.Count() != currentPlan.Count)
            {
                // Duplicitní recept – spuštění znovu
                return RecommendPlanAnnealing(mealPlan, user);
            }



            for (int i = 0; i < mealTypes.Count; i++)
            {
                mealPlan.SetMeal(i, currentPlan[i]);
            }


            return mealPlan;
        }





        double TriangularDistribution(Random rand, double min, double max, double mode)
        {
            double u = rand.NextDouble();
            double c = (mode - min) / (max - min);

            if (u < c)
            {
                return min + Math.Sqrt(u * (max - min) * (mode - min));
            }
            else
            {
                return max - Math.Sqrt((1 - u) * (max - min) * (max - mode));
            }
        }
    }
}
