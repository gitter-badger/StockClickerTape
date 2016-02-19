using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StockTickerBehavior : MonoBehaviour
{
    public GameObject TickerGrid; // to communicate back up to the parent -- might just have the parent spawn these prefabs and set up to trigger events
    protected GameManager m_gameManager;

    // UI
    protected Button m_button;
    protected Text m_symbol;
    protected Text m_price;
    protected Text m_shares;
    protected RawImage m_graph;

    // data
    protected Stock m_stock;
    public Stock StockRef
    {
        get
        {
            return m_stock;
        }
    }

    // called after ctor, before Start() by TickerGridBehavior
    public void InitializeStockSymbol(Stock stock, GameManager gameManager)
    {
        m_stock = stock;
        m_gameManager = gameManager;
        GameEvents.OnSharesChanged += OnSharesChanged;
        GameEvents.OnGameStep += OnGameStep;
    }

    // Use this for initialization
    void Start()
    {
        m_button = GetComponent<Button>();
        if (m_button == null)
        {
            Debug.Log("StockTickerBehavior: Could not find button component");
        }

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            switch (child.name)
            {
                case "Symbol":
                    m_symbol = child.GetComponent<Text>();
                    break;
                case "CurrentPrice":
                    m_price = child.GetComponent<Text>();
                    break;
                case "CurrentShares":
                    m_shares = child.GetComponent<Text>();
                    break;
                case "Graph":
                    m_graph = child.GetComponent<RawImage>();
                    break;
                default:
                    break;
            }
        }
        if (m_symbol != null)
        {
            m_symbol.text = StockRef.Symbol;
        }
        if (m_price != null)
        {
            m_price.text = StockRef.CurrentPrice.ToString("$#.00");
        }
        if (m_shares != null)
        {
            m_shares.text = StockRef.Shares.ToString();
        }
        if (m_graph != null)
        {
        }
    }

    public void OnDestroy()
    {
        GameEvents.OnGameStep -= OnGameStep;
        GameEvents.OnSharesChanged -= OnSharesChanged;
    }

    public void OnStockClicked()
    {
        Debug.Log(m_symbol.text + " clicked!");
        m_gameManager.OnStockClicked(StockRef.ID);
    }

    public void OnGameStep()
    {
        if (m_price != null)
        {
            m_price.text = StockRef.CurrentPrice.ToString("$#.00");
        }
    }

    public void OnSharesChanged(Stock refStock)
    {
        if (StockRef.ID == refStock.ID) // I'd like the actual references to be the same -- should check this
        {
            if (StockRef.Shares <= 0)
            {
                GameObject.Destroy(gameObject);
            }
            else if (m_shares != null)
            {
                m_shares.text = StockRef.Shares.ToString();
            }
        }
    }
}
