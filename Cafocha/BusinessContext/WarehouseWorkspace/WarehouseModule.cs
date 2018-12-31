﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.GUI.CafowareWorkSpace.Helper;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.WarehouseWorkspace
{
    public class WarehouseModule
    {
        private List<Stock> _stockList;
        RepositoryLocator _unitofworkWH;


        public WarehouseModule()
        {
            _unitofworkWH = new RepositoryLocator();
        }

        public WarehouseModule(RepositoryLocator unitofworkWh)
        {
            _unitofworkWH = unitofworkWh;
        }

        public List<Stock>StockList
        {
            get => _stockList;
            set => _stockList = value;
        }

        public void loadStock()
        {
            _unitofworkWH.StockRepository
                .Get(c => c.Deleted.Equals(0), includeProperties: "APWareHouse").ToList();
        }

        public void updateStock()
        {
            foreach (var stock in _unitofworkWH.StockRepository.Get(includeProperties: "APWareHouse"))
            {
                if (stock.Deleted == 1)
                {
                    var deletedIngd = StockList.FirstOrDefault(x => x.StoId.Equals(stock.StoId));
                    if (deletedIngd != null)
                    {
                        StockList.Remove(deletedIngd);
                    }
                    continue;
                }

                var curStock = StockList.FirstOrDefault(x => x.StoId.Equals(stock.StoId));
                if (curStock == null)
                {
                    StockList.Add(stock);
                }
                else
                {
                    curStock.Name = stock.Name;
                    curStock.Info = stock.Info;
                    curStock.UnitIn = stock.UnitIn;
                    curStock.UnitOut = stock.UnitOut;
                    curStock.StandardPrice = stock.StandardPrice;

                    curStock.ApWareHouse.Contain = stock.ApWareHouse.Contain;
                    curStock.ApWareHouse.StdContain = stock.ApWareHouse.StdContain;
                }
            }
        }

        public void deleteStock(Stock stock)
        {
            stock.Deleted = 1;
            _unitofworkWH.StockRepository.Update(stock);
            _unitofworkWH.Save();
        }

        public void insertWarehouse(ApWareHouse apWareHouse)
        {
            _unitofworkWH.ApWareHouseRepository.Insert(apWareHouse);
            _unitofworkWH.Save();
        }


        public void insertStock(Stock stock)
        {
            _unitofworkWH.StockRepository.Insert(stock);
            _unitofworkWH.Save();
        }

        public void updateStock(Stock stock)
        {
            _unitofworkWH.StockRepository.Update(stock);
            _unitofworkWH.Save();
        }

        private void UpdateAPWareHouseContain(StockIn stockIn)
        {
            foreach (var details in stockIn.StockInDetails)
            {
                var stock = _stockList.FirstOrDefault(x => x.StoId.Equals(details.StoId));
                if (stock != null)
                {
                    ApWareHouse wareHouse = _unitofworkWH.ApWareHouseRepository.GetById(stock.ApwarehouseId);
                    if (wareHouse != null)
                    {
                        wareHouse.Contain += details.Quan * UnitInTrans.ToUnitContain(stock.UnitOut);
                        _unitofworkWH.ApWareHouseRepository.Update(wareHouse);
                    }
                }
            }
        }

        private void UpdateAPWareHouseContain(StockOut stockOut)
        {
            foreach (var details in stockOut.StockOutDetails)
            {
                var stock = _stockList.FirstOrDefault(x => x.StoId.Equals(details.StockId));
                if (stock != null)
                {
                    ApWareHouse wareHouse = _unitofworkWH.ApWareHouseRepository.GetById(stock.ApwarehouseId);
                    if (wareHouse != null)
                    {
                        wareHouse.Contain -= details.Quan * UnitOutTrans.ToUnitContain(stock.UnitOut);
                        _unitofworkWH.ApWareHouseRepository.Update(wareHouse);
                    }
                }
            }
        }

        public void addStockIn(StockIn stockIn)
        {
            stockIn.Intime = DateTime.Now;
            stockIn.SiId = _unitofworkWH.StockInRepository.AutoGeneteId_DBAsowell(stockIn).SiId;
            foreach (var stockInDetail in stockIn.StockInDetails)
            {
                stockInDetail.SiId = stockIn.SiId;
            }
            _unitofworkWH.StockInRepository.Insert(stockIn);

            //ToDo: Update the contain value in Warehouse database
            UpdateAPWareHouseContain(stockIn);

            _unitofworkWH.Save();
        }

        public void addStockOut(StockOut stockOut)
        {
            stockOut.OutTime = DateTime.Now;
            stockOut.StockoutId = _unitofworkWH.StockOutRepository.AutoGeneteId_DBAsowell(stockOut).StockoutId;
            foreach (var stockInDetail in stockOut.StockOutDetails)
            {
                stockInDetail.StockoutId = stockOut.StockoutId;
            }
            _unitofworkWH.StockOutRepository.Insert(stockOut);

            //ToDo: Update the contain value in Warehouse database
            UpdateAPWareHouseContain(stockOut);

            _unitofworkWH.Save();
        }
        public ApWareHouse getApWareHouse(string id)
        {
           return _unitofworkWH.ApWareHouseRepository.GetById(id);
        }
    }
}
