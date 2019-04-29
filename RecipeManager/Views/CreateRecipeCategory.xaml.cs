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
    public partial class CreateRecipeCategory : Window
    {
        private readonly RecipeCategoryController _recipeCategoryController;
        public CreateRecipeCategory(SqlConnection sqlConnection)
        {
            InitializeComponent();
            _recipeCategoryController = new RecipeCategoryController(sqlConnection);
            ListBoxRecipeCategories.ItemsSource = _recipeCategoryController.GetRecipeCategories();
        }

        public void SaveRecipeCategory(object sender, RoutedEventArgs e)
        {
            _recipeCategoryController.CreateRecipeCategory(RecipeCategoryTextBox.Text);
            this.Close();
        }
    }
}
