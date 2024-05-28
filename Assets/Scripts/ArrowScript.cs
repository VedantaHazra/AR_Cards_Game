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

    [SerializeField]
    private GameObject arrowVFXPrefab;
    public bool rangedSpecialAttack;

    public float damage;
    public int range;

    void Awake(){
        rangedSpecialAttack = false;
        arrowVFXPrefab.SetActive(false);
    }

    void Start()
    {
        if(rangedSpecialAttack){
            arrowVFXPrefab.SetActive(true);
        }
    }

    void Update()
    {
        if(!shot) { return; }

        MoveServerRpc();
        time += Time.deltaTime;
        // if(time > timeSpan) {
        //     Destroy(gameObject);
        // }

    }
    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(){
        Vector3 position = transform.position + vector*speed;
        transform.position = position;
    }
    public void Shot(Vector3 vect)
    {
        shot = true;
        vector = vect;
        //transform.SetParent(null);
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
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {

            if (other.transform.parent.TryGetComponent(out NetworkObject networkObject))
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    
                    (ulong, ulong) fromShooterToHit = new(owner.Value, networkObject.OwnerClientId);
                    Debug.Log("Bullet has Collision to player"+fromShooterToHit);
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
