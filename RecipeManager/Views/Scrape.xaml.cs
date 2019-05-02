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
        }

        public void SetContentCategoryDropdown(List<RecipeCategory> list)
        {
            RecipeCategoryDropdown.ItemsSource = list;
            RecipeCategoryDropdown.SelectedIndex = 0;
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
