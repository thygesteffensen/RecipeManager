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
using RecipeManager.Models;
using RecipeManager.Viewmodel;

namespace RecipeManager.Views
{
    public partial class CreateRecipeCategory : Window
    {
        private readonly RecipeCategoryVM _recipeCategoryVm;
        public CreateRecipeCategory(string dbPath)
        {
            InitializeComponent();

            _recipeCategoryVm = new RecipeCategoryVM(dbPath);
            ListBoxRecipeCategories.ItemsSource = _recipeCategoryVm.GetRecipeCategories();
        }

        public void SaveRecipeCategory(object sender, RoutedEventArgs e)
        {
            _recipeCategoryVm.CreateRecipeCategory(RecipeCategoryTextBox.Text);
            this.Close();
        }
    }
}
