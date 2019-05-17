using System.Windows;
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
            Close();
        }
    }
}