using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAction : GoapAction
{
    private bool canAttack;

    public FleeAction()
    {
        addEffect("canAttack", true);
    }

    public override void reset()
    {
        // Reset
        canAttack = false;
    }

    public override void secondaryReset() { }

    public override bool isDone()
    {
        return canAttack;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    { 
        return true;
    }

    public override bool movementPass(GameObject agent)
    {
        return true;
    }

    public override bool perform(GameObject agent)
    {
        if (!agent.GetComponent<EnemyBehaviour>().isHealthy())
        {
            PlayerBehaviour player = FindObjectOfType<PlayerBehaviour>();
            Vector3 fleeDirection = (agent.transform.position - player.gameObject.transform.position).normalized;
            fleeDirection = new Vector3(fleeDirection.x, 0.0f, fleeDirection.z);

            if (Vector3.Distance(agent.transform.position, player.gameObject.transform.position) <= 5.0f)
            {
                transform.position += (fleeDirection * agent.GetComponent<GeneralEnemy>().moveSpeed * Time.deltaTime);
            }
        }
        else
            canAttack = true;

        return true;

    }

}
