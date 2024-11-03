namespace BCSH2_SEM.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set;} = new List<Ingredient>();
        public string Popis {  get; set; }
    }
}
