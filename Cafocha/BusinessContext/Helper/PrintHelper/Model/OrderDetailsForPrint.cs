﻿namespace Cafocha.BusinessContext.Helper.PrintHelper.Model
{
    public class OrderDetailsForPrint
    {
        // Main data (data for Receipt printing)
        public int Quan { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }

        public decimal Amt => ProductPrice * Quan;


        // Extend data (data for other printing)
        public string ProductId { get; set; }
        public int ProductType { get; set; }
        public string Note { get; set; }
    }
}