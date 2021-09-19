using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
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
