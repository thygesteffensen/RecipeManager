using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeManager.Controllers
{
    class ScrapeController
    {
        public ScrapeController(SqlConnection sqlConnection)
        {
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
                Console.WriteLine("Done");
            }
        }

        public void RetrieveRecipe(List<string> lines)
        {
            string recipeName;
            List<string> recipeDescription = new List<string>();
            var regexDescription = new System.Text.RegularExpressions.Regex("itemprop=\"recipeInstructions\"");
            var regexRecipeIngredients = new System.Text.RegularExpressions.Regex("ingredientlist");
            var regexRecipeIngredientsElements = new System.Text.RegularExpressions.Regex("recipeIngredient");
            var regexRecipeIngredientsEnd = new System.Text.RegularExpressions.Regex("</ul>");

            foreach (var line in lines)
            {
                Match match;
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
                    foreach(string lineInner in lines)
                    {
                        description += lineInner;
                        var regexExit = new Regex("</div>");
                        if (regexExit.IsMatch(lineInner))
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
                List<string> ingredietnsListlist = new List<string>();
                match = regexRecipeIngredients.Match(line);
                if (match.Success)
                {
                    string ingredients = line.Substring(match.Index);
                    do
                    {
                        ingredients += line;
                        if (regexRecipeIngredientsElements.IsMatch(line))
                        {
                            break;
                        }
                    } while (regexRecipeIngredientsEnd.IsMatch(line));

                    var regexParagraf = new Regex("<li (.*?)>(.*?)</li>");
                    foreach (Match itemMatch in regexParagraf.Matches(ingredients))
                    {
                        ingredietnsListlist.Add(StripTagsRegex(itemMatch.ToString()));
                    }
                }
            }
        }

        public string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}