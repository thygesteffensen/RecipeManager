﻿using System;
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
        private readonly List<CommodityShadow> _commodities = new List<CommodityShadow>();
        private readonly RecipeController _recipeController;

        public int Id { get; private set; }

        public CreateRecipe(RecipeController recipeController)
        {
            this._recipeController = recipeController;
            InitializeComponent();


            RecipeCategoryDropdown.ItemsSource = _recipeController.GetRecipeCategories();
            RecipeCategoryDropdown.SelectedIndex = 0;
            ComboBoxUnit.SelectedIndex = 0;


            List<Commodity> _commodities = _recipeController.GetCommodities();

            List<string> commodityNames = _commodities.Select(commodity => (string)commodity.Name).ToList();
            CommodityName.ItemsSource = commodityNames;
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
                _commodities.Add(new CommodityShadow
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
                _commodities.Add(new CommodityShadow
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

            ListBoxCommodities.ItemsSource = _commodities;
            ListBoxCommodities.Items.Refresh();
        }

        private void RemoveCommodity_Click(object sender, RoutedEventArgs e)
        {
            // Removing from lsit
            _commodities.Remove(GetCommodityFromCommodity(sender));

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
            _commodities.Remove(GetCommodityFromCommodity(sender));

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

            if (_commodities.Count < 1)
            {
                ErrorDialog("Du har ikke angivet nogen råvare til opskriften.");
                return;
            }
            _recipeController.CreateRecipe(_commodities, recipeName, recipeDescription, recipeCategory);
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
            CommodityShadow commodityShadow= _commodities.Find(x => x.Id == id);

            return commodityShadow;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
