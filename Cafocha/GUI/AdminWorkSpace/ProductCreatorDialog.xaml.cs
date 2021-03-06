﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities;
using Microsoft.Win32;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for ProductCreatorDialog.xaml
    /// </summary>
    public partial class ProductCreatorDialog : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        private readonly Product _currentProduct = new Product();
        private List<Stock> _igreList;
        private readonly List<ProductModule.PDTemp> _pdtList;

        private string browseImagePath = "";

        private bool iscboRaise;

        private bool isRaiseEvent;

        public bool isRaiseIngreShowEvent = false;

        private readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        public ProductCreatorDialog(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            _pdtList = _businessModuleLocator.ProductModule.PdtList;
            InitializeComponent();

            Loaded += ProductCreatorPage_Loaded;

            _currentProduct = new Product();

            initComboBox();
        }

        private void ProductCreatorPage_Loaded(object sender, RoutedEventArgs e)
        {
            _igreList = _businessModuleLocator.WarehouseModule.IngredientList;

            _pdtList.Clear();
        }

        private void initComboBox()
        {
            iscboRaise = true;
            cboType.Items.Add(ProductType.Drink);
            cboType.Items.Add(ProductType.Topping);
            cboType.Items.Add(ProductType.Dessert);
            cboType.Items.Add(ProductType.Other);

            iscboRaise = false;
        }

        private void LvAvaibleIngredient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            var ingre = lv.SelectedItem as Stock;

            if (ingre == null) return;

            if (_pdtList.Count != 0)
            {
                var igre = _pdtList.Where(x => x.ProDe.IgdId.Equals(ingre.StoId)).FirstOrDefault();
                if (igre != null)
                {
                    MessageBox.Show("This Ingredient is already exist in Product Details List! Please choose another!");
                    return;
                }
            }

            var newPD = new ProductDetail
            {
                PdetailId = "",
                ProductId = "",
                IgdId = ingre.StoId,
                Quan = 0,
                UnitUse = ""
            };

            isRaiseEvent = true;
            //_currentProduct.ProductDetails.Add(newPD);
            _pdtList.Add(new ProductModule.PDTemp {ProDe = newPD, Ingre = ingre});
            isRaiseEvent = false;
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pdtList.Count() != 0)
                    foreach (var pd in _pdtList)
                        if (pd.ProDe.UnitUse.Equals("") || pd.ProDe.UnitUse == null || pd.ProDe.Quan < 1)
                        {
                            MessageBox.Show("Please check information of " + pd.Ingre.Name +
                                            " again! Something is not valid!");
                            return;
                        }

                //check name
                var name = txtName.Text.Trim();
                if (name.Length == 0)
                {
                    MessageBox.Show("Điền tên sản phẩm!");
                    txtName.Focus();
                    return;
                }

                //check info
                var info = txtInfo.Text.Trim();

                //check type
                var type = (int) cboType.SelectedItem;

                //check imagename
                var imgname = txtImageName.Text.Trim();
                if (imgname.Length == 0)
                {
                    MessageBox.Show("Chọn hình cho sản phẩm!");
                    btnLinkImg.Focus();
                    return;
                }

                //check discount
                int discount = 0;
                if (string.IsNullOrEmpty(txtDiscount.Text.Trim()))
                {
                    MessageBox.Show("Thêm giá cho sản phẩm");
                    txtDiscount.Focus();
                    return;
                }
                else
                {
                    discount = int.Parse(txtDiscount.Text.Trim());
                }


                //check price
                decimal price = 0;
                if (string.IsNullOrEmpty(txtPrice.Text.Trim()))
                {
                    MessageBox.Show("Thêm giá cho sản phẩm");
                    txtPrice.Focus();
                    return;
                }
                else
                {
                    price = decimal.Parse(txtPrice.Text.Trim());
                }

                _currentProduct.ProductId = "";
                _currentProduct.Name = name;
                _currentProduct.Info = info;
                _currentProduct.Type = type;
                _currentProduct.ImageLink = imgname;
                _currentProduct.Discount = discount;
                _currentProduct.Price = price;

                //C:\Program Files\ITComma\Asowel POS\Project POS\POS\POS
                var destinationFile = startupProjectPath + "\\Images\\Products\\" + txtImageName.Text.Trim();
                try
                {
                    //using (FileStream fs = new FileStream("D:\\tableImagePath.txt", FileMode.Create))
                    //{
                    //    using (StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8))
                    //    {
                    //        sWriter.WriteLine(startupProjectPath);
                    //    }
                    //}

                    if (File.Exists(destinationFile))
                    {
                        var mess = MessageBox.Show("Hình này đã có ở sản phẩm khác bạn có muốn thêm?",
                            "Cảnh báo!", MessageBoxButton.YesNo);
                        if (mess == MessageBoxResult.Yes)
                        {
                            File.Delete(destinationFile);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Hãy chọn tấm hình khác và thử lại!");
                            return;
                        }
                    }

                    //if (Path.GetExtension(destinationFile) != ".jpg")
                    //{
                    //    var mess = MessageBox.Show("Chỉ được sử dụng hình JPG, vui lòng chọn lại.",
                    //        "Cảnh báo!", MessageBoxButton.YesNo);
                    //    if (mess == MessageBoxResult.Yes)
                    //    {
                    //        File.Delete(destinationFile);
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show(
                    //            "Hãy chọn tấm hình khác và thử lại!");
                    //        return;
                    //    }
                    //}

                    File.Copy(browseImagePath, destinationFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                _businessModuleLocator.ProductModule.insertProduct(_currentProduct, _pdtList);

                MessageBox.Show("Thêm sản phẩm " + _currentProduct.Name + "(" + _currentProduct.ProductId +
                                ") Thành công!");
                ClearAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Close();
        }

        private void btnLinkImg_Click(object sender, RoutedEventArgs e)
        {
            var browseFile = new OpenFileDialog();
            browseFile.DefaultExt = ".";
            browseFile.Filter =
                "All Image Files (*.jpg, *.jpeg)|*.jpg; *.jpeg"; // " | JPEG Files (*.jpeg)|*.jpeg |  JPG Files (*.jpg)|*.jpg";
            var result = browseFile.ShowDialog();

            if (result == true)
            {
                txtImageName.Text = browseFile.SafeFileName;
                browseImagePath = browseFile.FileName;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();
        }

        private void ClearAllData()
        {
            isRaiseEvent = true;
            txtName.Text = "";
            txtInfo.Text = "";
            cboType.SelectedIndex = 0;
            txtImageName.Text = "";
            txtDiscount.Text = "";
            txtPrice.Text = "";

            _pdtList.Clear();
            isRaiseEvent = false;
        }

        private void CalSuggestPrice()
        {
            decimal sugprice = 0;
            foreach (var pd in _pdtList) sugprice += (decimal) (pd.ProDe.Quan / 1000) * pd.Ingre.StandardPrice;
        }

        private void LvDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}