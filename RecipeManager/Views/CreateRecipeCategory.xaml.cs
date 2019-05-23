using System;
using System.Windows;
using RecipeManager.Models;
using RecipeManager.Viewmodel;

namespace RecipeManager.Views
{
    public partial class CreateRecipeCategory : Window
    {
        private readonly RecipeCategoryVM _recipeCategoryVm;
        private Func<RecipeCategory, int> callback;

        public CreateRecipeCategory(string dbPath, Func<RecipeCategory, int> callback)
        {
            InitializeComponent();
            this.callback = callback;


            _recipeCategoryVm = new RecipeCategoryVM(dbPath);
            ListBoxRecipeCategories.ItemsSource = _recipeCategoryVm.GetRecipeCategories();
        }

        public void SaveRecipeCategory(object sender, RoutedEventArgs e)
        {
            
            callback(_recipeCategoryVm.CreateRecipeCategory(RecipeCategoryTextBox.Text));
            Close();
        }
    }
}