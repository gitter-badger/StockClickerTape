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
    }

    public void DisplayMarkets(List<Stock> markets)
    {
        m_tickerGrid.DisplayMarkets(markets);
    }

    public void DisplayPortfolio(List<Stock> portfolio)
    {
        m_tickerGrid.DisplayPortfolio(portfolio);
    }

    public void OnCashChanged(float cash)
    {
        m_playerCash.text = cash.ToString("$#.00");
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
            DisplayMarkets(gameManager.Portfolio);
            m_uiState = EUIState.Portfolio;
        }
    }
}
