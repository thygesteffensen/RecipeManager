using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CreateRecipe.xaml
    /// </summary>
    public partial class CreateRecipe : Window
    {
        public List<Comodity> Comodities = new List<Comodity>();
        public CreateRecipe()
        {
            InitializeComponent();
        }

        private void AddComodity_Click(object sender, RoutedEventArgs e)
        {
            //TODO:  Find the real ID of a commodity!!!
            Comodities.Add(new Comodity(){ComodityName = TextBoxName.Text, Id = Comodities.Count, Value = Convert.ToDouble(TextBoxValue.Text), Unit = ComboBoxUnit.Text});
            TextBoxName.Clear();
            TextBoxValue.Clear();

            ListBoxComodties.ItemsSource = Comodities;
            ListBoxComodties.Items.Refresh();
        }

        private void RemoveComodity_Click(object sender, RoutedEventArgs e)
        {
            // Removing from lsit
            Comodities.Remove(GetComodityFromComodity(sender));
            
            // Refresing list
            ListBoxComodties.Items.Refresh();
        }

        private void EditComodity_Click(object sender, RoutedEventArgs e)
        {
            Comodity comodity = GetComodityFromComodity(sender);

            TextBoxValue.Text = comodity.Value + "";
            TextBoxName.Text = comodity.ComodityName;

            // Removing from lsit
            Comodities.Remove(GetComodityFromComodity(sender));

            // Refresing list
            ListBoxComodties.Items.Refresh();
        }

        private Comodity GetComodityFromComodity(object sender)
        {
            // Getting ID from button
            var button = sender as Button;
            var tag = button.Tag;
            int id = int.Parse(tag.ToString());

            // Getting Comodity list index
            Comodity comodity = Comodities.Find(x => x.Id == id);

            return comodity;
        }

        /// <summary>
        /// Used to validate number box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            //Regex regex = new Regex("[0-9]*.?[0-9]*");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }

    public class Comodity
    {
        public string ComodityName { get; set; }
        public int Id { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return $"{Value} {Unit} {ComodityName} ({Id})";
        }
    }
}
