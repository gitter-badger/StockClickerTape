using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public enum EUIState
    {
        Start,
        Markets,
        Portfolio
    }

    protected EUIState m_uiState;
    public EUIState UIState
    {
        get
        {
            return m_uiState;
        }
    }

    protected TickerGridBehavior m_tickerGrid;
    protected Text m_playerCash;
    protected TickerTapeBehavior m_tickerTape;

    public void Start()
    {
        m_uiState = EUIState.Start;
        if (gameManager == null)
        {
            Debug.Log("UIManager: GameManager reference not set!");
        }

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            switch (child.name)
            {
                case "TickerGrid":
                    m_tickerGrid = child.GetComponent<TickerGridBehavior>();
                    break;
                case "PlayerCash":
                    m_playerCash = child.GetComponent<Text>();
                    break;
                case "TickerTape":
                    m_tickerTape = child.GetComponent<TickerTapeBehavior>();
                    break;
                default:
                    break;
            }
        }
        if (m_tickerGrid == null)
        {
            Debug.Log("UIManager: TickerGrid component not found");
        }
        if (m_playerCash == null)
        {
            Debug.Log("UIManager: PlayerCash component not found");
        }
        if (m_tickerTape == null)
        {
            Debug.Log("UIManager: TickerText component not found");
        }

        GameEvents.OnGameStep += OnGameStep;
        GameEvents.OnSharesChanged += OnSharesChanged;
        GameEvents.OnCashChanged += OnCashChanged;
        GameEvents.OnMarketEvent += OnMarketEvent;

        m_tickerTape.InitializeData();
    }

    public void OnCashChanged(int cash)
    {

    }

    public void OnGameStep()
    {

    }

    public void OnSharesChanged(Stock stock)
    {

    }

    public void OnMarketEvent(MarketEvents marketEvent, Stock stock)
    {

    }

    public void OnCashChanged(float cash)
    {
        m_playerCash.text = cash.ToString("$#.00");
    }

    public void DisplayMarkets(List<Stock> markets)
    {
        m_tickerGrid.DisplayMarkets(markets);
    }

    public void DisplayPortfolio(List<Stock> portfolio)
    {
        m_tickerGrid.DisplayPortfolio(portfolio);
    }

    public void OnClickMarkets()
    {
        if (UIState != EUIState.Markets)
        {
            // change to Markets view
            DisplayMarkets(gameManager.Markets);
            m_uiState = EUIState.Markets;
        }
    }

    public void OnClickPortfolio()
    {
        if (UIState != EUIState.Portfolio)
        {
            // change to Portfolio view
            DisplayPortfolio(gameManager.Portfolio);
            m_uiState = EUIState.Portfolio;
        }
    }
}
