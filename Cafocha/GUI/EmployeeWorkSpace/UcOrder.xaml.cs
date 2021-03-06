﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.BusinessContext.Helper.PrintHelper;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for UcOrder.xaml
    /// </summary>
    public partial class UcOrder : UserControl
    {
        private static string NO_EMPLOY_WORKING_DIALOG = "Chưa có nhân viên đang làm việc. Vui lòng thử lại";
        private BusinessModuleLocator _businessModuleLocator;
        private Employee currentEmp;

        private bool InitCus_raiseEvent;


        private bool initStatus_RaiseEvent;


        /// <summary>
        ///     Migrate all Order info in current Table to OrderNote object that will be insert to Database
        /// </summary>
        /// <param name="newOrder"></param>
        private bool isClearingTable;

        private bool isClearingTalbe_ForDelete;


        private bool isUcOrderFormLoading;


        private readonly bool isUnCheckChair = false;


        public UcOrder()
        {
            InitializeComponent();

            Loaded += UcOder_Loaded;
            Unloaded += UcOder_Unloaded;
        }

        private void UcOder_Loaded(object sender, RoutedEventArgs e)
        {
            isUcOrderFormLoading = true;
            _businessModuleLocator = ((MainWindow) Window.GetWindow(this))._businessModuleLocator;


            InitCus_raiseEvent = true;
            initStatus_RaiseEvent = true;
            txtDay.Text = "";
            txtTotal.Text = "";
            wp.Children.Clear();
            lvData.ItemsSource = new List<TakingOrderModule.OrderDetails_Product_Joiner>();

            LoadCustomerOwner();
            LoadWorkingEmployee();
            txtTotal.Text = "";
            txtTotal.IsEnabled = false;
            isUcOrderFormLoading = false;
        }

        private void UcOder_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Window.GetWindow(this)).currentChair = null;
        }


        /// <summary>
        ///     show all orderdetails in the current checked chair.
        ///     allow to modify these orderdetails
        /// </summary>
        public void RefreshControl()
        {
            try
            {
                initStatus_RaiseEvent = true;
                // binding
                lvData.ItemsSource = _businessModuleLocator.TakingOrderModule.getOrderDetailsDisplay();
                ;
                loadTotalPrice();

                initStatus_RaiseEvent = false;
            }
            catch (Exception ex)
            {
                MainWindow.AppLog.Error(ex);
            }
        }

        /// <summary>
        ///     show all orderdetails of all chairs in the table.
        ///     but not allow to modify these orderdetails
        /// </summary>
        public void RefreshControlAllChair()
        {
            // binding
            lvData.ItemsSource = _businessModuleLocator.TakingOrderModule.getOrderDetailsDisplay();
            if (!isUnCheckChair) loadTotalPrice();
        }

        private void LoadCustomerOwner()
        {
            cboCustomers.ItemsSource = _businessModuleLocator.CustomerModule.getAllCustomer();
            cboCustomers.SelectedValuePath = "CusId";
            cboCustomers.DisplayMemberPath = "Name";
            cboCustomers.MouseEnter += (sender, args) => { cboCustomers.Background.Opacity = 100; };
            cboCustomers.MouseLeave += (sender, args) => { cboCustomers.Background.Opacity = 0; };

            if (_businessModuleLocator.TakingOrderModule.OrderTemp.CusId != null)
            {
                InitCus_raiseEvent = true;
                cboCustomers.SelectedValue = _businessModuleLocator.TakingOrderModule.OrderTemp.CusId;
            }

            InitCus_raiseEvent = false;
        }

        private void LoadWorkingEmployee()
        {
            var prevSelected = cboEmployee.SelectedIndex;
            var empLogin =
                _businessModuleLocator.EmployeeModule.Emploglist.Where(x => x.IsStartWorking && x.Emp.EmpRole == (int)EmployeeRole.Counter == true);
            var t = new List<Employee>();
            foreach (var emp in empLogin)
            {
                t.Add(emp.Emp);
            }
            cboEmployee.ItemsSource = t;
            cboEmployee.Items.Refresh();
            cboCustomers.MouseEnter += (sender, args) => { cboCustomers.Background.Opacity = 100; };
            cboCustomers.MouseLeave += (sender, args) => { cboCustomers.Background.Opacity = 0; };
            if (prevSelected == -1 && t.Count() != 0)
            {
                cboEmployee.SelectedItem = 0;
            }
            else
            {
                cboEmployee.SelectedItem = prevSelected;
            }
            InitCus_raiseEvent = false;
        }

        private void CboCustomers_SeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!InitCus_raiseEvent)
            {
                if (EmployeeModule.WorkingEmployee == null)
                {
                    MessageBox.Show(NO_EMPLOY_WORKING_DIALOG);
                    return;
                }


                _businessModuleLocator.TakingOrderModule.OrderTemp.CusId =
                    (string) (sender as ComboBox).SelectedValue;
                _businessModuleLocator.TakingOrderModule.OrderTemp.Discount = _businessModuleLocator.CustomerModule
                    .getCustomer(_businessModuleLocator.TakingOrderModule.OrderTemp.CusId).Discount;
                loadTotalPrice();
                checkWorkingAction();
            }
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }


        /// <summary>
        ///     inner class that use to store the joined data from
        ///     orderdetails entities and product entities
        /// </summary>
        public void loadTotalPrice()
        {
            _businessModuleLocator.TakingOrderModule.loadTotalPrice();
            txtTotal.Text = string.Format("{0:0.000}", _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice);
        }

        private void bntDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            //            ClearTheTable();

           
            int index;
            var dep = (DependencyObject)e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvData.ItemContainerGenerator.IndexFromContainer(dep);

            _businessModuleLocator.TakingOrderModule.deleteOrderDetail(index, true);

            RefreshControl();
        }

        private void bntEdit_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeModule.WorkingEmployee == null)
            {
                MessageBox.Show(NO_EMPLOY_WORKING_DIALOG);
                return;
            }

            var dep = (DependencyObject) e.OriginalSource;

            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);

            if (dep == null)
                return;

            var index = lvData.ItemContainerGenerator.IndexFromContainer(dep);


            var inputnote = new InputNote(_businessModuleLocator.TakingOrderModule.OrderTemp.OrderDetailsTemps
                .ElementAt(index).Note);

            if (inputnote.ShowDialog() == true)
                _businessModuleLocator.TakingOrderModule.updateOrderNote(index, inputnote.Note.ToLower());


            RefreshControl();
            checkWorkingAction();
        }


        private void bntPay_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeModule.WorkingEmployee == null)
            {
                MessageBox.Show(NO_EMPLOY_WORKING_DIALOG);
                return;
            }

            _businessModuleLocator.TakingOrderModule.OrderTemp.EmpId = cboEmployee.SelectedIndex != -1 ? cboEmployee.SelectedValue.ToString() : EmployeeModule.WorkingEmployee.Emp.EmpId;

            // input the rest data
            var newOrder = new OrderNote();
            newOrder.TotalPrice = _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice;
            var inputTheRest = new InputTheRestOrderInfoDialog(newOrder);
            if (!inputTheRest.MyShowDialog()) return;


            // convert data and save to database
            if (_businessModuleLocator.TakingOrderModule.convertTableToOrder(newOrder))
                _businessModuleLocator.TakingOrderModule.saveOrderToDB(newOrder);
            else
                return;

            // printing
            var printer = new DoPrintHelper(_businessModuleLocator.RepositoryLocator, DoPrintHelper.Receipt_Printing,
                newOrder);
            printer.DoPrint();

            // clean the old table data
            ClearTheTable();
        }

        private void BntPrint_OnClick(object sender, RoutedEventArgs e)
        {
            if (EmployeeModule.WorkingEmployee == null)
            {
                MessageBox.Show(NO_EMPLOY_WORKING_DIALOG);
                return;
            }

            _businessModuleLocator.TakingOrderModule.OrderTemp.EmpId = cboEmployee.SelectedIndex != -1 ? cboEmployee.SelectedValue.ToString() : EmployeeModule.WorkingEmployee.Emp.EmpId;

            // input the rest data
            var newOrder = new OrderNote();
            newOrder.TotalPrice = _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice;
            _businessModuleLocator.TakingOrderModule.convertTableToOrder(newOrder);

            // printing
            var printer = new DoPrintHelper(_businessModuleLocator.RepositoryLocator,
                DoPrintHelper.TempReceipt_Printing, newOrder);
            printer.DoPrint();


            // update employee ID that effect to the OrderNote
            checkWorkingAction();
        }

        private void BntDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (EmployeeModule.WorkingEmployee == null)
            {
                MessageBox.Show(NO_EMPLOY_WORKING_DIALOG);
                return;
            }

            ClearTheTable();
            checkWorkingAction();
        }

        /// <summary>
        ///     Clean the whole Order info in  current Table
        /// </summary>
        private void ClearTheTable()
        {
            isClearingTable = true;
            _businessModuleLocator.TakingOrderModule.clearOrder();

//        TODO: Reset is printed or not    
//            curTable.IsOrdered = 0;
//            curTable.IsPrinted = 0;

            LoadCustomerOwner();
            RefreshControlAllChair();

            //TODO: clear order or print
//            if (cur != null)
//            {
//                cur.IsOrdered = 0;
//                cur.IsPrinted = 0;
//            }

            isClearingTable = false;

            ((MainWindow) Window.GetWindow(this)).bntEntry.IsEnabled = true;
        }

        private void ClearTheTable_ForDelete()
        {
            isClearingTalbe_ForDelete = true;
            _businessModuleLocator.TakingOrderModule.clearOrder();

            LoadCustomerOwner();
            RefreshControlAllChair();

            //TODO: clear order or print
            //            if (cur != null)
            //            {
            //                cur.IsOrdered = 0;
            //                cur.IsPrinted = 0;
            //            }

            isClearingTalbe_ForDelete = false;


            ((MainWindow) Window.GetWindow(this)).bntEntry.IsEnabled = true;
        }


        /// <summary>
        ///     Give Back the Ingredient contain when delete Product from Order
        /// </summary>
        /// <param name="orderDetails">The OrderDetails that contain a give back Product</param>
        /// <param name="productQuan">give back product quantity</param>
        private void checkWorkingAction()
        {
//            if (EmployeeModule.WorkingEmployee == null ||
//                EmployeeModule.WorkingEmployee.Emp.EmpId.Equals(_businessModuleLocator.TakingOrderModule.OrderTemp.EmpId)) return;

        }


        // INPUT PAX

        // INPUT TOTALPRICE
        private void txtTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_businessModuleLocator == null || _businessModuleLocator.TakingOrderModule.OrderTemp == null ||
                isUcOrderFormLoading || isClearingTable || isClearingTalbe_ForDelete)
                return;
            var txtTotal = sender as TextBox;
            if (string.IsNullOrEmpty(txtTotal.Text))
                _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice = 0;
            else
                _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice = decimal.Parse(txtTotal.Text);

            // update employee ID that effect to the OrderNote
            checkWorkingAction();
        }

        // FORMAT TOTALPRICE TEXTBOX
        private void txtTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            // When user leave the Total Price textbox, it will format their input value
            var txtTotal = sender as TextBox;

            var Total = _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice;
//            var SaleValue = Total;
//            var Svc = Total * 5 / 100;
//            var Vat = (Total + Total * 5 / 100) * 10 / 100;
//            Total = Total + Total * 5 / 100 + (Total + Total * 5 / 100) * 10 / 100;

            _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice = Total;
//            _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPriceNonDisc = Math.Round(Total, 3);
//            Math.Round(Svc, 3);
//            _businessModuleLocator.TakingOrderModule.OrderTemp.Vat = Math.Round(Vat, 3);
//            _businessModuleLocator.TakingOrderModule.OrderTemp.SaleValue = Math.Round(SaleValue, 3);

            txtTotal.Text = string.Format("{0:0.000}", _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice);
        }

        private void CboEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _businessModuleLocator.TakingOrderModule.OrderTemp.EmpId = cboEmployee.SelectedValue.ToString();
        }


        private void CboEmployee_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LoadWorkingEmployee();
        }

        private void CboEmployee_GotMouseCapture(object sender, MouseEventArgs e)
        {
            LoadWorkingEmployee();
        }
    }
}