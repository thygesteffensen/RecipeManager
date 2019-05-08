using System.Collections.Generic;
using System.Windows;
using RecipeManager.Controllers;
using RecipeManager.Models;
using RecipeManager.Views;

namespace RecipeManager
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowController _controlMainWindowController;
        public MainWindow()
        {
            InitializeComponent();
            _controlMainWindowController = new MainWindowController();

            ReloadView();
        }

        private void CategoryListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CategoryListBox.SelectedItem != null)
                LoadRecipes((CategoryListBox.SelectedItem as RecipeCategory));
        }

        private void LoadRecipes(RecipeCategory category)
        {
            RecipeListBox.ItemsSource = _controlMainWindowController.GetRecipes(category);
        }

        private void RecipeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecipeListBox.SelectedItem == null) return;

            Recipe recipe = (Recipe) RecipeListBox.SelectedItem;
            TextBlockTitle.Text = recipe.Name;
            TextBoxRecipe.Text = recipe.Description;
            CommodityListBox.ItemsSource = _controlMainWindowController.GetCommodities(recipe);
        }


        private void LoadDummyData_Click(object sender, RoutedEventArgs e)
        {
            _controlMainWindowController.PopulateDBDummyData();
            ReloadView();
        }

        private void WipeDatabase(object sender, RoutedEventArgs e)
        {
            _controlMainWindowController.DeleteAllContent();
            ReloadView();
        }

        private void OpenCreateRecipe(object sender, RoutedEventArgs e)
        {
            _controlMainWindowController.OpenCreateRecipeWindow();
            ReloadView();
        }

        private void OpenScrapeLink(object sender, RoutedEventArgs e)
        {
            _controlMainWindowController.OpenScrapeLink();
            ReloadView();
        }

        private void OpenCreateRecipeCategory(object sender, RoutedEventArgs e)
        {
            CreateRecipeCategory createRecipeCategory = new CreateRecipeCategory(_controlMainWindowController.GetSqlConnection());
            createRecipeCategory.ShowDialog();
            ReloadView();
        }

        private void ReloadView()
        {
            CategoryListBox.ItemsSource = _controlMainWindowController.GetCategories();
            RecipeListBox.ItemsSource = null;
            TextBlockTitle.Text = "";
            TextBoxRecipe.Text = "";
        }
	}
}
