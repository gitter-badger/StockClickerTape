using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TickerGridBehavior : MonoBehaviour
{
    public GameObject StockTicker;
    public GameManager gameManager;

    protected List<GameObject> m_displayStocks;

	// Use this for initialization
	void Start()
    {

        if (StockTicker == null)
        {
            Debug.Log("TickerGridBehavior: no StockTicker prefab set");
            return;
        }

        m_displayStocks = new List<GameObject>();

	}

    public void DisplayMarkets(List<Stock> markets)
    {
        Debug.Log("Found " + markets.Count + " symbols");

        // delete previous elements
        foreach (GameObject go in m_displayStocks)
        {
            GameObject.Destroy(go);
        }

        // make grid of stock ticker symbols
        foreach (Stock stock in markets)
        {
            // instantiate the grid UI object
            GameObject goStockTicker = GameObject.Instantiate(StockTicker);
            if (goStockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not instantiate StockTicker prefab");
                continue;
            }

            // get the behavior for this symbol
            StockTickerBehavior stockTicker = goStockTicker.GetComponent<StockTickerBehavior>();
            goStockTicker.transform.SetParent(this.transform);

            if (stockTicker == null)
            {
                Debug.Log("TickerGridBehavior: could not find StockTickerBehavior component in StockTicker prefab");
                GameObject.Destroy(goStockTicker);
            }
            m_displayStocks.Add(goStockTicker);
            stockTicker.InitializeStockSymbol(stock, gameManager);
        }
    }

    public void DisplayPortfolio(List<Stock> portfolio)
    {
        // delete previous elements
        foreach (GameObject go in m_displayStocks)
        {
            GameObject.Destroy(go);
        }

        DisplayMarkets(portfolio);
    }

    public void RefreshView()
    {
        //foreach (GameObject go in m_displayStocks)
        {

        }
    }
}
