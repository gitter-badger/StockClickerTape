using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TickerGridBehavior : MonoBehaviour
{
    public GameObject StockTicker;
    public GameManager gameManager;
    public int NumberOfStockPanels;

    protected ScrollRect m_scrollRect;
    //protected List<StockTickerBehavior> m_displayStocks;
    protected List<GameObject> m_goStockPanels;

	// Use this for initialization
	void Start()
    {

        if (StockTicker == null)
        {
            Debug.Log("TickerGridBehavior: no StockTicker prefab set");
            return;
        }

        m_goStockPanels = new List<GameObject>();
        //m_displayStocks = new List<StockTickerBehavior>();

        m_scrollRect = GetComponentInParent<ScrollRect>();
        if (m_scrollRect == null)
        {
            Debug.Log(this.name + ": ScrollRect component not found");
        }

        for (int i = 0; i < NumberOfStockPanels; ++i)
        {
            GameObject goStockTicker = GameObject.Instantiate(StockTicker);
            if (goStockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not instantiate StockTicker prefab");
                continue;
            }
            goStockTicker.transform.SetParent(transform);
            m_goStockPanels.Add(goStockTicker);
        }

        m_scrollRect.verticalNormalizedPosition = 1f;
    }

    public void DisplayMarkets(List<Stock> markets)
    {
        Debug.Log(name + " DisplayMarkets()");
        // delete previous elements
        //m_scrollRect.onValueChanged.RemoveAllListeners();
        m_scrollRect.onValueChanged.RemoveAllListeners();
        /*
        foreach (StockTickerBehavior stockUI in m_displayStocks)
        {
            stockUI.ClearData();
        }
        m_displayStocks.Clear();
        */

        // make grid of stock ticker symbols
        List<GameObject>.Enumerator iterStockTicker = m_goStockPanels.GetEnumerator();
        int i = 0;
        foreach (Stock stock in markets)
        {
            // instantiate the grid UI object
            GameObject goStockTicker = m_goStockPanels[i++];
            if (goStockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not instantiate StockTicker prefab");
                continue;
            }
            iterStockTicker.MoveNext();

            // get the behavior for this symbol
            StockTickerBehavior stockTicker = goStockTicker.GetComponent<StockTickerBehavior>();

            if (stockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not find StockTickerBehavior component in StockTicker prefab");
                //GameObject.Destroy(goStockTicker);
            }
            //m_displayStocks.Add(stockTicker);
            stockTicker.InitializeStockSymbol(stock, gameManager);
            m_scrollRect.onValueChanged.AddListener(stockTicker.OnScrollRectValueChanged);
        }
    }

    public void DisplayPortfolio(List<Stock> portfolio) // TODO -- remove parameter
    {
        //DisplayMarkets(portfolio);

        Debug.Log(name + " DisplayPortfolio()");
        // delete previous elements
        m_scrollRect.onValueChanged.RemoveAllListeners();
        /*
        foreach (StockTickerBehavior stockUI in m_displayStocks)
        {
            stockUI.ClearData();
        }
        m_displayStocks.Clear();
        */

        // make grid of stock ticker symbols
        List<GameObject>.Enumerator iterStockTicker = m_goStockPanels.GetEnumerator();
        int i = 0;
        foreach (Stock stock in gameManager.Markets)
        {
            // instantiate the grid UI object
            GameObject goStockTicker = m_goStockPanels[i++];
            if (goStockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not instantiate StockTicker prefab");
                continue;
            }
            iterStockTicker.MoveNext();

            // get the behavior for this symbol
            StockTickerBehavior stockTicker = goStockTicker.GetComponent<StockTickerBehavior>();

            if (stockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not find StockTickerBehavior component in StockTicker prefab");
                //GameObject.Destroy(goStockTicker);
            }
            //m_displayStocks.Add(stockTicker);
            if (stock.Shares > 0)
            {
                stockTicker.InitializeStockSymbol(stock, gameManager);
                m_scrollRect.onValueChanged.AddListener(stockTicker.OnScrollRectValueChanged);
            }
            else
            {
                stockTicker.ClearData();
            }
        }
    }
    
}
