using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Element
{
    // Start is called before the first frame update
    void Start()
    {
        hitDelay = 0.5f;
        rainEffectTimer = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        MainUpdate();
    }

    private void FixedUpdate()
    {
        if (!gameObject.name.ToLower().Contains("fire source")) // Rain doesn't affect fire sources
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
                            if (GetComponentInParent<Weapon>() != null)
                                GetComponentInParent<Weapon>().weaponStatus = Weapon.WEAPON_STATUS.NONE;

                            Destroy(gameObject);
                        }
                        else
                            rainEffectTimer -= Time.deltaTime;
                    }

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
                if (!other.gameObject.GetComponent<Entity>().isBurning())
                    other.gameObject.GetComponent<Entity>().IncreaseBurnPoints();

                hit = true;
            }
            else if (other.gameObject.GetComponent<Weapon>() != null)
            {
                if (other.gameObject.GetComponent<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.NONE &&
                    other.gameObject.GetComponent<Weapon>().flammable)
                    other.gameObject.GetComponent<Weapon>().BurnWeapon();
            }
            else if (other.gameObject.GetComponent<Boulder>() != null)
            {
                if (!other.gameObject.GetComponent<Boulder>().isStatusApplied)
                    other.gameObject.GetComponent<Boulder>().BurnBoulder();
            }
        }

    }

}
