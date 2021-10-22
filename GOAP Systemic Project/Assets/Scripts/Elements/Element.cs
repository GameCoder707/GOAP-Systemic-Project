using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    protected bool hit = false;

    protected float hitDelay;
    protected float rainEffectTimer;

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

        if(transform.parent != null)
        {
            if(transform.parent.gameObject.GetComponent<Weapon>() != null)
            {
                if (!transform.parent.gameObject.GetComponent<Animator>().GetBool("isSwinging"))
                    GetComponent<SphereCollider>().enabled = false;
                else
                    GetComponent<SphereCollider>().enabled = !hit;
            }
            
        }
    }
}
