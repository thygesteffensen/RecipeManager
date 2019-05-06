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
        private readonly ScrapeController _scrapeController;
        private readonly List<Commodity> _commodities; 
        public Scrape(SqlConnection sqlConnection, ScrapeController scrapeController)
        {
            InitializeComponent();
            this._scrapeController = scrapeController;

            _commodities = scrapeController.GetCommodities();

            List<string> commodityNames = _commodities.Select(commodity => (string) commodity.Name).ToList();
            CommodityName.ItemsSource = commodityNames;

            /* Hidden objects which should not be seen yet */
            ShowVerificationStep(false);
            NotificationTextBlock.Visibility = Visibility.Hidden;
        }


        public void SetContentCategoryDropdown(List<RecipeCategory> list)
        {
            RecipeCategoryDropdown.ItemsSource = list;
            RecipeCategoryDropdown.SelectedIndex = 0;
        }

        private int _listIndex = 0;
        private List<ScrapeController.CommodityShadowConfirmed> _shadowList;
        public void ConfirmCommodities(List<ScrapeController.CommodityShadowConfirmed> shadowList)
        {
           
            ShowVerificationStep(true);
            this._shadowList = shadowList;
            PopulateConfirmFields();
        }

        public void PopulateConfirmFields()
        {
            ConfirmButton.Content = $"Bekræft ({_listIndex+1}/{_shadowList.Count})";
            var temp = _shadowList[_listIndex];
            if (temp.ConfirmedCommodity)
            {
                NameGuess.Text = temp.Commodity.Name;
                CommodityName.SelectedIndex = _commodities.FindIndex(a => a.Name.Contains(temp.Commodity.Name));
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

        public void ConfirmCommodity(object sender, RoutedEventArgs e)
        {
            var temp = _shadowList[_listIndex];
            temp.Commodity = CommodityName.SelectedIndex > 0 ? (Commodity)_commodities[CommodityName.SelectedIndex] : null;
            temp.Unit = (Units) ComboBoxUnit.SelectedItem;
            temp.Value = Convert.ToDouble(ValueConfirmed.Text);
            _listIndex++;
            if (_listIndex == _shadowList.Count)
            {
                _scrapeController.StoreRecipe(_shadowList,(RecipeCategory) RecipeCategoryDropdown.SelectedItem);
                return;
            }
            PopulateConfirmFields();
        }


        private async void GetRecipeInitializeProcess(object sender, RoutedEventArgs e)
        {
            string url = URLInput.Text;
            if (!url.Contains("valdemarsro"))
            {
                // This check is only due to lag of real dev
                MessageBox.Show("Forkert URL, Vi understøtter kun opskrifter fra Valdemarso.dk", "Fejl",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Getting the view ready
            RecipeCategoryDropdown.IsEnabled = false;
            GetRecipeButton.IsEnabled = false;
            URLInput.IsEnabled = false;
            NotificationTextBlock.Visibility = Visibility.Visible;
            // Calling the controller
            await Task.Run(() => _scrapeController.ScrapeWebsite(url));
        }


        private void ShowVerificationStep(bool boolean)
        {
            this.Height = boolean ? 270 : 200;
            NotificationTextBlock.Text = boolean 
                ? "Vi kunne ikke bestemme de følgende ingredienser, bekræft dem venligst" 
                : "Henter opskrift, vent venligst";
            AmountGuess.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            UnitGuess.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            NameGuess.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            ValueConfirmed.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            ComboBoxUnit.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            CommodityName.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            ConfirmButton.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
