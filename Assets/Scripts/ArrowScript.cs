using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public bool shot = false; 
    [SerializeField] private float speed = 1f; 
    private Vector3 vector;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(shot)
        {
            Vector3 position = transform.position + vector*speed;
            transform.position = position;
        }
    }
    public void Shot(Vector3 vect)
    {
        shot = true;
        vector = vect;
        transform.SetParent(null);
    }
}
