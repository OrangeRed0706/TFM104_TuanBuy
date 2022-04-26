using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuanBuy.Models.Entities;

namespace TuanBuy.ViewModel
{
    //訂單管理
    public class OrderBackMangeViewModel
    {
        //訂單編號
        public string OrderId { get; set; }
        //User姓名
        public string UserName { get; set; }
        //開始時間
        public string CreateDate { get; set; }
        //商品名稱
        public string ProductName { get; set; }
        //商品數量
        public int Count { get; set; }
        //送貨地址
        public string Address { get; set; }
        //電話
        public string Phone { get; set; }
        //付款類型
        public int? PaymentType { get; set; }
        //付款金額
        public decimal Price { get; set; }
        public bool Disable { get; set; } = false;
    }
    //商品管理
    public class ProductBackMangeViewModel
    {
        //產品編號
        public int ProductId { get; set; }
        //商品名稱
        public string ProductName { get; set; }
        //商品價錢
        public decimal Price { get; set; }
        //商品圖片
        public string PicPath { get; set; }
        //商品類型
        public string Category { get; set; }
        //商品開始時間
        public string CreateTime { get; set; }
        //商品結束時間
        public string EndTime { get; set; }
        //商品描述
        public string Description { get; set; }
        //商品內容
        public string Content { get; set; }
        //達標金額
        public decimal Total { get; set; }
        //團主名稱
        public string UserName { get; set; }
        public bool Disable { get; set; }
    }
    //首頁資訊
    public class HomeBackMangeViewModel
    {
        //使用者數量
        public int UserCount { get; set; }
        //產品數量
        public int ProductCount { get; set; }
        //處理中訂單  已完成訂單
        public int ProcessOrder { set; get; }
        //已完成訂單
        public int FinishOrder { get; set; }
        //平台營業總額
        public dynamic TotalSales { get; set; }
        //熱銷商品數量
        public int HotproductCount { get; set; }
        //熱銷產品名稱
        public string ProductName { get; set; }
        //user團主姓名
        public string Name { get; set; }
        //團主銷售額
        public decimal Totalturnover{ get; set;}
        //熱門產品數量
        public List<HotProduct> hotProducts { get; set; }
        //團主排行
        public List<SellerRanking> sellerRankings { get; set; }
        //圖表資訊
        public List<Graph> Graphs { get; set; }
    }
    public class HotProduct
    {
        //熱門產品名稱
        public string HotProductName { get; set; }
        //熱門產品數量
        public int HotProductCount { get; set; }
    }
    public class SellerRanking
    {
        //賣家id
        public int SellerId { get; set; }
        //賣家頭像圖片
        public string PicPath { get; set; }
        //賣家名稱
        public string SellerName { get; set; }
        //賣家銷售金額
        public dynamic Price { get; set; }    
    }
    //圖表
    public class Graph
    {
        //年份
        public int Year { get; set; }
        //月份
        public int Month { get; set; } 
        //月份金額
        public dynamic MonthPrice { get; set; }
    }
}
