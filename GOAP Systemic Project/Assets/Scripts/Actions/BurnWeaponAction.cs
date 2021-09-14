using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnWeaponAction : GoapAction
{
    private bool statusApplied;
    private bool weaponSwung;
    private bool skipAlternateAction;

    private LayerMask interactableLayer = 1 << 6;

    public BurnWeaponAction()
    {
        addPrecondition("hasWeapon", true);

        addEffect("isStatusAppliedToWeapon", true);
    }

    public override void reset()
    {
        // Reset
        statusApplied = false;
    }

    public override bool isDone()
    {
        return statusApplied;
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

    public override bool movementPass(GameObject agent)
    {
        if (GetComponentInChildren<Weapon>() != null)
        {
            if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.NONE)
                return true;
            else
                return false;
        }
        else
            return false;

    }

    public override bool perform(GameObject agent)
    {

        if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.NONE)
            skipAlternateAction = true; // We skipping the alternate action because no status effect has been applied prior to this

        if (skipAlternateAction)
        {
            if (!weaponSwung)
            {
                agent.GetComponentInChildren<Animator>().SetBool("isSwinging", true);
                weaponSwung = true;
            }

            if (GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.BURNING &&
                agent.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WeaponIdleAnim"))
            {
                statusApplied = true;
                weaponSwung = false;
                skipAlternateAction = false;
            }
        }
        else
        {
            statusApplied = true;
            weaponSwung = false;
        }

        return true;
    }
}
