using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : Element
{
    public int power;

    // Start is called before the first frame update
    void Start()
    {
        power = 1;
        rainEffectTimer = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        MainUpdate();

        GetComponent<CapsuleCollider>().enabled = !hit;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.name.ToLower().Contains("weather"))
            {
                if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.RAINY)
                {
                    if (rainEffectTimer < 0)
                    {
                        power = 2;
                    }
                    else
                        rainEffectTimer -= Time.deltaTime;
                }
                else
                {
                    power = 1;
                    rainEffectTimer = 1.0f;
                }
            }
        }
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
                {
                    if (power == 0)
                        power = 1;

                    other.gameObject.GetComponent<Entity>().IncreaseElectricPoints(power);
                }


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
