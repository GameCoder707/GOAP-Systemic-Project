using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WEAPON_STATUS { NONE = 0, BURNING = 1, ELECTRIFIED = 2};

    public bool isOwned;
    public bool flammable;
    public bool conductive;

    public WEAPON_STATUS weaponStatus = WEAPON_STATUS.NONE;

    public GameObject fireEffect;

    // Health of the weapon
    public int integrity;

    private int damage = 5;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    protected void MainUpdate()
    {
        // Weapon is a child of the entity
        if(transform.parent != null)
        {
            if(integrity <= 0)
            {
                transform.parent.gameObject.GetComponent<EnemyStats>().hasWeapon = false;
                transform.parent.gameObject.GetComponent<EnemyStats>().combatReady = false;
                Destroy(gameObject);
            }
        }
    }

    public void DamagePlayer(PlayerBehaviour player)
    {
        if(integrity > 0)
        {
            if (weaponStatus != WEAPON_STATUS.NONE)
            {
                switch (weaponStatus)
                {
                    case WEAPON_STATUS.BURNING:
                        // Apply burn status
                        player.InflictDamage(damage, WEAPON_STATUS.BURNING);
                        break;
                    case WEAPON_STATUS.ELECTRIFIED:
                        // Apply shock status
                        player.InflictDamage(damage, WEAPON_STATUS.ELECTRIFIED);
                        break;
                }
            }
            else
                player.InflictDamage(damage);

            integrity -= 1;
        }
    }

    public void BurnWeapon()
    {
        Instantiate(fireEffect, transform.position + Vector3.up, transform.rotation, transform);
        weaponStatus = WEAPON_STATUS.BURNING;
    }
}
