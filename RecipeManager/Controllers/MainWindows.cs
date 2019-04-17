﻿using System.Collections.Generic;
using RecipeManager.Models;


namespace RecipeManager.Controllers
{
    class MainWindows
    {
        private int i = 0;
        public List<Category> GetCategories()
        {
            // THis hould containt a DB model call
            List<Category> list = new List<Category>();

            for (int j = 1; j < 25; j++, i++)
            {
                list.Add(new Category(){Title = $"Katogri {i}", Id = i});
            }

            return list;
        }

        public List<Recipe> GetRecipes(Category category)
        {
            List<Recipe> list = new List<Recipe>();

            for (int j = 1; j < 25; j++, i++)
            {
                list.Add(new Recipe() { Title = $"Katogri {i}", Id = i, Description = " Lorm Bacon Imspum Sadel med mere...."});
            }

            return list;
        }
    }

    /// <summary>
    /// This class stores the loaded categories.
    /// </summary>
    public class Category
    {
        public string Title { get; set; }
        public int Id { get; set; }
    }
}
