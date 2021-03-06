﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using RecipeManager.Models;
using RecipeManager.Viewmodel;

namespace RecipeManager.Views
{
    /// <summary>
    ///     Interaction logic for Scrape.xaml
    /// </summary>
    public partial class Scrape : Window
    {
        private readonly List<Commodity> _commodities;
        private readonly ScrapeVM _scrapeVm;

        private int _listIndex;
        private List<ScrapeVM.CommodityShadowConfirmed> _shadowList;

        public Scrape(ScrapeVM scrapeVm)
        {
            InitializeComponent();
            _scrapeVm = scrapeVm;

            _commodities = scrapeVm.GetCommodities();

            var commodityNames = _commodities.Select(commodity => commodity.Name).ToList();
            CommodityName.ItemsSource = commodityNames;

            /* Hidden objects which should not be seen yet */
            ShowVerificationStep(false);
            NotificationTextBlock.Visibility = Visibility.Hidden;
        }


        public void SetContentCategoryDropdown(List<RecipeCategory> list)
        {
            RecipeCategoryDropdown.ItemsSource = list;
            RecipeCategoryDropdown.SelectedIndex = 0;
        }

        public void ConfirmCommodities(List<ScrapeVM.CommodityShadowConfirmed> shadowList)
        {
            ShowVerificationStep(true);
            _shadowList = shadowList;
            PopulateConfirmFields();
        }

        public void PopulateConfirmFields()
        {
            ConfirmButton.Content = $"Bekræft ({_listIndex + 1}/{_shadowList.Count})";
            ScrapeVM.CommodityShadowConfirmed temp;
            try
            {
                temp = _shadowList[_listIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                ShowVerificationStep(false);
                RecipeCategoryDropdown.IsEnabled = true;
                GetRecipeButton.IsEnabled = true;
                URLInput.IsEnabled = true;
                NotificationTextBlock.Visibility = Visibility.Hidden;
                MessageBox.Show("Kunne ikke finde en opskrift på den givne side", "Forkert URL", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (temp.ConfirmedCommodity)
            {
                NameGuess.Text = temp.Commodity.Name;
                CommodityName.SelectedIndex = _commodities.FindIndex(a => a.Name.Contains(temp.Commodity.Name));
            }
            else
            {
                NameGuess.Text = temp.Name;
                CommodityName.Text = temp.Name;
            }

            if (temp.ConfirmedUnit)
            {
                UnitGuess.Text = temp.Unit.ToString();
                ComboBoxUnit.SelectedItem = temp.Unit;
            }
            else
            {
                UnitGuess.Text = temp.UnitString;
                ComboBoxUnit.SelectedItem = temp.UnitString;
            }

            AmountGuess.Text = temp.Value + "";
            ValueConfirmed.Text = temp.Value + "";
        }

        public void ConfirmCommodity(object sender, RoutedEventArgs e)
        {
            var temp = _shadowList[_listIndex];
            temp.Commodity = CommodityName.SelectedIndex > 0 ? _commodities[CommodityName.SelectedIndex] : null;
            temp.Unit = (Units) ComboBoxUnit.SelectedItem;
            temp.Value = Convert.ToDouble(ValueConfirmed.Text);
            _listIndex++;
            if (_listIndex == _shadowList.Count)
            {
                _scrapeVm.StoreRecipe(_shadowList, (RecipeCategory) RecipeCategoryDropdown.SelectedItem);
                return;
            }

            PopulateConfirmFields();
        }


        private async void GetRecipeInitializeProcess(object sender, RoutedEventArgs e)
        {
            var url = URLInput.Text;
            if (!url.Contains("valdemarsro"))
            {
                // This check is only due to lag of real dev
                MessageBox.Show("Forkert URL, Vi understøtter kun opskrifter fra Valdemarso.dk", "Fejl",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Getting the view ready
            RecipeCategoryDropdown.IsEnabled = false;
            GetRecipeButton.IsEnabled = false;
            URLInput.IsEnabled = false;
            NotificationTextBlock.Visibility = Visibility.Visible;
            // Calling the controller
            try
            {
                await Task.Run(() => _scrapeVm.ScrapeWebsite(url));
                return;
            }
            catch (UriFormatException)
            {
                NotificationTextBlock.Visibility = Visibility.Hidden;
                MessageBox.Show("Kunne ikke finde en opskrift på den givne side", "Forkert URL", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (WebException)
            {
                NotificationTextBlock.Visibility = Visibility.Hidden;
                MessageBox.Show("Der er ingen internetforbindelse", "Ingen forbindelse", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            RecipeCategoryDropdown.IsEnabled = true;
            GetRecipeButton.IsEnabled = true;
            URLInput.IsEnabled = true;
        }


        private void ShowVerificationStep(bool boolean)
        {
            Height = boolean ? 270 : 200;
            NotificationTextBlock.Text = boolean
                ? "Vi kunne ikke bestemme de følgende ingredienser, bekræft dem venligst"
                : "Henter opskrift, vent venligst";
            AmountGuess.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            UnitGuess.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            NameGuess.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            ValueConfirmed.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            ComboBoxUnit.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            CommodityName.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
            ConfirmButton.Visibility = boolean ? Visibility.Visible : Visibility.Hidden;
        }
    }
}