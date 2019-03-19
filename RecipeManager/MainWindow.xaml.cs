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
            controlMainWindows = new MainWindows();
        }

        private void FillOutButton_Click(object sender, RoutedEventArgs e)
        {
            string[] strArr = controlMainWindows.getCategories();
            foreach (string varStr in strArr)
            {
                TextBlock printTextBlock = new TextBlock();
                printTextBlock.Text = varStr;
                StackView.Children.Add(printTextBlock);
            }
        }

        private void categoryClickHandler(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
