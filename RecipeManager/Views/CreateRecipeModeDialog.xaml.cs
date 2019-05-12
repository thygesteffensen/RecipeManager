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
using System.Windows.Shapes;

namespace RecipeManager.Views
{
    /// <summary>
    /// Interaction logic for CreateRecipeModeDialog.xaml
    /// </summary>
    public partial class CreateRecipeModeDialog : Window
    {
        private int _selection = 0; // 1 = link, 2 = manuel

        public CreateRecipeModeDialog()
        {
            InitializeComponent();
        }

        private void btnDialogLink_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            _selection = 1;
        }

        private void btnDialogManuel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            _selection = 2;
        }

        public int Selection => _selection;
    }
}