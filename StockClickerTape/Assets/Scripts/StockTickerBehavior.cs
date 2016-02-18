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
    protected string m_strSymbol;
    public string Symbol
    {
        get
        {
            return m_strSymbol;
        }
    }

    protected float m_fPrice;
    public float Price
    {
        get
        {
            return m_fPrice;
        }
        set
        {
            m_fPrice = value;
        }
    }
    
    protected int m_nShares;
    public int Shares
    {
        get
        {
            return m_nShares;
        }
        set
        {
            m_nShares = value;
        }
    }

    protected int m_nID;
    public int ID
    {
        get
        {
            return m_nID;
        }
    }

    // called after ctor, before Start() by TickerGridBehavior
    public void InitializeStockSymbol(int ID, string symbol, float price, int shares, GameManager gameManager)
    {
        m_nID = ID;
        m_strSymbol = symbol;
        m_fPrice = price;
        m_nShares = shares;
        m_gameManager = gameManager;
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
            m_symbol.text = Symbol;
        }
        if (m_price != null)
        {
            m_price.text = m_fPrice.ToString("$#.00");
        }
        if (m_shares != null)
        {
            m_shares.text = m_nShares.ToString();
        }
        if (m_graph != null)
        {
        }
    }

    public void OnStockClicked()
    {
        Debug.Log(m_symbol.text + " clicked!");
        if (!m_gameManager.OnStockClicked(ID))
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void OnGameStep()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
