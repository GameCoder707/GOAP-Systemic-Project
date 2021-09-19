using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : Element
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        MainUpdate();

        GetComponent<CapsuleCollider>().enabled = !hit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hit)
        {
            if ((other.gameObject.CompareTag("Player") && GetComponentInParent<PlayerBehaviour>() == null)
                ||
                (other.gameObject.CompareTag("Enemy") && GetComponentInParent<GeneralEnemy>() == null))
            {
                if (!other.gameObject.GetComponent<Entity>().isElectrified())
                    other.gameObject.GetComponent<Entity>().IncreaseElectricPoints();

                hit = true;
            }
            else if (other.gameObject.GetComponent<Weapon>() != null)
            {
                if (other.gameObject.GetComponent<Weapon>().weaponStatus != Weapon.WEAPON_STATUS.ELECTRIFIED &&
                    other.gameObject.GetComponent<Weapon>().conductive)
                    other.gameObject.GetComponent<Weapon>().ElectrifyWeapon();
            }
        }

    }
}
