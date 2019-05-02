using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RecipeManager.Controllers;
using RecipeManager.Models;

namespace RecipeManager.Views
{
    /// <summary>
    /// Interaction logic for CreateRecipe.xaml
    /// </summary>
    public partial class CreateRecipe : Window
    {
        private List<CommodityShadow> Commodities = new List<CommodityShadow>();
        private RecipeController recipeController;
        private RecipeCategoryController recipeCategoryController;
        private CommodiyModel commodityModel;

        public int Id { get; private set; }

        public CreateRecipe(SqlConnection sqlConnection)
        {
            InitializeComponent();

            recipeController = new RecipeController(sqlConnection);
            recipeCategoryController = new RecipeCategoryController(sqlConnection);
            commodityModel = new CommodiyModel(sqlConnection);

            RecipeCategoryDropdown.ItemsSource = recipeCategoryController.GetRecipeCategories();
            RecipeCategoryDropdown.SelectedIndex = 0;
            ComboBoxUnit.SelectedIndex = 0;

            CommodityName.ItemsSource = commodityModel.GetCommodities();
        }

        private void AddCommodity_Click(object sender, RoutedEventArgs e)
        {
            double value;
            try
            {
                value = Convert.ToDouble(TextBoxValue.Text);
            }
            catch (FormatException)
            {
                ErrorDialog("Mængden er ikke korrekt indtastet");
                return;
            }

            // We need to figure out if the Commodity added already exists or if it
            // just a text string.
            // First we will try to cast it as a Commodity object...
            Commodity commodity = null;
            try
            {
                commodity = (Commodity) CommodityName.SelectedItem;
            }
            catch (InvalidCastException exp)
            {
                // This isn't a severe exception
                Console.Write($@"Couldn't cast: {exp}");
            }

            if (commodity != null)
            {
                // Commodity could not be casted. therefore it must be a string...
                Commodities.Add(new CommodityShadow
                {
                    Id = (int)1,
                    Commodity = commodity,
                    Name = commodity.Name,
                    Value = value,
                    Unit = (Units)ComboBoxUnit.SelectionBoxItem
                });
            }
            else
            {
                string name = (string)CommodityName.Text;
                if (name.Length < 1)
                {
                    ErrorDialog("Råvare navnet er ikke gyldigt.");
                    return;
                }
                Commodities.Add(new CommodityShadow
                {
                    Id = (int)1,
                    Name = name,
                    Value = value,
                    Unit = (Units)ComboBoxUnit.SelectionBoxItem
                });
            }

            TextBoxValue.Clear();
            CommodityName.SelectedIndex = -1;
            CommodityName.Text = "";

            ListBoxCommodities.ItemsSource = Commodities;
            ListBoxCommodities.Items.Refresh();
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            // Removing from lsit
            Commodities.Remove(GetCommodityFromCommodity(sender));

            // Refresing list
            ListBoxCommodities.Items.Refresh();
        }

        private void EditCommodity_Click(object sender, RoutedEventArgs e)
        {
            CommodityShadow commodityShadow = GetCommodityFromCommodity(sender);

            TextBoxValue.Text = commodityShadow.Value + "";
            CommodityName.Text = commodityShadow.Name;
            ComboBoxUnit.SelectedValue = commodityShadow.Unit;

            // Removing from liSt
            Commodities.Remove(GetCommodityFromCommodity(sender));

            // Refreshing list
            ListBoxCommodities.Items.Refresh();
        }

        private void SaveRecipe(object sender, RoutedEventArgs e)
        {
            // Collect all the information! 
            // First we will get the Recipe Category
            RecipeCategory recipeCategory = RecipeCategoryDropdown.SelectedItem as RecipeCategory;
            if (recipeCategory is null)
            {
                ErrorDialog("Du har ikke valgt en kategori");
                return;
            }

            // Retrieve recipe name
            string recipeName = RecipeName.Text;
            if (recipeName.Length < 1)
            {
                ErrorDialog("Du har ikke angivet et navn på opskriften.");
                return;
            }

            // Retrieve recipe description
            string recipeDescription = Description.Text;
            if (recipeDescription.Length < 1)
            {
                ErrorDialog("Du har ikke angivet nogen beskrivelse til opskriften.");
                return;
            }

            if (Commodities.Count < 1)
            {
                ErrorDialog("Du har ikke angivet nogen råvare til opskriften.");
                return;
            }
            recipeController.CreateRecipe(Commodities, recipeName, recipeDescription, recipeCategory);
            this.Close();
        } 

        private void ErrorDialog(string message)
        {
            MessageBox.Show(message, "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private CommodityShadow GetCommodityFromCommodity(object sender)
        {
            // Getting ID from button
            var button = sender as Button;
            var tag = button.Tag;
            int id = int.Parse(tag.ToString());

            // Getting Commodity list index
            CommodityShadow commodityShadow= Commodities.Find(x => x.Id == id);

            return commodityShadow;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
