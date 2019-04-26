using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RecipeManager;
using RecipeManager.Controllers;
using RecipeManager.Views;
using RecipeManager.Models;
using System.Data.SqlClient;
using System.Data;

namespace HelloWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowController controlMainWindowController;
        public MainWindow()
        {
            InitializeComponent();
            controlMainWindowController = new MainWindowController();

            ReloadView();
        }

        private void CategoryListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CategoryListBox.SelectedItem != null)
                LoadRecipes((CategoryListBox.SelectedItem as RecipeCategory));
        }

        private void LoadRecipes(RecipeCategory category)
        {
            RecipeListBox.ItemsSource = controlMainWindowController.GetRecipes(category);
        }

        private void RecipeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecipeListBox.SelectedItem != null)
            {
                TextBlockTitle.Text = (RecipeListBox.SelectedItem as Recipe).Name;
                TextBoxRecipe.Text = (RecipeListBox.SelectedItem as Recipe).Description;
            }
        }
       

        private void LoadDummyData_Click(object sender, RoutedEventArgs e)
        {
            controlMainWindowController.PopulateDBDummyData();
            ReloadView();
        }

        private void OpenCreateRecipe(object sender, RoutedEventArgs e)
        {
            /*
             * HENRIK Vil man gøre dette eller vil man oprette controleren først
             */
            CreateRecipe createRecipe = new CreateRecipe();
            createRecipe.ShowDialog();
        }

        private void OpenCreateRecipeCategoru(object sender, RoutedEventArgs e)
        {
            CreateRecipeCategory createRecipeCategory = new CreateRecipeCategory(controlMainWindowController.GetSqlConnection());
            createRecipeCategory.ShowDialog();
            ReloadView();
        }

        private void ReloadView()
        {
            List<RecipeCategory> list = controlMainWindowController.GetCategories();
            CategoryListBox.ItemsSource = list;
            RecipeListBox.ItemsSource = null;
            TextBlockTitle.Text = "";
            TextBoxRecipe.Text = "";
        }
	}
}
