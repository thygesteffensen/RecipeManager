using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeManager.Models
{
    class RecipeModel
    {
        private List<Recipe> _listRecipes = new List<Recipe>();

        public List<Recipe> GetRecipes()
        {
            return _listRecipes;
        }

        public void SaveRecipe(Recipe recipe)
        {
            _listRecipes.Add(recipe);
        }
    }

    public class Recipe
    {
        public List<Comodity> Comodities { get; set; }
        public string Title { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class Comodity
    {
        public string ComodityName { get; set; }
        public int Id { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return $"{Value} {Unit} {ComodityName} ({Id})";
        }
    }
}
