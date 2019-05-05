using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Scrape.xaml
    /// </summary>
    public partial class Scrape : Window
    {
        private ScrapeController scrapeController;
        public Scrape(SqlConnection sqlConnection, ScrapeController scrapeController)
        {
            InitializeComponent();
            this.scrapeController = scrapeController;

            CommodityName.ItemsSource = scrapeController.GetCommodities();
        }

        public void SetContentCategoryDropdown(List<RecipeCategory> list)
        {
            RecipeCategoryDropdown.ItemsSource = list;
            RecipeCategoryDropdown.SelectedIndex = 0;
        }

        private bool confirmeState = false;
        private int listIndex = 0;
        private List<ScrapeController.CommodityShadowConfirmed> _shadowList;
        public void ConfirmIngredient(List<ScrapeController.CommodityShadowConfirmed> shadowList)
        {
            this._shadowList = shadowList;
            PopulateConfirmFields();
        }

        public void PopulateConfirmFields()
        {
            var temp = _shadowList[listIndex];
            if (temp.ConfirmedCommodity)
            {
                NameGuess.Text = temp.Commodity.Name;
                CommodityName.SelectedItem = temp.Commodity;
            }
            else
            {
                NameGuess.Text = temp.Name;
                CommodityName.Text = temp.Name;
            }

            if (temp.ConfirmedUnit)
            {
                UnitGuess.Text = temp.Unit.ToString();
                ComboBoxUnit.SelectedItem = temp.Unit;
            }
            else
            {
                UnitGuess.Text = temp.UnitString;
                ComboBoxUnit.SelectedItem = temp.UnitString;
            }

            AmountGuess.Text = temp.Value + "";
            ValueConfirmed.Text = temp.Value + "";
        }

        public void ConfirmRecipe(object sender, RoutedEventArgs e)
        {
            var temp = _shadowList[listIndex];
            temp.Commodity = (Commodity) CommodityName.SelectedItem;
            temp.Unit = (Units) ComboBoxUnit.SelectedItem;
            temp.Value = Convert.ToDouble(ValueConfirmed.Text);
            listIndex++;
            if (listIndex == _shadowList.Count)
            {
                scrapeController.StoreRecipe(_shadowList,(RecipeCategory) RecipeCategoryDropdown.SelectedItem);
                return;
            }
            PopulateConfirmFields();
        }


        private void ErrorDialog(string message)
        {
            MessageBox.Show(message, "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Get(object sender, RoutedEventArgs e)
        {
            // Getting the view ready
            LockFields(false);
            NotificationTextBlock.Visibility = Visibility.Visible;
            NotificationTextBlock.Text = "Henter opskrift, vent venligst";
            // Calling the controller
            scrapeController.ScrapeWebsite(TextBox.Text);
        }


        private void HideVerificationStep(bool boolean)
        {
            if (boolean)
            {
                NotificationTextBlock.Visibility = Visibility.Hidden;
            }
            else
            {
                NotificationTextBlock.Visibility = Visibility.Visible;
            }
        }

        public void LockFields(bool boolean)
        {
            RecipeCategoryDropdown.IsEnabled = boolean;
            GetRecipeButton.IsEnabled = boolean;
            TextBox.IsEnabled = boolean;
        }
    }
}
