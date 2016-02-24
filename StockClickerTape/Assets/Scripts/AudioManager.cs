using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundEffect
{
    public AudioClip clip;
    public int relativeChance; 
}

public class AudioManager : MonoBehaviour
{
    public SoundEffect[] BuySFX;
    public SoundEffect[] SellSFX;

    protected AudioSource audioSource;

	// Use this for initialization
	void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log(name + ": AudioSource component not found!");
        }
        GameEvents.OnBuy += OnBuy;
        GameEvents.OnSell += OnSell;
	}

    protected AudioClip RelativeRandomSFX(SoundEffect[] SFX)
    {
        int totalChance = 0;
        if (SFX.Length <= 0)
        {
            return null;
        }
        int[] chanceArray = new int[SFX.Length];
        int idx = 0;
        foreach (SoundEffect effect in SFX)
        {
            totalChance += effect.relativeChance;
            chanceArray[idx++] = totalChance;
        }
        int rando = Random.Range(0, totalChance);
        for (int sfx = 0; sfx < SFX.Length; ++sfx)
        {
            if (rando < chanceArray[sfx])
            {
                return SFX[sfx].clip;
            }
        }
        return null;
    }

    public void OnBuy(Stock stock)
    {
        AudioClip clip = RelativeRandomSFX(BuySFX);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void OnSell(Stock stock)
    {
        AudioClip clip = RelativeRandomSFX(SellSFX);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
}
