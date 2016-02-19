using UnityEngine;
using System.Collections;


public class Stock
{
    public static int HISTORY_SIZE = 100;

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

        stock.m_priceHistory = new float[HISTORY_SIZE];
        stock.CurrentPrice = price;


        
        return stock;
    }

    protected Stock()
    {
        m_nPriceRunner = 0;
    }

    private Stock(Stock stock)
    {

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

    protected float m_fCurrentPrice;
    public float CurrentPrice
    {
        get
        {
            return m_fCurrentPrice;
        }
        set
        {
            m_fCurrentPrice = value;
            ++m_nPriceRunner;
            if (m_nPriceRunner >= HISTORY_SIZE)
            {
                m_nPriceRunner = 0;
            }
            m_priceHistory[m_nPriceRunner] = value;
        }
    }

    protected float[] m_priceHistory;
    protected int m_nPriceRunner;

    protected int GetPriceHistoryIndexFromCurrentStep(int step)
    {
        int idx;
        if (step >= HISTORY_SIZE) // oldest stored index is one after the current one
        {
            step = HISTORY_SIZE - 1;
        }
        idx = (m_nPriceRunner - step + HISTORY_SIZE) % HISTORY_SIZE; // I don't feel like dealing with -modulus
        return idx;
    }

    public float GetPriceHistoryFromCurrentStep(int step) // idx is the number of steps back to retrieve
    {
        // convert step from "steps back" to "theoretical index" to "index from runner"
        if (step >= HISTORY_SIZE) // too far back, just return furthest back index
        {
            step = HISTORY_SIZE - 1;
        }
        int idx = GetPriceHistoryIndexFromCurrentStep(step);
        return m_priceHistory[idx];
    }
}
