using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TickerTapeBehavior : MonoBehaviour
{
    public float ScrollSpeed;
    protected Vector3 m_originalPosition;
    protected bool m_bScrolling;
    protected Text m_tickerText;
    protected bool m_bInEvent;

	// Use this for initialization
	void Start()
    {
        m_bInEvent = false;
        m_bScrolling = false;
        m_originalPosition = transform.position;
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            switch (child.name)
            {
                case "TickerText":
                    m_tickerText = child.GetComponent<Text>();
                    break;
                default:
                    break;
            }
        }
        if (m_tickerText == null)
        {
            Debug.Log(name + ": TickerText component not found!");
        }
	}

    public void InitializeData()
    {
        GameEvents.OnMarketEvent += OnMarketEventEnter;
    }

    public void OnMarketEventEnter(MarketEvents marketEvent, Stock stock)
    {
        if (m_bInEvent)
        {
            return;
        }
        string str = marketEvent.EventText;
        m_bInEvent = true;
        m_bScrolling = true;
        string printStr = str;
        if (stock != null)
        {
            m_tickerText.text = str;
            int replaceIdx = str.IndexOf("%symbol%");
            while (replaceIdx != -1)
            {
                printStr = str.Remove(replaceIdx, "%symbol%".Length);
                printStr = printStr.Insert(replaceIdx, stock.Symbol);
                replaceIdx = str.IndexOf("%symbol%");
            }
            m_tickerText.text = printStr;
        }
        m_tickerText.transform.position = m_originalPosition;
    }

    public void OnMarketEventExit()
    {
        m_bScrolling = false;
        m_tickerText.text = string.Empty;
        m_tickerText.transform.position = m_originalPosition;
        m_bInEvent = false;
    }
	
	// Update is called once per frame
	void Update()
    {
	    if (m_bScrolling)
        {
            float xPos = m_tickerText.transform.position.x - ScrollSpeed * Time.deltaTime;
            
            if (xPos <= 0f)
            {
                xPos = 0f;
                m_bScrolling = false;
            }

            m_tickerText.transform.position = new Vector3(xPos, m_tickerText.transform.position.y, m_tickerText.transform.position.z);
        }
	}
}
