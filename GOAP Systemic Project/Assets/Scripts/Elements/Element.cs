using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    protected bool hit = false;

    protected float hitDelay;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    protected void MainUpdate()
    {
        if (hit)
        {
            hitDelay -= Time.deltaTime;

            if (hitDelay <= 0)
            {
                hit = false;
                hitDelay = 0.5f;
            }
        }
    }
}
