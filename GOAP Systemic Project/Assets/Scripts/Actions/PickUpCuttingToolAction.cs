using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCuttingToolAction : GoapAction
{
    private LayerMask interactableLayer = 1 << 6;
    private LayerMask enemyLayer = 1 << 8;

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
                    switch (agent.GetComponent<GeneralEnemy>().type)
                    {
                        case GeneralEnemy.ENEMY_TYPE.HEAVY:

                            Collider[] enemiesInArea = Physics.OverlapSphere(transform.position, 15.0f, enemyLayer);

                            List<GameObject> updatedEnemiesInArea = new List<GameObject>();

                            for (int j = 0; j < enemiesInArea.Length; j++)
                            {
                                if (enemiesInArea[j].gameObject.GetInstanceID() != agent.gameObject.GetInstanceID())
                                    updatedEnemiesInArea.Add(enemiesInArea[j].gameObject);
                            }

                            if (updatedEnemiesInArea.Count > 0)
                            {
                                // We making sure no medium type enemies need the axe
                                for (int k = 0; k < updatedEnemiesInArea.Count; k++)
                                    if (updatedEnemiesInArea[k].gameObject.GetComponent<GeneralEnemy>().type == GeneralEnemy.ENEMY_TYPE.MEDIUM)
                                        if (updatedEnemiesInArea[k].gameObject.GetComponent<GeneralEnemy>().
                                            CanEnemyInteractWithObject(interactables, "weapon", true, GeneralEnemy.ENEMY_TYPE.LIGHT))
                                        {
                                            interactables[i].gameObject.GetComponent<CuttingTool>().isOwned = true;
                                            target = interactables[i].gameObject;
                                            flag = true;
                                        }
                            }
                            else if (interactables[i].gameObject.GetComponent<CuttingTool>().isOwned == false)
                            {
                                if (!agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(
                                    interactables, "weapon", false, GeneralEnemy.ENEMY_TYPE.HEAVY))
                                    interactables[i].gameObject.GetComponent<CuttingTool>().isOwned = true;

                                target = interactables[i].gameObject;
                                flag = true;
                            }


                            break;

                        case GeneralEnemy.ENEMY_TYPE.MEDIUM:

                            if (interactables[i].gameObject.GetComponent<CuttingTool>().isOwned == false)
                            {
                                if (!agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(
                                    interactables, "weapon", true, GeneralEnemy.ENEMY_TYPE.LIGHT))
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