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
using RecipeManager.Controllers;

namespace RecipeManager.Views
{
    /// <summary>
    /// Interaction logic for Scrape.xaml
    /// </summary>
    public partial class Scrape : Window
    {
        private ScrapeController scrapeController;
        public Scrape(SqlConnection sqlConnection)
        {
            InitializeComponent();
            scrapeController = new ScrapeController(sqlConnection);
        }

        public void Get(object sender, RoutedEventArgs e)
        {
            scrapeController.ScrapeWebsite(TextBox.Text);
        }
    }
}
