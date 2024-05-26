using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MeleeAttackCard : Card
{
    public int damageRadius;
    public string AttackName;
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
    }
}
