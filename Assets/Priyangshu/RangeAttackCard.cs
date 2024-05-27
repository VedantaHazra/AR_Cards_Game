using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RangeAttackCard : Card
{
    public int range;
    public int speed;

    public GameObject projectile;

    public string AttackName;

    public void Attack()
    {
        Debug.Log("Range Attack");
    }

    public override void upgradeCard()
    {
        level++;
        damage += 5;
        speed += 3;
        cooldownTime -= 1;

        Debug.Log("Ranged Card Upgraded");
    }

    public override void useCard()
    {
        Debug.Log("Ranged Card Used");
        // do animations on player
        this.player = GameObject.Find("Hawkeye");
        player.GetComponent<HawkeyeController>().rangedSpecialAttack = true;
        player.GetComponent<HawkeyeController>().rangedAttack( speed, damage, range); 
    }
}
