using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private bool hit = false;

    private float hitDelay = 0.3f;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            hitDelay -= Time.deltaTime;

            if (hitDelay <= 0)
            {
                hit = false;
                hitDelay = 0.3f;
            }
        }

        GetComponent<SphereCollider>().enabled = !hit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hit)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerBehaviour>().burnPoints += 1;
                hit = true;
            }
            else if (other.gameObject.GetComponent<Weapon>() != null)
            {
                if (other.gameObject.GetComponent<Weapon>().weaponStatus != Weapon.WEAPON_STATUS.BURNING)
                    other.gameObject.GetComponent<Weapon>().BurnWeapon();
            }
        }

    }

}
