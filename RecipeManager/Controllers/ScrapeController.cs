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
using System.Windows;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Controllers
{
    public class ScrapeController
    {
        private Scrape scrape;
        private SqlConnection sqlConnection;
        private CommodityModel _commodityModel;
        private RecipeModel _recipeModel;
        private RCModel _rcModel;
        private RecipeCommodityModel _recipeCommodityModel;

        string recipeName;
        List<string> recipeDescription = new List<string>();
        List<string> ingredietnsListlist = new List<string>();

        public ScrapeController(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
            this._commodityModel = new CommodityModel(sqlConnection);
            this._recipeModel = new RecipeModel(sqlConnection);
            this._rcModel = new RCModel(sqlConnection);
            this._recipeCommodityModel = new RecipeCommodityModel(sqlConnection);

            scrape = new Scrape(sqlConnection, this);
            scrape.SetContentCategoryDropdown(GetRecipeCategories());
            scrape.ShowDialog();
        }

        public List<Commodity> GetCommodities()
        {
            return _commodityModel.GetCommodities();
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
            Regex re = new Regex(@"^([\d]*)?");

            List<CommodityShadowConfirmed> shadowList = new List<CommodityShadowConfirmed>();

            var length = ingredietnsListlist.Count;
            var index = 0;
            foreach (var ingredient in ingredietnsListlist)
            {
                // Creating the shadow object
                CommodityShadowConfirmed shadowObject = new CommodityShadowConfirmed();

                var ingredientStuff = ingredient.Split(',')[0]; // Remove text after comma
                Match m = re.Match(ingredientStuff);
                if (m.Groups[1].Length == 0)
                {
                    // Now we have something like "salt" or "frisk peber" which do not have value or unit :(
                    shadowObject.Name = ingredientStuff;
                    shadowObject.Value = 1;
                    shadowObject.Unit = Units.stk;
                    continue;
                }

                var listList = ingredientStuff.Split(' '); // Splitting the string


                // Determine the commodity
                var commodityName = string.Join(" ", listList.Skip(2));
                List<Commodity> commodities = _commodityModel.GetCommodities(partialName: commodityName);
                if (commodities.Count == 1)
                {
                    // There is only one result, thus we have found the right commodity.
                    shadowObject.Commodity = commodities[0];
                    shadowObject.ConfirmedCommodity = true;
                }
                else
                {
                    shadowObject.Name = commodityName;
                }

                // Determine the value
                shadowObject.Value = StringToDouble(listList[0]);

                // Determine the unit
                var unitString = listList[1];
                var unitDefined = Enum.TryParse<Units>(unitString, out var unit);
                shadowObject.ConfirmedUnit = unitDefined;
                if (unitDefined)
                {
                    shadowObject.Unit = unit;
                }

                shadowObject.UnitString = unitString;

                shadowList.Add(shadowObject);
            }

            scrape.ConfirmIngredient(shadowList);
        }

        public void StoreRecipe(List<CommodityShadowConfirmed> list, RecipeCategory recipeCategory)
        {
            // Store it here
            // Creating some recipies
            string description = "";
            foreach (var block in recipeDescription)
            {
                description += block + "\n";
            }

            Recipe recipe = _recipeModel.CreateRecipe(recipeName, description);


            // Now we need to bind them together! We do need these objects :D
            _rcModel.CreateRC(recipe, recipeCategory);

            foreach (var commodityShadow in list)
            {
                Commodity commodity;
                commodity = commodityShadow.ConfirmedCommodity
                    ? commodityShadow.Commodity
                    : _commodityModel.CreateCommodity(commodityShadow.Name);

                _recipeCommodityModel.CreateRecipeCommodity(recipe, commodity, commodityShadow.Value,
                    commodityShadow.ConfirmedUnit
                        ? commodityShadow.Unit.ToString()
                        : commodityShadow.UnitString);
            }
            MessageBox.Show("Opskriften er blevet gemt", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
            scrape.Close();
        }

        public class CommodityShadowConfirmed : CommodityShadow
        {
            public bool ConfirmedUnit { get; set; }
            public bool ConfirmedValue { get; set; }
            public bool ConfirmedCommodity { get; set; }
            public string UnitString { get; set; }
        }

        private double StringToDouble(string input)
        {
            Regex re = new Regex(@"^([\d]*)(([.,]([\d]{1,3}))| ?([\d]{1,2})\/([\d]{1,3}))?$");
//            string input = "1 1/2";
            Match m = re.Match(input);
            double value = (m.Groups[1].Length != 0) ? double.Parse(m.Groups[1].Value) : 0.0;

            if (m.Groups[4].Length != 0)
            {
                value += double.Parse("0." + m.Groups[3].Value);
            }
            else if (m.Groups[5].Length != 0)
            {
                value += double.Parse(m.Groups[5].Value) / double.Parse(m.Groups[6].Value);
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