using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnBoulderAction : GoapAction
{
    private bool boulderBurned;

    private LayerMask interactableLayer = 1 << 6;

    public BurnBoulderAction()
    {
        addPrecondition("hasWeapon", true);
        addPrecondition("isStatusAppliedToWeapon", true);

        addEffect("attackPlayerWithObjects", true);
    }

    public override void reset()
    {
        // Reset
        boulderBurned = false;
    }

    public override void secondaryReset() { }

    public override bool isDone()
    {
        return boulderBurned;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        if(agent.GetComponentInChildren<Weapon>() != null)
        {
            if (agent.GetComponentInChildren<Weapon>().weaponStatus != Weapon.WEAPON_STATUS.BURNING)
                return false;
        }

        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.name.ToLower().Contains("boulder"))
                {
                    if (!interactables[i].gameObject.GetComponent<Boulder>().isPushed)
                    {
                        target = interactables[i].gameObject;
                        break;
                    }
                }
            }
        }

        return target != null;
    }

    public override bool movementPass(GameObject agent)
    {
        if (agent.GetComponentInChildren<Weapon>() != null)
        {
            if (agent.GetComponentInChildren<Weapon>().weaponStatus == Weapon.WEAPON_STATUS.BURNING)
                return true;
            else
                return false;
        }

        return false;
    }

    public override bool perform(GameObject agent)
    {
        if (agent.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("WeaponIdleAnim"))
        {
            agent.GetComponentInChildren<Weapon>().anim.SetBool("isSwinging", true);
        }

        if (target.GetComponent<Boulder>().isStatusApplied)
            boulderBurned = true;

        return true;

    }
}
