using UnityEngine;
using System.Collections;

public class GameEvents
{
    public delegate void CashChanged(float cash);
    public static event CashChanged OnCashChanged;

    public delegate void GameStep();
    public static event GameStep OnGameStep;

    public delegate void SharesChanged(Stock stock);
    public static event SharesChanged OnSharesChanged;

    public static void BroadcastCashChanged(float cash)
    {
        OnCashChanged(cash);
    }

    public static void BroadcastGameStep()
    {
        OnGameStep();
    }

    public static void BroadcastSharesChanged(Stock stock)
    {
        OnSharesChanged(stock);
    }
}
