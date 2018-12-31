﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cafocha.Entities;
using Cafocha.GUI;
using Cafocha.GUI.EmployeeWorkSpace;
using Cafocha.GUI.WareHouseWorkSpace;
using log4net;

namespace Cafocha.BusinessContext
{
    public class LoginModule
    {
        private LoginWindow _loginWindow;
        private static readonly ILog AppLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoginModule(LoginWindow loginWindow)
        {
            _loginWindow = loginWindow;
        }

        public async Task LoginAsync(string username, string pass)
        {
            try
            {
                await Task.Run(() =>
                {
                    List<Employee> empList = _loginWindow._unitofwork.EmployeeRepository.Get().ToList();

                    var emp = empList.FirstOrDefault(x => x.Username.Equals(username) && x.DecryptedPass.Equals(pass));
                    if (emp != null)
                    {
                        Application.Current.Properties["EmpLogin"] = emp;
                        if (emp.EmpRole == (int)EmployeeRole.Stock)
                        {
                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                WareHouseWindow wareHouse = new WareHouseWindow();
                                wareHouse.Show();
                            });
                        }
                        else
                        {
                            try
                            {
                                SalaryNote empSalaryNote = _loginWindow._unitofwork.SalaryNoteRepository.Get(sle =>
                                    sle.EmpId.Equals(emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                                    sle.ForYear.Equals(DateTime.Now.Year)).First();

                                App.Current.Properties["EmpSN"] = empSalaryNote;
                                WorkingHistory empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalaryNote.SnId,
                                    EmpId = empSalaryNote.EmpId
                                };
                                App.Current.Properties["EmpWH"] = empWorkHistory;
                            }
                            catch (Exception ex)
                            {
                                SalaryNote empSalary = new SalaryNote
                                {
                                    EmpId = emp.EmpId,
                                    SalaryValue = 0,
                                    WorkHour = 0,
                                    ForMonth = DateTime.Now.Month,
                                    ForYear = DateTime.Now.Year,
                                    IsPaid = 0
                                };
                                _loginWindow._unitofwork.SalaryNoteRepository.Insert(empSalary);
                                _loginWindow._unitofwork.Save();
                                WorkingHistory empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalary.SnId,
                                    EmpId = empSalary.EmpId
                                };
                                App.Current.Properties["EmpWH"] = empWorkHistory;
                                App.Current.Properties["EmpSN"] = empSalary;
                            }

                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                EmpLoginListData.emploglist.Clear();
                                EmpLoginListData.emploglist.Add(new EmpLoginList
                                {
                                    Emp = emp,
                                    EmpSal = Application.Current.Properties["EmpSN"] as SalaryNote,
                                    EmpWH = Application.Current.Properties["EmpWH"] as WorkingHistory,
                                    TimePercent = 0
                                });

                                Cafocha.GUI.EmployeeWorkSpace.MainWindow main = new Cafocha.GUI.EmployeeWorkSpace.MainWindow();
                                main.Show();
                            });
                        }
                    }
                    else    
                    {
                        //Get Admin
                        List<AdminRe> adList = _loginWindow._unitofwork.AdminreRepository.Get().ToList();

                        var ad = adList.FirstOrDefault(x => x.Username.Equals(username) && x.DecryptedPass.Equals(pass));
                        //TODO: fix ad Emp
                        var adEmp = empList.FirstOrDefault(x => x.EmpId.Equals("EMP0000002"));
                        if (ad != null)
                        {

                            Application.Current.Properties["EmpLogin"] = adEmp;
                            Application.Current.Properties["AdLogin"] = ad;

                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                AdminNavWindow navwindow = new AdminNavWindow();
                                navwindow.Show();
                            });
                        }

                        if (ad == null && emp == null)
                        {
                            MessageBox.Show("incorrect username or password");
                            return;
                        }
                    }


                    _loginWindow.Dispatcher.Invoke(() =>
                    {
                        _loginWindow.Close();
                    });

                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }

        public async Task LoginByCodeAsync(string code)
        {
            try
            {
                await Task.Run(() =>
                {
                    List<Employee> empList = _loginWindow._unitofwork.EmployeeRepository.Get().ToList();
                    Employee loginEmp = empList.FirstOrDefault(x => x.DecryptedCode.Equals(code));
                    if (loginEmp != null)
                    {
                        Application.Current.Properties["EmpLogin"] = loginEmp;

                        if (loginEmp.EmpRole == (int)EmployeeRole.Stock)
                        {
                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                WareHouseWindow wareHouse = new WareHouseWindow();
                                wareHouse.Show();
                            });
                        }
                        else
                        {
                            try
                            {
                                SalaryNote empSalaryNote = _loginWindow._unitofwork.SalaryNoteRepository.Get(sle =>
                                    sle.EmpId.Equals(loginEmp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                                    sle.ForYear.Equals(DateTime.Now.Year)).First();

                                App.Current.Properties["EmpSN"] = empSalaryNote;
                                WorkingHistory empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalaryNote.SnId,
                                    EmpId = empSalaryNote.EmpId
                                };
                                App.Current.Properties["EmpWH"] = empWorkHistory;
                            }
                            catch (Exception ex)
                            {
                                SalaryNote empSalary = new SalaryNote
                                {
                                    EmpId = loginEmp.EmpId,
                                    SalaryValue = 0,
                                    WorkHour = 0,
                                    ForMonth = DateTime.Now.Month,
                                    ForYear = DateTime.Now.Year,
                                    IsPaid = 0
                                };
                                _loginWindow._unitofwork.SalaryNoteRepository.Insert(empSalary);
                                _loginWindow._unitofwork.Save();
                                WorkingHistory empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalary.SnId,
                                    EmpId = empSalary.EmpId
                                };
                                App.Current.Properties["EmpWH"] = empWorkHistory;
                                App.Current.Properties["EmpSN"] = empSalary;
                            }

                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                EmpLoginListData.emploglist.Clear();
                                EmpLoginListData.emploglist.Add(new EmpLoginList
                                {
                                    Emp = loginEmp,
                                    EmpSal = Application.Current.Properties["EmpSN"] as SalaryNote,
                                    EmpWH = Application.Current.Properties["EmpWH"] as WorkingHistory,
                                    TimePercent = 0
                                });

                                Cafocha.GUI.EmployeeWorkSpace.MainWindow main = new Cafocha.GUI.EmployeeWorkSpace.MainWindow();
                                main.Show();
                            });
                        }
                    }
                    else
                    {
                        MessageBox.Show("incorrect username or password");
                        return;
                    }

                    _loginWindow.Dispatcher.Invoke(() =>
                    {
                        _loginWindow.Close();
                    });

                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }
    }
}