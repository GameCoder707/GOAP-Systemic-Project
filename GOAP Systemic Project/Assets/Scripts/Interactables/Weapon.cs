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
    public GameObject electricEffect;

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
            if(transform.parent.gameObject.GetComponent<GeneralEnemy>() != null)
            {
                if (integrity <= 0)
                {
                    transform.parent.gameObject.GetComponent<EnemyStats>().hasWeapon = false;
                    Destroy(gameObject);
                }


                if (anim.GetBool("isSwinging"))
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("WeaponSwingAnim"))
                    {
                        anim.SetBool("isSwinging", false);
                        weaponHit = false;
                    }

                }

                GetComponent<CapsuleCollider>().enabled = anim.GetCurrentAnimatorStateInfo(0).IsName("WeaponSwingAnim");
            }

        }

    }

    public void DamagePlayer(PlayerBehaviour player)
    {
        if (integrity > 0)
        {
            player.InflictDamage(damage);

            integrity -= 1;
        }
    }

    public void BurnWeapon()
    {
        Instantiate(fireEffect, transform.position + transform.up, transform.rotation, transform);
        weaponStatus = WEAPON_STATUS.BURNING;
    }

    public void ElectrifyWeapon()
    {
        Instantiate(electricEffect, transform.position + transform.up, transform.rotation, transform);
        weaponStatus = WEAPON_STATUS.ELECTRIFIED;
    }

    public void WeaponPickedUp()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        anim.SetBool("isOwned", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!weaponHit)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                DamagePlayer(other.gameObject.GetComponent<PlayerBehaviour>());
                weaponHit = true;
            }

        }

    }
}
