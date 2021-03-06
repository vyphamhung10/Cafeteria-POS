﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for EmployeeListPage.xaml
    /// </summary>
    public partial class EmployeeListPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private AdminRe admin;
        private Employee emp;
        internal EmployeeAddOrUpdateDialog empAddUptDialog;
        private List<Employee> empwithad;

        public EmployeeListPage(BusinessModuleLocator businessModuleLocator, AdminRe ad)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            admin = ad;

            Loaded += EmployeeListPage_Loaded;
        }

        private void EmployeeListPage_Loaded(object sender, RoutedEventArgs e)
        {
            //            empwithad = _unitofork.EmployeeRepository.Get(x => x.Manager.Equals(admin.AdId) && x.Deleted.Equals(0)).ToList();
            refreshData();

            txtBirth.Text = "";
            txtStart.Text = "";
        }

        private void lvData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            emp = lvDataEmployee.SelectedItem as Employee;
            if (emp == null)
            {
                txtID.Text = "";
                txtName.Text = "";
                txtBirth.Text = "";
                txtAddress.Text = "";
                txtHour_wage.Text = "";
                txtMail.Text = "";
                txtPhone.Text = "";
                txtStart.Text = "";
                txtAcount.Text = "";
                return;
            }

            txtID.Text = emp.EmpId;
            txtName.Text = emp.Name;
            txtBirth.Text = emp.Birth.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
            txtAddress.Text = emp.Addr;
            txtHour_wage.Text = emp.HourWage.ToString();
            txtMail.Text = emp.Email;
            txtPhone.Text = emp.Phone;
            txtStart.Text = emp.Startday.ToString("dd/MM/yyyy",CultureInfo.InvariantCulture);
            txtAcount.Text = emp.Username;
            switch (emp.EmpRole)
            {
                case (int) EmployeeRole.Counter:
                {
                    txtRole.Text = EmployeeRole.Counter.ToString();
                    break;
                }
                case (int) EmployeeRole.Stock:
                {
                    txtRole.Text = EmployeeRole.Stock.ToString();
                    break;
                }
            }
        }

        private void bntAddnew_Click(object sender, RoutedEventArgs e)
        {
            empAddUptDialog = new EmployeeAddOrUpdateDialog(_businessModuleLocator);
            empAddUptDialog.ShowDialog();

            refreshData();
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lvDataEmployee.SelectedItem == null)
            {
                MessageBox.Show("Employee must be selected to update! Choose again!");
                return;
            }

            empAddUptDialog = new EmployeeAddOrUpdateDialog(_businessModuleLocator, emp);
            empAddUptDialog.ShowDialog();
            lvDataEmployee.UnselectAll();
            lvDataEmployee.Items.Refresh();
        }

        private void bntDel_Click(object sender, RoutedEventArgs e)
        {
            if (lvDataEmployee.SelectedItem == null)
            {
                MessageBox.Show("Bạn chưa chọn nhân viên để xóa!");
                return;
            }

            var delEmp = lvDataEmployee.SelectedItem as Employee;
            if (delEmp != null)
            {
                var delMess = MessageBox.Show("Do you want to delete " + delEmp.Name + "(" + delEmp.Username + ")?",
                    "Warning! Are you sure?", MessageBoxButton.YesNo);
                if (delMess == MessageBoxResult.Yes)
                {
                    _businessModuleLocator.EmployeeModule.deleteEmployee(delEmp);
                    refreshData();
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa chọn nhân viên để xóa!");
            }
        }

        private void refreshData()
        {
            empwithad = _businessModuleLocator.EmployeeModule.getEmployees().ToList();
            lvDataEmployee.ItemsSource = empwithad;
            lvDataEmployee.UnselectAll();
            lvDataEmployee.Items.Refresh();
        }
    }
}