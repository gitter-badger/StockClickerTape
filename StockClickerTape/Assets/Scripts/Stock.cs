using UnityEngine;
using System.Collections;


public class Stock
{
    public static Stock MakeStock(int id, string symbol, float price)
    {
        Stock stock = new Stock();
        stock.Trend = ETrend.Normal;
        stock.Volatility = EVolatility.Normal;
        stock.ID = id;
        stock.Symbol = symbol;
        stock.Shares = 0;
        stock.CostBasis = 0f;
        stock.SellValue = 0f;
        stock.CurrentPrice = price;
        
        return stock;
    }

    public enum ETrend // affects general direction of price movement
    {
        Normal,
        Bull,
        Bear
    }

    public enum EVolatility // affects magnitude of price movement
    {
        Normal,
        Low,
        High
    }

    public ETrend Trend;
    public EVolatility Volatility;
    public int ID;
    public string Symbol;
    public int Shares;
    public float CostBasis;
    public float SellValue; // don't need it
    public float CurrentPrice;
}
