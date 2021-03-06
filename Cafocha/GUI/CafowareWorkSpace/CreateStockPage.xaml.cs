﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for CreateStockPage.xaml
    /// </summary>
    public partial class CreateStockPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        private Stock _currentNewStock = new Stock();

        private Stock _selectedStock;
        private readonly List<Stock> _stockList;

        public CreateStockPage(BusinessModuleLocator businessModuleLocator, List<Stock> stockList)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = stockList;
            InitializeComponent();
            lvStock.ItemsSource = _stockList;
            initComboBox();
        }

        public CreateStockPage(BusinessModuleLocator businessModuleLocator, Stock editStock)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = _businessModuleLocator.WarehouseModule.StockList;
            InitializeComponent();
            lvStock.ItemsSource = _stockList;
            initComboBox();
            _selectedStock = editStock;
        }

        public CreateStockPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = _businessModuleLocator.WarehouseModule.StockList;
            InitializeComponent();
            lvStock.ItemsSource = _stockList;
            initComboBox();
        }

        private void initComboBox()
        {
            var stockGroupList
                = new List<StockType>(WarehouseModule.StockTypes)
                {
                    new StockType() { StId = "ALL", Deleted = 0, Name = "All" }
                };
            cboGroup.ItemsSource = stockGroupList;
        }


        /*********************************
        * Controls
        *********************************/

        private void cboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = SearchIBox.Text.Trim();
            var item = (((sender as ComboBox).SelectedItem) as StockType).StId;
            var all = (((sender as ComboBox).SelectedItem) as StockType).StId == "ALL" ;
            
            if (filter.Length == 0)
            {
                if (all)
                    lvStock.ItemsSource = _stockList;
                else
                    lvStock.ItemsSource =
                        _stockList.Where(x => x.StId.Equals(item));
            }
            else
            {
                if (all)
                    lvStock.ItemsSource = _stockList.Where(x => x.Name.Contains(filter));
                else
                    lvStock.ItemsSource = _stockList.Where(x =>
                        x.StId.Equals(item) && x.Name.Contains(filter));
            }
        }

        private void bntDel_Click(object sender, RoutedEventArgs e)
        {
            if (lvStock.SelectedItem == null)
            {
                MessageBox.Show("Chưa chọn nguyên vật liệu!");
                return;
            }

            var delStock = lvStock.SelectedItem as Stock;
            if (delStock != null)
            {
                var delMess =
                    MessageBox.Show(
                        "Bạn có muốn xóa " + delStock.Name +
                        "(" + delStock.StoId + ")?", "Cảnh báo? Bạn có chắc chắn", MessageBoxButton.YesNo);
                if (delMess == MessageBoxResult.Yes)
                {
                    _businessModuleLocator.WarehouseModule.deleteStock(delStock);

                    // refesh data
                    ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
                    lvStock.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa chọn nguyên vật liệu để xóa");
            }
        }


        /*********************************
        * Manipulate Search Box
        *********************************/

        private void SearchIBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = SearchIBox.Text.Trim();
            var selectedStock = cboGroup.SelectedIndex;

            if (selectedStock < 0)
            {
                if (filter.Length == 0)
                    lvStock.ItemsSource = _stockList.Where(p => p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
            }
            else
            {
                if (filter.Length == 0)
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.StId.Equals((String) cboGroup.SelectedItem) && p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.StId.Equals((String) cboGroup.SelectedItem) && p.Name.Contains(filter) && p.Deleted.Equals(0));
            }
        }

        private void SearchIBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }


        /*********************************
        * Manipulate Form
        *********************************/

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var d = new AddOrUpdateStock(_businessModuleLocator, null);
            d.ShowDialog();
            
            clearAllData();

            // refesh data
            ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clearAllData();
        }

        private void clearAllData()
        {
            _currentNewStock = new Stock();
            lvStock.Items.Refresh();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lvStock.SelectedIndex == -1)
            {
                MessageBox.Show("Bạn chưa chọn nguyên vật liệu");
                return;
            }
            //check name
            var d = new AddOrUpdateStock(_businessModuleLocator, lvStock.SelectedItem as Stock);
            d.ShowDialog();

            clearAllData();

            _selectedStock = null;
            // refesh data
            ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
        }

        private void LvStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStock = lvStock.SelectedItem as Stock;
        }
    }
}