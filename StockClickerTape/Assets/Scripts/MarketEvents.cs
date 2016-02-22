using UnityEngine;
using System.Collections;

[System.Serializable]
public class MarketEvents
{
    public enum EEventType
    {
        Bull,
        Bear,
        HighVolatility,
        LowVolatility,
        UpTrend,
        DownTrend
    }

    public enum EStocksAffected
    {
        One,
        All
    }

    public EEventType EventType;
    public float Periodicity;
    public float Duration;
    public string EventText;
    public EStocksAffected StocksAffected;
}