using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ArrowScript : NetworkBehaviour
{
    public bool shot = false; 
    [SerializeField] private float speed = 1f; 
    [SerializeField] private float timeSpan = 30f; 
    private Vector3 vector;
    private float time = 0f;
    

    private NetworkVariable<ulong> owner = new(999);
    private NetworkVariable<bool> isActiveSelf = new(true);

    public static event Action<(ulong from, ulong to)> OnHitPlayer; 
    private const int MAX_FLY_TIME = 5;


    void Update()
    {
        if(!shot) { return; }

        Vector3 position = transform.position + vector*speed;
        transform.position = position;
        time += Time.deltaTime;
        // if(time > timeSpan) {
        //     Destroy(gameObject);
        // }

    }
    public void Shot(Vector3 vect)
    {
        shot = true;
        vector = vect;
        transform.SetParent(null);
        DeactivateSelfDelay();
    }

    public override void OnNetworkSpawn()
    {
        //DeactivateSelfDelay();
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetOwnershipServerRpc(ulong id)
    {
        this.owner.Value = id;
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetBulletIsActiveServerRpc(bool isActive)
    {
        if(!GetComponent<NetworkObject>()) return;
        
        
        isActiveSelf.Value = isActive;

        if (isActive == false)
        {
            GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            GetComponent<NetworkObject>().Spawn();
        }
    }


    public void DeactivateSelfDelay()
    {
        StartCoroutine(DeactivateSelfDelayCoroutine());
    }

    IEnumerator DeactivateSelfDelayCoroutine()
    {
        yield return new WaitForSeconds(MAX_FLY_TIME);
        SetBulletIsActiveServerRpc(false);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            if (collision.transform.TryGetComponent(out NetworkObject networkObject))
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Debug.Log("Bullet has Collision to player");
                    (ulong, ulong) fromShooterToHit = new(owner.Value, networkObject.OwnerClientId);
                    OnHitPlayer?.Invoke(fromShooterToHit);
                    SetBulletIsActiveServerRpc(false);
                    return;
                }
            }
            else
            {
                SetBulletIsActiveServerRpc(false);
            }
        }
    }
}
