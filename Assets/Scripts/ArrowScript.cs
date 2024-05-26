using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public bool shot = false; 
    [SerializeField] private float speed = 1f; 
    [SerializeField] private float timeSpan = 30f; 
    private Vector3 vector;
    private float time = 0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(!shot) { return; }
        Vector3 position = transform.position + vector*speed;
        transform.position = position;
        time += Time.deltaTime;
        if(time > timeSpan) {
            Destroy(gameObject);
        }

    }
    public void Shot(Vector3 vect)
    {
        shot = true;
        vector = vect;
        transform.SetParent(null);
    }
}
