using UnityEngine;
using System.Collections;

public class GameEvents
{
    public delegate void CashChanged(int cash);
    public CashChanged OnCashChanged;

    public delegate void GameStep();
    public GameStep OnGameStep;
}
