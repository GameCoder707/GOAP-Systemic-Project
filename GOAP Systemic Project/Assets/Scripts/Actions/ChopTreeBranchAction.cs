using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTreeBranchAction : GoapAction
{
    private bool hasWeapon = false;

    public GameObject woodenStick;

    private LayerMask interactableLayer = 1 << 6;

    private float workDuration = 2.5f;
    public float elapsedTime = 0.0f;

    public ChopTreeBranchAction()
    {
        addPrecondition("hasCuttingTool", true);

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
        if (agent.GetComponent<EnemyStats>().hasWeapon)
            return false;

        target = null;

        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("tree"))
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
        return true;
    }

    public override bool perform(GameObject agent)
    {
        if (elapsedTime >= workDuration)
        {
            GameObject obj = Instantiate(woodenStick, agent.transform.position + (agent.transform.right * 0.5f), agent.transform.rotation, agent.transform);
            obj.GetComponent<Weapon>().isOwned = true;
            obj.GetComponent<Weapon>().WeaponPickedUp(GetComponent<Entity>());

            EnemyStats stats = agent.GetComponent<EnemyStats>();
            hasWeapon = true;
            stats.hasWeapon = true;
            elapsedTime = 0.0f;
        }
        else
            elapsedTime += Time.deltaTime;

        return true;
    }

}
