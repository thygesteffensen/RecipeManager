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

namespace HelloWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindows controlMainWindows;
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Opskrift håndtertinssystem";
            controlMainWindows = new MainWindows();
        }

        private void CategoryListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CategoryListBox.SelectedItem != null)
                LoadRecipes((CategoryListBox.SelectedItem as Category));

        }

        private void LoadRecipes(Category Category)
        {
            RecipeListBox.ItemsSource = controlMainWindows.GetRecipes(Category);
        }

        private void RecipeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecipeListBox.SelectedItem != null)
            {
                TextBlockTitle.Text = (RecipeListBox.SelectedItem as Recipe).Title;
                TextBoxRecipe.Text = (RecipeListBox.SelectedItem as Recipe).Content;
            }
        }

        private void FillOutButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryListBox.ItemsSource = controlMainWindows.GetCategories();
        }

        
    }
}
