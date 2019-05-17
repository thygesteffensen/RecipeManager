using System.Windows;

namespace RecipeManager.Views
{
    /// <summary>
    ///     Interaction logic for CreateRecipeModeDialog.xaml
    /// </summary>
    public partial class CreateRecipeModeDialog : Window
    {
        public CreateRecipeModeDialog()
        {
            InitializeComponent();
        }

        public int Selection { get; private set; }

        private void btnDialogLink_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Selection = 1;
        }

        private void btnDialogManuel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Selection = 2;
        }
    }
}