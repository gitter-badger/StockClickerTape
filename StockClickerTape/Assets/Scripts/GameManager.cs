using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameEvents gameEvents;

    public UIManager uiManager;

    public float PlayerInitialCash;

    public string[] StockSymbols;

    protected List<Stock> m_markets;
    public List<Stock> Markets
    {
        get
        {
            return m_markets;
        }
    }

    protected List<Stock> m_portfolio;
    public List<Stock> Portfolio
    {
        get
        {
            return m_portfolio;
        }
    }

    protected float m_playerCash;
    public float PlayerCash
    {
        get
        {
            return m_playerCash;
        }
        set
        {
            m_playerCash = value;
        }
    }

    protected int m_uniqueID;

	void Start()
    {
        PlayerCash = PlayerInitialCash;

        if (uiManager == null)
        {
            Debug.Log("GameManager: UIManager reference not set");
        }

        m_markets = new List<Stock>();
        m_portfolio = new List<Stock>();

        m_uniqueID = 0;
        
        foreach (string symbol in StockSymbols)
        {
            Stock stock = new Stock();
            if (stock != null)
            {
                stock.ID = m_uniqueID++;
                stock.Symbol = symbol;
                stock.BuyValue = 0f;
                stock.SellValue = 0f;
                stock.Shares = 0;
                stock.CurrentPrice = 100f;
            }
            m_markets.Add(stock);
        }
    }

    public Stock GetMarketStock(int ID)
    {
        foreach (Stock stock in Markets)
        {
            if (stock.ID == ID)
            {
                return stock;
            }
        }
        return null;
    }

    public Stock GetPortfolioStock(int ID)
    {
        foreach (Stock stock in Portfolio)
        {
            if (stock.ID == ID)
            {
                return stock;
            }
        }
        return null;
    }

    protected bool m_bSetup = false;

	// Update is called once per frame
	void Update()
    {
	    if (!m_bSetup)
        {
            m_bSetup = true;
            uiManager.OnClickMarkets();
            uiManager.OnCashChanged(PlayerCash);
        }
	}
    
    public bool OnStockClicked(int ID)
    {
        Stock marketStock = null;
        Stock portfolioStock = null;
        switch (uiManager.UIState)
        {
            case UIManager.EUIState.Markets: // BUY
                // find the stock
                marketStock = GetMarketStock(ID);
                if (marketStock != null)
                {
                    // check if the player has available funds
                    if (PlayerCash >= marketStock.CurrentPrice)
                    {
                        // check if this stock is already in the player's portfoli
                        portfolioStock = GetPortfolioStock(ID);
                        if (portfolioStock == null)
                        {
                            portfolioStock = new Stock();
                            portfolioStock.ID = ID;
                            portfolioStock.Symbol = marketStock.Symbol;
                            portfolioStock.Shares = 1;
                            portfolioStock.CurrentPrice = marketStock.CurrentPrice;
                            portfolioStock.BuyValue = marketStock.CurrentPrice;
                            portfolioStock.SellValue = marketStock.CurrentPrice; // should account for bonuses that let you buy more than 1 share temporarily
                            m_portfolio.Add(portfolioStock);
                        }
                        else
                        {
                            ++portfolioStock.Shares;
                            portfolioStock.BuyValue += marketStock.CurrentPrice;
                            portfolioStock.SellValue = marketStock.CurrentPrice * portfolioStock.Shares;
                        }
                        PlayerCash -= marketStock.CurrentPrice;
                    }
                }
                break;
            case UIManager.EUIState.Portfolio: // SELL
                // find the stock
                portfolioStock = GetPortfolioStock(ID);
                if (portfolioStock != null)
                {
                    // check if the player has shares
                    if (portfolioStock.Shares > 0)
                    {
                        --portfolioStock.Shares;
                        PlayerCash += portfolioStock.CurrentPrice;
                    }
                    if (portfolioStock.Shares <= 0)
                    {
                        m_portfolio.Remove(portfolioStock);
                        uiManager.OnCashChanged(PlayerCash);
                        return false;
                    }
                }
                break;
            default:
                break;
        }
        uiManager.OnCashChanged(PlayerCash);
        return true;
    }

}
