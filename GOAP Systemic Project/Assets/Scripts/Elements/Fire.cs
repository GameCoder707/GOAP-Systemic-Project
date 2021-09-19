using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Element
{
    // Start is called before the first frame update
    void Start()
    {
        hitDelay = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        MainUpdate();

        GetComponent<SphereCollider>().enabled = !hit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hit)
        {
            if ((other.gameObject.CompareTag("Player") && GetComponentInParent<PlayerBehaviour>() == null)
                ||
                (other.gameObject.CompareTag("Enemy") && GetComponentInParent<GeneralEnemy>() == null))
            {
                if (!other.gameObject.GetComponent<Entity>().isBurning())
                    other.gameObject.GetComponent<Entity>().burnPoints += 1;

                hit = true;
            }
            else if (other.gameObject.GetComponent<Weapon>() != null)
            {
                if (other.gameObject.GetComponent<Weapon>().weaponStatus != Weapon.WEAPON_STATUS.BURNING &&
                    other.gameObject.GetComponent<Weapon>().flammable)
                    other.gameObject.GetComponent<Weapon>().BurnWeapon();
            }
        }

    }

}
