using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeaponAction : GoapAction
{
    private LayerMask interactableLayer = 1 << 6;

    private bool hasWeapon;

    public PickUpWeaponAction()
    {
        addEffect("hasWeapon", true);
    }

    public override void reset()
    {
        // Reset
        hasWeapon = false;

    }

    public override bool isDone()
    {
        return hasWeapon;
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
                if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                {
                    if(interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                        interactables[i].gameObject.transform.parent == null)
                    {
                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                        target = interactables[i].gameObject;
                        break;
                    }

                }
            }
        }

        return target != null;
    }

    public override bool perform(GameObject agent)
    {
        EnemyStats stats = GetComponent<EnemyStats>();

        hasWeapon = true;
        stats.hasWeapon = true;

        target.transform.parent = agent.transform;
        target.transform.position = agent.transform.position + Vector3.up;
        target.transform.rotation = Quaternion.identity;

        return true;
    }
}
