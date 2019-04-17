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
                TextBoxRecipe.Text = (RecipeListBox.SelectedItem as Recipe).Description;
            }
        }

        private void FillOutButton_Click(object sender, RoutedEventArgs e)
        {
            CategoryListBox.ItemsSource = controlMainWindows.GetCategories();
        }

        private void OpenCreateRecipe(object sender, RoutedEventArgs e)
        {
            CreateRecipe createRecipe = new CreateRecipe();
            createRecipe.ShowDialog();
        }

		private void TestDB(object sender, RoutedEventArgs e)
		{
			//string path = Path.Combine(Application.StartupPath, "RecipeMangerDatabase.mdf");
			string startupPath = Environment.CurrentDirectory;
			string path = startupPath + "\\RecipeMangerDatabase.mdf";
            path = "C:\\Users\\Thyge Steffensen\\Documents\\RecipeManager\\RecipeManager\\RecipeManagerDatabase.mdf";
            Console.Write(path);

			//SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + path + ";");
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" + path + "\";Integrated Security=True");
			conn.Open();

			SqlCommand command = new SqlCommand("SELECT * FROM Recipe", conn);

			SqlDataReader reader = command.ExecuteReader();
            reader.Close();
			Console.Write("Database result will be printed here:");
			//while (reader.Read())
			//{
				//string text = reader.GetString(0) + "";
				//Console.Write(text);
			//}
			
			SqlCommand c = new SqlCommand("INSERT INTO Recipe (Id, Name) VALUES(4, \"Jack\")", conn);
			//c.Parameters.AddWithValue("@i", 4);
			//c.Parameters.AddWithValue("@v", "Jack");

			c.ExecuteNonQuery();
			
			conn.Dispose();
		}
	}
}
