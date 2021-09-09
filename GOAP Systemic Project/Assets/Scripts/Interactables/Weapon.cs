using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WEAPON_STATUS { NONE = 0, BURNING = 1, ELECTRIFIED = 2 };

    public bool isOwned;
    public bool flammable;
    public bool conductive;

    public WEAPON_STATUS weaponStatus = WEAPON_STATUS.NONE;

    public GameObject fireEffect;

    private Animator anim;

    private bool weaponHit; // To ignore repeated collisions

    // Health of the weapon
    public int integrity;

    private int damage = 5;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        weaponHit = false;
    }

    // Update is called once per frame
    protected void MainUpdate()
    {
        // Weapon is a child of the entity
        if (transform.parent != null)
        {
            if (integrity <= 0)
            {
                transform.parent.gameObject.GetComponent<EnemyStats>().hasWeapon = false;
                transform.parent.gameObject.GetComponent<EnemyStats>().combatReady = false;
                Destroy(gameObject);
            }
        }

        if (anim.GetBool("isSwinging"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("WeaponSwingAnim"))
            {
                anim.SetBool("isSwinging", false);
                weaponHit = false;
            }

        }

    }

    public void DamagePlayer(PlayerBehaviour player)
    {
        if (integrity > 0)
        {
            //anim.SetBool("isSwinging", true);

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
        Instantiate(fireEffect, transform.position + transform.up, transform.rotation, transform);
        weaponStatus = WEAPON_STATUS.BURNING;
    }

    public void WeaponPickedUp()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        anim.SetBool("isOwned", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!weaponHit)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                DamagePlayer(other.gameObject.GetComponent<PlayerBehaviour>());
                weaponHit = true;
            }

        }

    }
}
