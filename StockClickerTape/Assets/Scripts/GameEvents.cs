using UnityEngine;
using System.Collections;

public class GameEvents
{
    public delegate void CashChanged(float cash);
    public static event CashChanged OnCashChanged;

    public delegate void GameStep();
    public static event GameStep OnGameStep;

    public delegate void Buy(Stock stock);
    public static event Buy OnBuy;

    public delegate void Sell(Stock stock);
    public static event Sell OnSell;

    public delegate void MarketEvent(MarketEvents marketEvent, Stock stock);
    public static event MarketEvent OnMarketEvent;

    public static void BroadcastCashChanged(float cash)
    {
        OnCashChanged(cash);
    }

    public static void BroadcastGameStep()
    {
        OnGameStep();
    }

    public static void BroadcastBuy(Stock stock)
    {
        OnBuy(stock);
    }

    public static void BroadcastSell(Stock stock)
    {
        OnSell(stock);
    }

    public static void BroadcastMarketEvent(MarketEvents marketEvent, Stock stock)
    {
        OnMarketEvent(marketEvent, stock);
    }
}
