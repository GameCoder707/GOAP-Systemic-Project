using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WEAPON_STATUS { NONE = 0, BURNING = 1, ELECTRIFIED = 2 };

    public bool isOwned;
    public Entity owner;
    public bool flammable;
    public bool conductive;

    public WEAPON_STATUS weaponStatus = WEAPON_STATUS.NONE;

    public GameObject fireEffect;
    public GameObject electricEffect;

    public Animator anim;

    private bool weaponHit; // To ignore repeated collisions
    private bool attackStance; // Is the weapon ready to attack

    // Health of the weapon
    public int integrity;

    protected int damage;

    // Start is called before the first frame update
    protected void MainStart()
    {
        anim = GetComponent<Animator>();

        if (transform.parent != null)
        {
            if (transform.parent.gameObject.GetComponent<Entity>() != null)
                WeaponPickedUp(transform.parent.gameObject.GetComponent<Entity>());
        }

        weaponHit = false;
        attackStance = false;
    }

    // Update is called once per frame
    protected void MainUpdate()
    {
        // Weapon is a child of the entity
        if (transform.parent != null)
        {
            if (transform.parent.gameObject.GetComponent<GeneralEnemy>() != null ||
                transform.parent.gameObject.GetComponent<PlayerBehaviour>() != null)
            {
                attackStance = true;

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
        if (weaponStatus != WEAPON_STATUS.BURNING)
        {
            Instantiate(fireEffect, transform.position + transform.up, transform.rotation, transform);
            weaponStatus = WEAPON_STATUS.BURNING;
        }
    }

    public void ElectrifyWeapon()
    {
        if (weaponStatus != WEAPON_STATUS.ELECTRIFIED)
        {
            Instantiate(electricEffect, transform.position + transform.up, transform.rotation, transform);
            weaponStatus = WEAPON_STATUS.ELECTRIFIED;
        }
    }

    public void WeaponPickedUp(Entity _owner)
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        anim.SetBool("isOwned", true);

        isOwned = true;
        owner = _owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackStance)
        {
            if (!weaponHit &&
                other.gameObject.GetInstanceID() != transform.parent.gameObject.GetInstanceID())
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    DamagePlayer(other.gameObject.GetComponent<PlayerBehaviour>());
                    weaponHit = true;
                }
                else if (other.gameObject.CompareTag("Enemy"))
                {
                    other.gameObject.GetComponent<EnemyBehaviour>().health -= damage;
                    weaponHit = true;
                }
                else if (other.gameObject.GetComponent<Boulder>() != null)
                {
                    if (weaponStatus == WEAPON_STATUS.BURNING)
                        other.gameObject.GetComponent<Boulder>().BurnBoulder();

                    weaponHit = true;
                }
            }
        }

    }
}
