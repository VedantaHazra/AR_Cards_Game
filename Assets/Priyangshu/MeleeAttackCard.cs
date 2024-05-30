using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
using UnityEngine;

[CreateAssetMenu]
public class MeleeAttackCard : Card
{
    public int damageRadius;
    public string AttackName;

    public GameObject slash;

    // public int speed;

    public void Attack()
    {
        Debug.Log("Melee Attack");
    }        

    public override void upgradeCard()
    {
        level++;
        damage += 20;
        damageRadius += 3;
        cooldownTime -= 1;

        Debug.Log("Melee Card Upgraded");
    }

    public override void useCard()
    {
        Debug.Log("Melee Card Used");
        // do animations on player
        this.player = GameObject.Find("Hawkeye");
        if(player == null)
        {
            this.player = GameObject.Find("Player 1(Clone)");
        }
        Debug.Log("Player: " + player);
        HawkeyeController HC = player.GetComponent<HawkeyeController>();
        if(HC != null)
        {
            HC.slashAttack(slash, damageRadius, damage);
        }
        else{
            NetworkPlayerMovement PC = player.GetComponent<NetworkPlayerMovement>();
            PC.slashAttack(slash, damageRadius, damage);
        }
        Debug.Log("Player: " + player);
        
    }
}
