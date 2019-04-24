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
        private MainWindowController controlMainWindows;
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Opskrift håndtertinssystem";
            controlMainWindows = new MainWindowController();

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
                TextBoxRecipe.Text = (RecipeListBox.SelectedItem as Recipe).Description;
            }
        }

        private void FillOutButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryListBox.ItemsSource = controlMainWindows.GetCategories();
        }

        /// <summary>
        /// This method populates the database with dummy data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private void LoadDummyData_Click(object sender, RoutedEventArgs e)
        {
            controlMainWindows.PopulateDBDummyData();
        }

        private void OpenCreateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipe createRecipe = new CreateRecipe();
            createRecipe.ShowDialog();
        }

		//private void TestDB(object sender, RoutedEventArgs e)
		//{
		//	SqlCommand command = new SqlCommand("SELECT * FROM Recipe", conn);

		//	SqlDataReader reader = command.ExecuteReader();
  //          reader.Close();
		//	Console.Write("Database result will be printed here:");
		//	//while (reader.Read())
		//	//{
		//		//string text = reader.GetString(0) + "";
		//		//Console.Write(text);
		//	//}
			
		//	SqlCommand c = new SqlCommand("INSERT INTO Recipe (Id, Name) VALUES(@ID, @NAME)", conn);
  //          c.CommandTimeout = 15;
  //          //c.CommandType = CommandType.Text;
		//	c.Parameters.AddWithValue("@ID", 4);
		//	c.Parameters.AddWithValue("@NAME", "Jack The Ripper");

		//	c.ExecuteNonQuery();
            
			
		//	conn.Dispose();
		//}
	}
}
