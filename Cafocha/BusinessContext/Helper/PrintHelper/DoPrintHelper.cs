﻿using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using Cafocha.BusinessContext.Helper.PrintHelper.Model;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.Helper.PrintHelper
{
    public class DoPrintHelper
    {
        public static readonly int TempReceipt_Printing = 1;
        public static readonly int Receipt_Printing = 5;
        public static readonly int Eod_Printing = 3;
        public static readonly int StockIn_Printing = 6;
        public static readonly int StockOut_Printing = 7;
        private readonly RepositoryLocator _unitofwork;

        private readonly OrderNote curOrder;
        private readonly StockIn _stockIn;
        private readonly StockOut _stockOut;

        private IPrintHelper ph;
        private readonly PrintDialog printDlg;
        private readonly int type;

        public DoPrintHelper(RepositoryLocator unitofwork, int printType)
        {
            _unitofwork = unitofwork;
            type = printType;
            printDlg = new PrintDialog();

        }

        public DoPrintHelper(RepositoryLocator unitofwork, int printType, OrderNote currentOrder)
        {
            _unitofwork = unitofwork;
            type = printType;
            curOrder = currentOrder;
            printDlg = new PrintDialog();
        }

        public DoPrintHelper(RepositoryLocator unitofwork, int printType, StockIn stockIn)
        {
            _unitofwork = unitofwork;
            type = printType;
            _stockIn = stockIn;
            printDlg = new PrintDialog();
        }

        public DoPrintHelper(RepositoryLocator unitofwork, int printType, StockOut stockOut)
        {
            _unitofwork = unitofwork;
            type = printType;
            _stockOut = stockOut;
            printDlg = new PrintDialog();
        }

        public void DoPrint()
        {
//            if (curOrder == null)
//            {
//                return;
//            }

            try
            {
                // Create a PrintHelper
                CreatePrintHelper();

                // Create a FlowDocument dynamically.
                var doc = ph.CreateDocument();
                doc.Name = "FlowDoc";


                PrintToReal(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        ///     CHOOSING PRINT TO PAPER OR PRINT TO WINDOWS
        /// </summary>
        /// <param name="doc"></param>
        private void PrintToReal(FlowDocument doc)
        {
            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            // convert FlowDocument to FixedDocument
            var paginator = idpSource.DocumentPaginator;
            var package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
            var packUri = new Uri("pack://temp.xps");
            PackageStore.RemovePackage(packUri);
            PackageStore.AddPackage(packUri, package);
            var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
            XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
            var fdoc = xps.GetFixedDocumentSequence().References[0].GetDocument(true);

            var previewWindow = new DocumentViewer
            {
                Document = fdoc
            };
            var printpriview = new Window();
            printpriview.Content = previewWindow;
            printpriview.Title = "Print Preview";
            printpriview.Show();
        }

        private void CreatePrintHelper()
        {
            // Create Print Helper
            if (type == Receipt_Printing)
            {
                var order = new OrderForPrint().GetAndConvertOrder(curOrder);
                order = order.GetAndConverOrderDetails(curOrder, _unitofwork);
                ph = new ReceiptPrintHelper
                {
                    Owner = new Owner
                    {
                        ImgName = "logo.png",
                        Address = "Khu phố 6, phường Linh Trung, quận Thủ Đức, thành phố Hồ Chí Minh",
                        Phone = "038.123.456",
                        PageName = "Hóa đơn"
                    },

                    Order = order
                };
            }


            if (type == TempReceipt_Printing)
            {

                var order = new OrderForPrint().GetAndConvertOrder(curOrder);
                order = order.GetAndConverOrderDetails(curOrder, _unitofwork);

                ph = new ReceiptPrintHelper
                {
                    Owner = new Owner
                    {
                        ImgName = "logo.png",
                        Address = "Khu phố 6, phường Linh Trung, quận Thủ Đức, thành phố Hồ Chí Minh",
                        Phone = "038.123.456",
                        PageName = "Hóa đơn"
                    },

                    Order = order
                };
            }

            if (type == StockIn_Printing)
            {

                var stockIn = new StockInForPrint().getAndConvertStockInForPrint(_stockIn)
                    .getAndConvertStockInDetailsForPrint(_stockIn, _unitofwork);

                ph = new StockInPrinter()
                {
                    Owner = new Owner
                    {
                        ImgName = "logo.png",
                        Address = "Khu phố 6, phường Linh Trung, quận Thủ Đức, thành phố Hồ Chí Minh",
                        Phone = "038.123.456",
                        PageName = "Phiếu Nhập Kho"
                    },

                    StockIn = stockIn
                };
            }

            if (type == StockOut_Printing)
            {

                var stockOut = new StockOutForPrint().getAndConvertStockInForPrint(_stockOut)
                    .getAndConvertStockInDetailsForPrint(_stockOut, _unitofwork);

                ph = new StockOutPrinter()
                {
                    Owner = new Owner
                    {
                        ImgName = "logo.png",
                        Address = "Khu phố 6, phường Linh Trung, quận Thủ Đức, thành phố Hồ Chí Minh",
                        Phone = "038.123.456",
                        PageName = "Phiếu Xuất Kho"
                    },

                    StockOut = stockOut
                };
            }
            if (type == Eod_Printing)
            {
                ph = new EndOfDayPrintHelper(_unitofwork);
            }
        }
    }
}