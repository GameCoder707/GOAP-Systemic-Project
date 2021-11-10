using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PushBoulderAction : GoapAction
{
    private bool boulderPushed;
    private bool positionSet;
    private bool waitForStatusEffect;

    private Vector3 pushPos;

    private LayerMask interactableLayer = 1 << 6;
    private LayerMask enemyLayer = 1 << 8;

    private float pushDelay;

    public PushBoulderAction()
    {
        addEffect("attackPlayerWithObjects", true);

        pushDelay = 0.3f;

        movingActionText = "Moving towards Boulder";
        performingActionText = "Aligning myself to Push boulder at Player";
    }

    public override void reset()
    {
        // Reset
        pushDelay = 0.3f;
        boulderPushed = false;
    }

    public override void secondaryReset() { }

    public override bool isDone()
    {
        return boulderPushed;
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
                if (interactables[i].gameObject.name.ToLower().Contains("boulder"))
                {
                    if (!interactables[i].gameObject.GetComponent<Boulder>().isPushed &&
                        interactables[i].gameObject.GetComponent<Boulder>().pusher == null)
                    {
                        target = interactables[i].gameObject;
                        target.GetComponent<Boulder>().pusher = agent;
                        break;
                    }

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
        Transform player = FindObjectOfType<PlayerBehaviour>().gameObject.transform;

        if (positionSet == false)
        {
            Vector3 dir = (target.transform.position - player.position).normalized;

            pushPos = target.transform.position + (2.75f * dir);

            target.GetComponent<Boulder>().isTinyMoving = false;

            positionSet = true;
        }

        if (Vector3.Distance(agent.transform.position, pushPos) <= 0.8f)
        {
            agent.GetComponent<NavMeshAgent>().isStopped = true;

            if (!waitForStatusEffect)
            {
                Collider[] surroundingEnemies = Physics.OverlapSphere(transform.position, 17.0f, enemyLayer);

                if (surroundingEnemies.Length > 0)
                {
                    for (int i = 0; i < surroundingEnemies.Length; i++)
                    {
                        if (surroundingEnemies[i].gameObject.GetComponent<GeneralEnemy>().type != GeneralEnemy.ENEMY_TYPE.HEAVY)
                        {
                            //if (surroundingEnemies[i].gameObject.GetComponent<GeneralEnemy>().CheckForFireSource())
                            if (CheckActionInPlan(surroundingEnemies[i].GetComponent<GoapAgent>().GetActionPlan(), "BurnBoulderAction"))
                            {
                                waitForStatusEffect = true;
                                break;
                            }
                        }
                    }

                    if (!waitForStatusEffect)
                        PushBoulder(player);
                }
                else // No enemies to burn boulder
                    PushBoulder(player);
            }
            else
            {
                RaycastHit hit;

                if (Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject.name.ToLower().Contains("weather"))
                        if (hit.collider.gameObject.GetComponent<Weather>().weatherType == Weather.WEATHER_TYPE.RAINY)
                            PushBoulder(player);
                }

                if (target.GetComponent<Boulder>().isStatusApplied)
                {
                    //Debug.Log("Pushing coz some enemy burned it");
                    PushBoulder(player);
                }
            }
        }
        else
        {
            GetComponent<NavMeshAgent>().SetDestination(pushPos);
            GetComponent<NavMeshAgent>().speed = agent.GetComponent<GeneralEnemy>().moveSpeed;
            GetComponent<NavMeshAgent>().stoppingDistance = 0;
        }

        return true;

    }

    private void PushBoulder(Transform player)
    {
        if (pushDelay <= 0)
        {
            Vector3 pushDir = (player.position - target.transform.position).normalized;
            pushDir = new Vector3(pushDir.x, 0.0f, pushDir.z);

            target.GetComponent<Rigidbody>().AddForce(pushDir * 15.0f, ForceMode.Impulse);
            target.GetComponent<Boulder>().isPushed = true;

            pushDelay = 0.3f;
            boulderPushed = true;
        }
        else
            pushDelay -= Time.deltaTime;

    }

    private bool CheckActionInPlan(Queue<GoapAction> actionPlan, string actionName)
    {
        foreach (GoapAction a in actionPlan)
        {
            if (a.GetType().Name == actionName)
                return true;
        }

        return false;
    }
}
