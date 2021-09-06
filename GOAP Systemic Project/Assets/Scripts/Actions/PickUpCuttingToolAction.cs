using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCuttingToolAction : GoapAction
{
    private LayerMask interactableLayer = 1 << 6;

    private bool hasCuttingTool;

    public PickUpCuttingToolAction()
    {
        addEffect("hasCuttingTool", true);
    }

    public override void reset()
    {
        // Reset
        hasCuttingTool = false;

    }

    public override bool isDone()
    {
        return hasCuttingTool;
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
            bool flag = false;

            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.ToString().ToLower().Contains("cutting tool"))
                {
                    switch(agent.GetComponent<GeneralEnemy>().type)
                    {
                        case GeneralEnemy.ENEMY_TYPE.HEAVY:

                            if(agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(
                                interactables, "cutting tool", true, GeneralEnemy.ENEMY_TYPE.MEDIUM))
                            {
                                if (interactables[i].gameObject.GetComponent<CuttingTool>().isOwned == false)
                                {
                                    interactables[i].gameObject.GetComponent<CuttingTool>().isOwned = true;
                                    target = interactables[i].gameObject;
                                    flag = true;
                                }
                            }

                            break;

                        case GeneralEnemy.ENEMY_TYPE.MEDIUM:

                            if (interactables[i].gameObject.GetComponent<CuttingTool>().isOwned == false)
                            {
                                interactables[i].gameObject.GetComponent<CuttingTool>().isOwned = true;
                                target = interactables[i].gameObject;
                                flag = true;
                            }

                            break;
                    }

                    if (flag)
                        break;

                }
            }
        }

        return target != null;
    }

    public override bool perform(GameObject agent)
    {
        EnemyStats stats = GetComponent<EnemyStats>();

        hasCuttingTool = true;
        stats.hasCuttingTool = true;

        Destroy(target);

        return true;
    }
}