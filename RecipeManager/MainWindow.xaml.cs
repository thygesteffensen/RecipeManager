using System;
using System.Windows;
using RecipeManager.Models;
using RecipeManager.Viewmodel;
using RecipeManager.Views;

namespace RecipeManager
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowVM _controlMainWindowVm;

        public MainWindow()
        {
            InitializeComponent();
            _controlMainWindowVm = new MainWindowVM();

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
            RecipeListBox.ItemsSource = _controlMainWindowVm.GetRecipes(category);
        }

        private void RecipeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecipeListBox.SelectedItem == null) return;

            Recipe recipe = (Recipe) RecipeListBox.SelectedItem;
            TextBlockTitle.Text = recipe.Name;
            TextBoxRecipe.Text = recipe.Description;
            CommodityListBox.ItemsSource = _controlMainWindowVm.GetCommodities(recipe);
        }

        private void DeleteRecipe(object sender, RoutedEventArgs e)
        {
            Recipe recipe = (Recipe) RecipeListBox.SelectedItem;
            if (MessageBox.Show($"Slet {recipe.Name}", "Er du sikker?", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                if (_controlMainWindowVm.DeleteRecipe(recipe))
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
            if (_controlMainWindowVm.EditRecipe(recipe))
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
            _controlMainWindowVm.PopulateDBDummyData();
            ReloadView();
        }

        private void WipeDatabase(object sender, RoutedEventArgs e)
        {
            _controlMainWindowVm.DeleteAllContent();
            ReloadView();
        }

        private void OpenSelectCreateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipeModeDialog dialog = new CreateRecipeModeDialog();
            if (dialog.ShowDialog() != true) return;

            if (dialog.Selection == 1)
                OpenCreateRecipe(sender, e);
            else if (dialog.Selection == 2) OpenScrapeLink(sender, e);
        }

        private void OpenCreateRecipe(object sender, RoutedEventArgs e)
        {
            _controlMainWindowVm.OpenCreateRecipeWindow();
            ReloadView();
        }

        private void OpenScrapeLink(object sender, RoutedEventArgs e)
        {
            _controlMainWindowVm.OpenScrapeLink();
            ReloadView();
        }

        private void OpenCreateRecipeCategory(object sender, RoutedEventArgs e)
        {
            _controlMainWindowVm.OpenCreateRecipeCategoryWindow();
            ReloadView();
        }

        private void ReloadView()
        {
            CategoryListBox.ItemsSource = _controlMainWindowVm.GetCategories();
            RecipeListBox.ItemsSource = null;
            CommodityListBox.ItemsSource = null;
            TextBlockTitle.Text = "";
            TextBoxRecipe.Text = "";
        }

        private void ExitProgram(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}