using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnWeaponAction : GoapAction
{
    private bool combatReady;
    private bool weaponSwung;

    private LayerMask interactableLayer = 1 << 6;

    public BurnWeaponAction()
    {
        addPrecondition("hasWeapon", true);

        addEffect("isStatusAppliedToWeapon", true);
    }

    public override void reset()
    {
        // Reset
        combatReady = false;

    }

    public override bool isDone()
    {
        return combatReady;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        target = null;

        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("campfire"))
                {
                    target = interactables[i].gameObject;
                    break;
                }
            }
        }


        return target != null;

    }

    public override bool perform(GameObject agent)
    {
        if (!weaponSwung)
        {
            agent.GetComponentInChildren<Animator>().SetBool("isSwinging", true);
            weaponSwung = true;
        }

        if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.BURNING &&
            agent.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WeaponIdleAnim"))
        {
            EnemyStats stats = agent.GetComponent<EnemyStats>();
            combatReady = true;
            stats.combatReady = true;
            weaponSwung = false;
        }

        return true;
    }
}
