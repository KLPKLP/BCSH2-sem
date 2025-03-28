﻿namespace BCSH2_SEM.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RecipeIngredient> Ingredients { get; set;} = new List<RecipeIngredient>();
        public string Description {  get; set; }
        public bool SuitableAsLunch { get; set; }
        public bool SuitableAsDinner { get; set; }
        public bool SuitableAsBreakfast { get; set; }
        public bool SuitableAsSnack { get; set; }

        public double TotalCalories => Ingredients.Sum(i => i.Ingredient.Calories * i.Quantity / 100);
        public double TotalProteins => Ingredients.Sum(i => i.Ingredient.Proteins * i.Quantity / 100);
        public double TotalCarbs => Ingredients.Sum(i => i.Ingredient.Carbs * i.Quantity / 100);
        public double TotalFats => Ingredients.Sum(i => i.Ingredient.Fats * i.Quantity / 100);


    }
}
