using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public GameStep gameStep;

    public float PlayerInitialCash;

    public string[] StockSymbols;

    public MarketEvents[] marketEvents;

    protected MarketEvents m_marketEvent;

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
        if (gameStep == null)
        {
            Debug.Log("GameManager: GameStep reference not set");
        }

        m_markets = new List<Stock>();
        m_portfolio = new List<Stock>();

        m_marketEvent = null;

        m_uniqueID = 0;
        
        foreach (string symbol in StockSymbols)
        {
            Stock stock = Stock.MakeStock(m_uniqueID++, symbol, 100f);
            if (stock != null)
            {
                m_markets.Add(stock);
            }
        }

        gameStep.SimulateTime();
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
            return;
        }

        // determine if any market events occur this frame
        if (m_marketEvent == null)
        {
            m_marketEvent = marketEvents[0];
            GameEvents.BroadcastMarketEvent(m_marketEvent, null);
            return;
        }
        /*
        List<MarketEvents> selected = new List<MarketEvents>();
        foreach (MarketEvents marketEvent in marketEvents)
        {
            float probability = Time.deltaTime * 1000f / (marketEvent.Periodicity * 1000f);
            float rando = Random.Range(0f, 1000f);
            if (rando < probability)
            {
                Debug.Log("Market Event: " + marketEvent.EventType.ToString());
                selected.Add(marketEvent);
            }
        }
        if (selected.Count > 0)
        {
            int select = Random.Range(0, selected.Count);
            m_marketEvent = selected[select];
            selected.Clear();
            Stock stockAffected = null;
            switch (m_marketEvent.StocksAffected)
            {
                case MarketEvents.EStocksAffected.One:
                    stockAffected = Markets[Random.Range(0, Markets.Count)];
                    break;
                case MarketEvents.EStocksAffected.All:
                    break;
                default:
                    break;
            }
            GameEvents.BroadcastMarketEvent(m_marketEvent, stockAffected);
            Debug.Log("Market Event: " + m_marketEvent.EventType.ToString());
        }
        */
    }

    public void OnStockClicked(int ID)
    {
        Stock marketStock = null;
        Stock portfolioStock = null;
        switch (uiManager.UIState) // don't like referencing uiManager here
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
                            marketStock = GetMarketStock(ID);
                            if (marketStock != null)
                            {
                                marketStock.Shares = 1;
                                marketStock.CostBasis = marketStock.CurrentPrice;
                                marketStock.SellValue = marketStock.CurrentPrice;
                                GameEvents.BroadcastSharesChanged(marketStock);
                                m_portfolio.Add(marketStock);
                                PlayerCash -= marketStock.CurrentPrice;
                                GameEvents.BroadcastCashChanged(PlayerCash);
                            }
                        }
                        else
                        {
                            ++portfolioStock.Shares; // could calculate other values based on the fundamentals of Shares and Price
                            portfolioStock.CostBasis += marketStock.CurrentPrice;
                            portfolioStock.SellValue = marketStock.CurrentPrice * portfolioStock.Shares;
                            GameEvents.BroadcastSharesChanged(portfolioStock);
                            PlayerCash -= marketStock.CurrentPrice;
                            GameEvents.BroadcastCashChanged(PlayerCash);
                        }
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
                        GameEvents.BroadcastSharesChanged(portfolioStock);
                        GameEvents.BroadcastCashChanged(PlayerCash);
                    }
                    if (portfolioStock.Shares <= 0)
                    {
                        m_portfolio.Remove(portfolioStock);
                        uiManager.OnCashChanged(PlayerCash);
                    }
                }
                break;
            default:
                break;
        }
        uiManager.OnCashChanged(PlayerCash);
    }

}
