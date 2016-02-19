using UnityEngine;
using System.Collections;

public class GameStep : MonoBehaviour
{
    public GameManager gameManager;
    public float StartupSimulationTime;
    public float StepTime;

    protected float fLastStep;

	// Use this for initialization
	void Start()
    {
	    if (gameManager == null)
        {
            Debug.Log("GameStep: GameManager not set!");
        }
        // GameStep won't start until a call to SimulateTime()
	}

    public void SimulateTime()
    {
        Debug.Log("Simulating " + StartupSimulationTime + " seconds of gameplay...");
        float runner = 0f;
        for (runner = StepTime; runner <= StartupSimulationTime; runner += StepTime)
        {
            foreach (Stock stock in gameManager.Markets)
            {
                GenerateNextPrice(stock);
            }
        }
        // account for fractions of a game step by subtracting the fraction from the current time
        fLastStep = Time.time - (StartupSimulationTime - runner - StepTime); // total sim time - simulated time

        Stock testStock = gameManager.Markets[0];
        Debug.LogFormat(testStock.Symbol + " history: " +
            testStock.GetPriceHistoryFromCurrentStep(4) + ", " +
            testStock.GetPriceHistoryFromCurrentStep(3) + ", " +
            testStock.GetPriceHistoryFromCurrentStep(2) + ", " +
            testStock.GetPriceHistoryFromCurrentStep(1) + ", " +
            testStock.GetPriceHistoryFromCurrentStep(0));
    }
	
	// Update is called once per frame
	void Update()
    {
        if (Time.time > fLastStep + StepTime)
        {
            fLastStep = Time.time;
            foreach (Stock stock in gameManager.Markets)
            {
                GenerateNextPrice(stock);
            }
            GameEvents.BroadcastGameStep();
        }
    }

    void GenerateNextPrice(Stock stock)
    {
        // determine directions
        int positiveProbability = 0;
        switch (stock.Trend)
        {
            case Stock.ETrend.Normal:
                positiveProbability = 50;
                break;
            case Stock.ETrend.Bull:
                positiveProbability = 67;
                break;
            case Stock.ETrend.Bear:
                positiveProbability = 33;
                break;
            default:
                break;

        }

        bool positive = false;
        if (Random.Range(0, 100) < positiveProbability)
        {
            positive = true;
        }
        else
        {
            positive = false;
        }

        float maxDelta = 0f;
        switch (stock.Volatility)
        {
            case Stock.EVolatility.Normal:
                maxDelta = .5f;
                break;
            case Stock.EVolatility.High:
                maxDelta = 1.5f;
                break;
            case Stock.EVolatility.Low:
                maxDelta = .2f;
                break;
            default:
                break;
        }

        float multiplier = Random.Range(0f, maxDelta);

        stock.CurrentPrice += ((positive == true) ? (1) : (-1)) * multiplier;

    }
}
