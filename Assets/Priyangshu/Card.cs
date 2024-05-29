using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    // public Player player;
    GameObject player;
    public new string name;
    public string type;
    public Sprite artwork;

    public int level;
    public string card_id;

    // public int cost;

    public int damage;
    public int cooldownTime;

    public bool isPossessed;

    public virtual void upgradeCard()
    {
        Debug.Log("Card Upgraded");
    }

    public virtual void useCard()
    {
        Debug.Log("Card Used");
        player = GameObject.Find("Player");
        // do animations on player
    }
}
