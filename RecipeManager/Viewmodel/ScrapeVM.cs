using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager.Viewmodel
{
    public class ScrapeVM
    {
        private readonly CommodityModel _commodityModel;
        private readonly List<string> _ingredientsList = new List<string>();
        private readonly RCModel _rcModel;
        private readonly RecipeCommodityModel _recipeCommodityModel;
        private readonly List<string> _recipeDescription = new List<string>();
        private readonly RecipeModel _recipeModel;
        private readonly Scrape _scrape;

        private string _recipeName;

        public ScrapeVM(string dbPath)
        {
            _commodityModel = new CommodityModel(dbPath);
            _recipeModel = new RecipeModel(dbPath);
            _rcModel = new RCModel(dbPath);
            _recipeCommodityModel = new RecipeCommodityModel(dbPath);

            var recipeCategoryVm = new RecipeCategoryVM(dbPath);

            _scrape = new Scrape(this);
            _scrape.SetContentCategoryDropdown(recipeCategoryVm.GetRecipeCategories());
            _scrape.ShowDialog();
        }

        public List<Commodity> GetCommodities()
        {
            return _commodityModel.GetCommodities();
        }

        public void ScrapeWebsite(string url)
        {
            HttpWebRequest request;
            request = (HttpWebRequest) WebRequest.Create(url);

            using (var response = (HttpWebResponse) request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var regexStart = new Regex("<h2 itemprop=\"name\"(.*?)>(.*?)</h2>");
                var regexEnd = new Regex("after-recipe");

                // Retrieving the important lines
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                    if (regexStart.IsMatch(line))
                        do
                        {
                            lines.Add(line);
                            if (regexEnd.IsMatch(line)) break;
                        } while ((line = reader.ReadLine()) != null);

                RetrieveRecipe(lines);
                VerifyIngredients();
            }
        }

        private void VerifyIngredients()
        {
            var re = new Regex(@"^([\d]*)?");

            var shadowList = new List<CommodityShadowConfirmed>();

            var length = _ingredientsList.Count;
            foreach (var ingredient in _ingredientsList)
            {
                // Creating the shadow object
                var shadowObject = new CommodityShadowConfirmed();

                var ingredientStuff = ingredient.Split(',')[0]; // Remove text after comma
                var m = re.Match(ingredientStuff);
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
                var commodities = _commodityModel.GetCommodities(commodityName);
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
                if (unitDefined) shadowObject.Unit = unit;

                shadowObject.UnitString = unitString;

                shadowList.Add(shadowObject);
            }

            // Since this method is called from a async thread
            _scrape.Dispatcher.Invoke(() => { _scrape.ConfirmCommodities(shadowList); });
        }

        public void StoreRecipe(List<CommodityShadowConfirmed> list, RecipeCategory recipeCategory)
        {
            var description = "";
            foreach (var block in _recipeDescription) description += block + "\n";

            var recipe = _recipeModel.CreateRecipe(_recipeName, description);


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
            _scrape.Close();
        }


        private void RetrieveRecipe(List<string> lines)
        {
            var regexDescription = new Regex("itemprop=\"recipeInstructions\"");
            var regexRecipeIngredients = new Regex("ingredientlist");
            var regexRecipeIngredientsElements = new Regex("recipeIngredient");
            var regexRecipeDescriptionEnd = new Regex("</div>");

            var i = 0;
            Match match;
            foreach (var line in lines)
            {
                i++;
                // Retrieving recipe name
                var regexRecipeName = new Regex("<h2 itemprop=\"name\"(.*?)>(.*?)</h2>");

                match = regexRecipeName.Match(line);
                if (match.Success) _recipeName = StripHTMLTagsRegex(match.ToString());

                // Retrieving the description
                match = regexDescription.Match(line);
                if (match.Success)
                {
                    var description = line.Substring(match.Index);
                    foreach (var lineInner in lines.GetRange(i, lines.Count - 1))
                    {
                        description += lineInner;
                        if (regexRecipeDescriptionEnd.IsMatch(lineInner)) break;
                    }

                    var regexParagraf = new Regex("<p>(.*?)</p>");
                    foreach (Match itemMatch in regexParagraf.Matches(description))
                        _recipeDescription.Add(StripHTMLTagsRegex(itemMatch.ToString()));
                }

                // Retrieving the ingredients
                match = regexRecipeIngredients.Match(line);
                if (match.Success)
                {
                    var ingredients = line.Substring(match.Index);
                    foreach (var lineInner in lines.GetRange(i, lines.Count - 1))
                    {
                        ingredients += lineInner;
                        if (regexRecipeIngredientsElements.IsMatch(lineInner)) break;
                    }

                    var regexParagraf = new Regex("<li (.*?)>(.*?)</li>");
                    foreach (Match itemMatch in regexParagraf.Matches(ingredients))
                        _ingredientsList.Add(StripHTMLTagsRegex(itemMatch.ToString()));
                }
            }
        }

        /// <summary>
        ///     Strips a string from HTML tags
        /// </summary>
        /// <param name="source">HTML string</param>
        /// <returns>Striped string</returns>
        private string StripHTMLTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        ///     Converts a given string to a double
        /// </summary>
        /// <param name="input">String</param>
        /// <returns>Double</returns>
        private double StringToDouble(string input)
        {
            var re = new Regex(@"^([\d]*)(([.,]([\d]{1,3}))| ?([\d]{1,2})\/([\d]{1,3}))?$");
            var m = re.Match(input);
            var value = m.Groups[1].Length != 0 ? double.Parse(m.Groups[1].Value) : 0.0;

            if (m.Groups[4].Length != 0)
                value += double.Parse("0." + m.Groups[3].Value);
            else if (m.Groups[5].Length != 0)
                value += double.Parse(m.Groups[5].Value) / double.Parse(m.Groups[6].Value);

            return value;
        }

        public class CommodityShadowConfirmed : CommodityShadow
        {
            public bool ConfirmedUnit { get; set; }
            public bool ConfirmedCommodity { get; set; }
            public string UnitString { get; set; }
        }
    }
}