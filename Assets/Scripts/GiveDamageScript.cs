using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GiveDamageScript : NetworkBehaviour
{
    private NetworkVariable<ulong> owner = new(999);
    private NetworkVariable<bool> isActiveSelf = new(true);

    public static event Action<(ulong from, ulong to)> OnHitPlayer; 

    [ServerRpc(RequireOwnership = false)]
    public void SetOwnershipServerRpc(ulong id)
    {
        this.owner.Value = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {

            if (other.transform.parent.TryGetComponent(out NetworkObject networkObject))
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    
                    (ulong, ulong) fromShooterToHit = new(owner.Value, networkObject.OwnerClientId);
                    Debug.Log("Kick has Collision to player"+fromShooterToHit);
                    OnHitPlayer?.Invoke(fromShooterToHit);
                    return;
                }
            }
        }
    }

}
