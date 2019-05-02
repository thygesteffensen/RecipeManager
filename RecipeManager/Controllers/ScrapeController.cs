﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Controllers
{
    public class ScrapeController
    {
        private Scrape scrape;
        private SqlConnection sqlConnection;

        string recipeName;
        List<string> recipeDescription = new List<string>();
        List<string> ingredietnsListlist = new List<string>();

        public ScrapeController(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;

            scrape = new Scrape(sqlConnection, this);
            scrape.SetContentCategoryDropdown(GetRecipeCategories());
            scrape.ShowDialog();
        }

        public void ScrapeWebsite(string url)
        {
            string line = string.Empty;

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var regexStart = new System.Text.RegularExpressions.Regex("<h2 itemprop=\"name\"(.*?)>(.*?)</h2>");
                var regexEnd = new System.Text.RegularExpressions.Regex("after-recipe");

                // Retrieving the important lines
                List<string> lines = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    if (regexStart.IsMatch(line))
                    {
                        do
                        {
                            lines.Add(line);
                            if (regexEnd.IsMatch(line))
                            {
                                break;
                            }
                        } while ((line = reader.ReadLine()) != null);
                    }
                }

                RetrieveRecipe(lines);
                VerifyIngredients();
                Console.WriteLine("Done");
            }
        }

        private void VerifyIngredients()
        {
            List<CommodityShadow> shadowList = new List<CommodityShadow>();

            var length = ingredietnsListlist.Count;
            var index = 0;
            foreach (var ingredient in ingredietnsListlist)
            {
                string ingredient_stuff = ingredient.Split(',')[0]; // Remove text after comma
                var list_list = ingredient_stuff.Split(' ');

                double value = StringToDouble(list_list[0]);
                


                index++;
            }
        }

        private double StringToDouble(string input)
        {
            Regex re = new Regex(@"^\s*(\d+)(\s*\.(\d*)|\s+(\d+)\s*/\s*(\d+))?\s*$");
            string str = " 9  1/ 2 ";
            Match m = re.Match(str);
            double value = m.Groups[1].Success ? double.Parse(m.Groups[1].Value) : 0.0;

            if (m.Groups[3].Success)
            {
                value += double.Parse("0." + m.Groups[3].Value);
            }
            else
            {
                value += double.Parse(m.Groups[4].Value) / double.Parse(m.Groups[5].Value);
            }

            return value;
        }

        private void RetrieveRecipe(List<string> lines)
        {
            var regexDescription = new System.Text.RegularExpressions.Regex("itemprop=\"recipeInstructions\"");
            var regexRecipeIngredients = new System.Text.RegularExpressions.Regex("ingredientlist");
            var regexRecipeIngredientsElements = new System.Text.RegularExpressions.Regex("recipeIngredient");
            var regexRecipeIngredientsEnd = new System.Text.RegularExpressions.Regex("</ul>");
            var regexRecipeDescriptionEnd = new System.Text.RegularExpressions.Regex("</div>");

            int i = 0;
            Match match;
            foreach (var line in lines)
            {
                i++;
                // Retrieving recipe name
                var regexRecipeName = new Regex("<h2 itemprop=\"name\"(.*?)>(.*?)</h2>");

                match = regexRecipeName.Match(line);
                if (match.Success)
                {
                    recipeName = StripTagsRegex(match.ToString());
                }

                // Retrieving the description
                match = regexDescription.Match(line);
                if (match.Success)
                {
                    string description = line.Substring(match.Index);
                    foreach (string lineInner in lines.GetRange(i, lines.Count - 1))
                    {
                        description += lineInner;
                        if (regexRecipeDescriptionEnd.IsMatch(lineInner))
                        {
                            break;
                        }
                    }

                    var regexParagraf = new Regex("<p>(.*?)</p>");
                    foreach (Match itemMatch in regexParagraf.Matches(description))
                    {
                        recipeDescription.Add(StripTagsRegex(itemMatch.ToString()));
                    }
                }

                // Retrieving the ingredients
                match = regexRecipeIngredients.Match(line);
                if (match.Success)
                {
                    string ingredients = line.Substring(match.Index);
                    foreach (string lineInner in lines.GetRange(i, lines.Count - 1))
                    {
                        ingredients += lineInner;
                        if (regexRecipeIngredientsElements.IsMatch(lineInner))
                        {
                            break;
                        }
                    }

                    var regexParagraf = new Regex("<li (.*?)>(.*?)</li>");
                    foreach (Match itemMatch in regexParagraf.Matches(ingredients))
                    {
                        ingredietnsListlist.Add(StripTagsRegex(itemMatch.ToString()));
                    }
                }
            }
        }

        public List<RecipeCategory> GetRecipeCategories()
        {
            RecipeCategoryController recipeCategoryController = new RecipeCategoryController(sqlConnection);
            return recipeCategoryController.GetRecipeCategories();
        }

        public string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}