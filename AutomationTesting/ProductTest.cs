﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace AutomationTesting
{
    /// <summary>
    /// Summary description for CodedUITest2
    /// </summary>
    [CodedUITest]
    public class ProductTest
    {
        public ProductTest()
        {
        }

        [TestMethod]
        public void searchByTypeDrink()
        {
            this.UIMap.loginAsAdmin();
            this.UIMap.startAdminWorkSpace();
            this.UIMap.goToProductInfomation();
            this.UIMap.chooseDrinkProduct();
            this.UIMap.assertDrinkProduct();
            this.UIMap.chooseToppingProduct();
            this.UIMap.assertToppingProduct();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }


        [TestMethod]
        public void searchByTyping()
        {
            this.UIMap.loginAsAdmin();
            this.UIMap.startAdminWorkSpace();
            this.UIMap.goToProductInfomation();
            this.UIMap.searchAppleTonic();
            this.UIMap.assertProductContainAppleInName();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        [Ignore]
        public void editProduct()
        {
            this.UIMap.loginAsAdmin();
            this.UIMap.startAdminWorkSpace();
            this.UIMap.goToProductInfomation();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }
        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            this.UIMap.launchApplication();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        ////Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            this.UIMap.closeApplication();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if (this.map == null)
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
