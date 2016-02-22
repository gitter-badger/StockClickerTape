using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StockTickerBehavior : MonoBehaviour
{
    public Color GraphLineColor;
    //protected RenderTexture m_graphTexture;
    //protected Texture2D m_graphTexture;
    //public Texture2D TestTexture;
    public GameObject LinePrefab;
    public int HistoryDepth;
    public int ScaleRefreshSteps;
    protected int m_nStepsToScaleRefresh;

    protected GameManager m_gameManager;

    // UI
    protected Button m_button;
    protected Text m_symbol;
    protected Text m_price;
    protected Text m_shares;
    protected RectTransform m_graphRect;
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
        SetData();
    }

    public void SetData()
    {
        if (m_graph != null && m_symbol != null)
        {
            m_graph.gameObject.SetActive(true);
            m_symbol.gameObject.SetActive(true);
            if (StockRef != null)
            {
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
                if (m_graphRect != null)
                {

                }
            }
            m_nStepsToScaleRefresh = ScaleRefreshSteps;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_button = GetComponent<Button>();
        if (m_button == null)
        {
            Debug.Log("StockTickerBehavior: Could not find button component");
        }

        Transform[] children = GetComponentsInChildren<Transform>(true);
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
                    m_graphRect = child.GetComponent<RectTransform>();
                    break;
                default:
                    break;
            }
        }
        SetData();
    }

    public void OnDestroy()
    {
        ClearData();
    }

    public void OnStockClicked()
    {
        if (m_stock != null)
        {
            m_gameManager.OnStockClicked(StockRef.ID);
        }
    }

    public void OnGameStep()
    {
        if (m_price != null)
        {
            m_price.text = StockRef.CurrentPrice.ToString("$#.00");
        }
        DrawStockChart();
    }

    public void OnSharesChanged(Stock refStock)
    {
        if (StockRef.ID == refStock.ID) // I'd like the actual references to be the same -- should check this
        {
            if (StockRef.Shares <= 0)
            {
                //GameObject.Destroy(gameObject);
                ClearData();
            }
            else if (m_shares != null)
            {
                m_shares.text = StockRef.Shares.ToString();
            }
        }
    }

    protected float yScaleMin = 0f; // change these only when they need to be
    protected float yScaleMax = 0f;
    protected LineRenderer m_graphLines = null;
    protected Vector3[] m_graphLineVectors = null;
    // the lazy set -- these do NOT all need to be members
    protected float xMin;
    protected float xMax;
    protected float yMin;
    protected float yMax;
    protected float xCurrent;
    protected float xStep;
    protected float yDataMin;
    protected float yDataMax;

    public void DrawStockChart()
    {
        --m_nStepsToScaleRefresh;
        if (m_graphLines == null)
        {
            xMin = m_graphRect.position.x - m_graphRect.rect.xMax / 2;
            xMax = m_graphRect.position.x + m_graphRect.rect.xMax / 2;
            yMin = m_graphRect.position.y - m_graphRect.rect.yMax / 2;
            yMax = m_graphRect.position.y + m_graphRect.rect.yMax / 2;
            xCurrent = xMin;
            xStep = (xMax - xMin) / HistoryDepth;
            yDataMin = 100000f;
            yDataMax = 0f;
            for (int j = 0; j < HistoryDepth; ++j)
            {
                float stepPrice = StockRef.GetPriceHistoryFromCurrentStep(j);
                if (stepPrice < yDataMin)
                {
                    yDataMin = stepPrice;
                }
                if (stepPrice > yDataMax)
                {
                    yDataMax = stepPrice;
                }
            }
            
            GameObject goLine = GameObject.Instantiate(LinePrefab);
            m_graphLines = goLine.GetComponent<LineRenderer>();
            m_graphLineVectors = new Vector3[HistoryDepth];
            m_graphLines.SetVertexCount(HistoryDepth);
            m_graphLines.SetWidth(2f, 2f);

            for (int step = HistoryDepth - 1, idx = 0; step >= 0; --step, ++idx, xCurrent += xStep)
            {
                float stepPrice = StockRef.GetPriceHistoryFromCurrentStep(step);
                float yPrev = (stepPrice - yDataMin) / (yDataMax - yDataMin) * (yMax - yMin) + yMin;
                m_graphLineVectors[idx] = new Vector3(xCurrent, yPrev, -100f);
                m_graphLines.SetPosition(idx, m_graphLineVectors[idx]);
                m_graphLines.SetColors(GraphLineColor, GraphLineColor);
            }
        }
        else
        {
            // once the original graph has been set up, just remove the first vertex, shift the others over, and add a new one
            for (int i = 0; i < m_graphLineVectors.Length - 1; ++i)
            {
                m_graphLineVectors[i] = new Vector3(m_graphLineVectors[i + 1].x - xStep, m_graphLineVectors[i + 1].y, -100f);
            }
            float stepPrice = StockRef.GetPriceHistoryFromCurrentStep(0);
            float yPrev = (stepPrice - yDataMin) / (yDataMax - yDataMin) * (yMax - yMin) + yMin;
            m_graphLineVectors[m_graphLineVectors.Length - 1] = new Vector3(xCurrent, yPrev, -100f);

            if (m_nStepsToScaleRefresh <= 0 || yPrev > yDataMax || yPrev < yDataMin) // note -- overwrites all data, does not use pre-existing data (can do if-else)
            {
                m_nStepsToScaleRefresh = ScaleRefreshSteps;
                // TODO -- function -- and clean up member variables
                yDataMin = 100000f;
                yDataMax = 0f;
                for (int j = 0; j < HistoryDepth; ++j)
                {
                    stepPrice = StockRef.GetPriceHistoryFromCurrentStep(j);
                    if (stepPrice < yDataMin)
                    {
                        yDataMin = stepPrice;
                    }
                    if (stepPrice > yDataMax)
                    {
                        yDataMax = stepPrice;
                    }
                }

                xCurrent = xMin;

                for (int step = HistoryDepth - 1, idx = 0; step >= 0; --step, ++idx, xCurrent += xStep)
                {
                    stepPrice = StockRef.GetPriceHistoryFromCurrentStep(step);
                    yPrev = (stepPrice - yDataMin) / (yDataMax - yDataMin) * (yMax - yMin) + yMin;
                    m_graphLineVectors[idx] = new Vector3(xCurrent, yPrev, -100f);
                    m_graphLines.SetPosition(idx, m_graphLineVectors[idx]);
                    m_graphLines.SetColors(GraphLineColor, GraphLineColor);
                }
            }
            m_graphLines.SetPositions(m_graphLineVectors);
        }
    }

    public void OnScrollRectValueChanged(Vector2 vec2)
    {
        //Debug.Log(StockRef.Symbol + ": onScrollRectValueChanged(" + vec2.x + ", " + vec2.y + ")");
        if (m_stock != null)
        {
            for (int i = 0; i < HistoryDepth; ++i)
            {
                GameObject.Destroy(m_graphLines);
            }
            m_graphLines = null;
            DrawStockChart();
        }
    }

    public void ClearData()
    {
        m_graph.gameObject.SetActive(false);
        m_symbol.gameObject.SetActive(false);
        m_stock = null;
        GameEvents.OnGameStep -= OnGameStep;
        GameEvents.OnSharesChanged -= OnSharesChanged;
        if (m_graphLines != null)
        {
            GameObject.Destroy(m_graphLines);
        }

    }
}

