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
    /// Interaction logic for CreateRecipeCategory.xaml
    /// </summary>
    public partial class CreateRecipeCategory : Window
    {
        private RecipeCategoryController recipeCategoryController;
        public CreateRecipeCategory(SqlConnection sqlConnection)
        {
            InitializeComponent();
            recipeCategoryController = new RecipeCategoryController(sqlConnection);
            ListBoxRecipeCategories.ItemsSource = recipeCategoryController.GetRecipeCategories();
        }

        public void SaveRecipeCategory(object sender, RoutedEventArgs e)
        {
            recipeCategoryController.CreateRecipeCategory(RecipeCategoryTextBox.Text);
            this.Close();
        }
    }
}
