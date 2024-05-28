using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SlashScript : MonoBehaviour
{
    
    public float speedScale;
    public AnimationCurve velocityCurve;

    float startTime;

    void Start()
    {
        startTime = Time.time;
        Debug.Log("Slash Created");
    }

    // Update is called once per frame
    void Update()
    {
        //move along foward axis of the object with velocity curve
        transform.position += transform.forward * speedScale* velocityCurve.Evaluate(Time.time  - startTime) * Time.deltaTime;

        if(Time.time  - startTime > 2.2f)
        {
            Destroy(gameObject);
        }
    }
}
