using System.Windows;
using RecipeManager.Controllers;
using RecipeManager.Models;

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

        private void CategoryListBox_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
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

        private void DeleteRecipe(object sender, RoutedEventArgs e)
        {
            Recipe recipe = (Recipe) RecipeListBox.SelectedItem;
            if (MessageBox.Show($"Slet {recipe.Name}", "Er du sikker?", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                if (_controlMainWindowController.DeleteRecipe(recipe))
                {
                    MessageBox.Show($"{recipe.Name} er blevet fjernet!", "Succes", MessageBoxButton.OK,
                        MessageBoxImage.None);
                    ReloadView();
                }
                else
                {
                    MessageBox.Show($"Kunne ikke slette {recipe.Name}", "Fejl", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void EditRecipe(object sender, RoutedEventArgs e)
        {
            Recipe recipe = (Recipe) RecipeListBox.SelectedItem;
            if (_controlMainWindowController.EditRecipe(recipe))
            {
                MessageBox.Show($"{recipe.Name} er blevet ændret!", "Succes", MessageBoxButton.OK,
                    MessageBoxImage.None);
                ReloadView();
            }
            else
            {
                MessageBox.Show($"Kunne ikke ændre {recipe.Name}", "Fejl", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            _controlMainWindowController.OpenCreateRecipeCategoryWindow();
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