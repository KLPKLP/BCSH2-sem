using BCSH2_SEM.Models;

namespace BCSH2_SEM.Methods
{
    public class CaloriesCalculator
    {
        /*
         * BMR for Men = 66.47 + (13.75 * weight [kg]) + (5.003 * size [cm]) − (6.755 * age [years])
        BMR for Women = 655.1 + (9.563 * weight [kg]) + (1.85 * size [cm]) − (4.676 * age [years])
        */

        //source: https://www.diabetes.co.uk/bmr-calculator.html
        /// <summary>
        /// Calculates BMR based on weight, height, age and gender
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        /// <param name="dateOfBirth"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public double BMR(double weight, double height, DateTime dateOfBirth, Gender gender)
        {
            double BMR = 0;
            double age = (DateTime.Today - dateOfBirth).TotalDays / 365.25; //přestupný rok

            switch (gender)
            {
                case Gender.Male:
                    BMR = 66.47 + (13.75 * weight) + (5.003 * height) - (6.755 * age);
                    break;

                case Gender.Female:
                    BMR = 655.1 + (9.563 * weight) + (1.85 * height) - (4.676 * age);
                    break;
            }

            return BMR;
        }

        // source: https://calc.kaloricketabulky.cz/
        /// <summary>
        /// Calculates amount of calories based on BMR and users goal
        /// </summary>
        /// <param name="BMR"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public double CalorieIntake(double BMR, Goal goal)
        {
            double bmr;
            switch (goal)
            {
                case Goal.Lose:
                    bmr = BMR * 0.85;
                    break;

                case Goal.Gain:
                    bmr = BMR * 1.1;
                    break;

                default:
                    bmr = BMR;
                    break;


            }
            return bmr;

        }

        //source: https://calc.kaloricketabulky.cz/
        // source: https://www.nal.usda.gov/programs/fnic
        /// <summary>
        /// Calculates how many grams of carbs, fats and proteins user can consume
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="calories"></param>
        /// <param name="protein"></param>
        /// <param name="carb"></param>
        /// <param name="fat"></param>
        public void Nutritions(Goal goal, double calories, out double protein, out double carb, out double fat)
        {
            double proteinCalories = 0;
            double fatCalories = 0;
            double carbCalories = 0;

            switch (goal)
            {
                case Goal.Lose:
                    proteinCalories = calories * 0.25;
                    carbCalories = calories * 0.43;
                   fatCalories = calories * 0.29;
                    break;

                case Goal.Gain:
                    proteinCalories = calories * 0.25;
                    carbCalories = calories * 0.49;
                    fatCalories = calories * 0.26;
                    break;


                default:
                    proteinCalories = calories * 0.28;
                    carbCalories = calories * 0.47;
                    fatCalories = calories * 0.28;
                    break;
            }

            double carbCaloriesPerGram = 4;
            double fatCaloriesPerGram = 9;
            double proteinCaloriesPerGram = 4;


            protein = proteinCalories / proteinCaloriesPerGram;
            carb =carbCalories / carbCaloriesPerGram;
            fat = fatCalories / fatCaloriesPerGram;
        }

    }




}
