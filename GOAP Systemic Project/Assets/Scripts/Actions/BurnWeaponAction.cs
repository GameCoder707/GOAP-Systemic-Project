using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnWeaponAction : GoapAction
{
    private bool combatReady;

    public GameObject fireObject;

    private LayerMask interactableLayer = 1 << 6;

    public BurnWeaponAction()
    {
        addPrecondition("hasWeapon", true);

        addEffect("combatReady", true);
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

        Collider[] interactables = Physics.OverlapSphere(transform.position, 10.0f, interactableLayer);

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
        Instantiate(fireObject, agent.transform.position + (Vector3.up * 2), agent.transform.rotation, agent.transform);

        EnemyStats stats = agent.GetComponent<EnemyStats>();
        combatReady = true;
        stats.hasWeapon = false;
        stats.combatReady = false;
        return true;
    }
}
