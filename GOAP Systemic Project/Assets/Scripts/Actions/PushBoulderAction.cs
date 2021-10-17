using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PushBoulderAction : GoapAction
{
    private bool boulderPushed;
    private bool positionSet;

    private Vector3 pushPos;

    private LayerMask interactableLayer = 1 << 6;

    public PushBoulderAction()
    {
        addEffect("attackPlayerWithObjects", true);
    }

    public override void reset()
    {
        // Reset
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
        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i].gameObject.name.ToLower().Contains("boulder"))
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
        Transform player = FindObjectOfType<PlayerBehaviour>().gameObject.transform;

        if (positionSet == false)
        {
            Vector3 dir = (target.transform.position - player.position).normalized;

            pushPos = target.transform.position + (2.5f * dir);

            target.GetComponent<Boulder>().isTinyMoving = false;

            positionSet = true;
        }

        if (Vector3.Distance(agent.transform.position, pushPos) <= 0.8f)
        {
            agent.GetComponent<NavMeshAgent>().isStopped = true;

            Vector3 pushDir = (player.position - target.transform.position).normalized;
            pushDir = new Vector3(pushDir.x, 0.0f, pushDir.z);

            target.GetComponent<Rigidbody>().AddForce(pushDir * 15.0f, ForceMode.Impulse);
            target.GetComponent<Boulder>().isPushed = true;

            boulderPushed = true;
        }
        else
        {
            Debug.Log(Vector3.Distance(agent.transform.position, pushPos));
            GetComponent<NavMeshAgent>().SetDestination(pushPos);
            GetComponent<NavMeshAgent>().speed = agent.GetComponent<GeneralEnemy>().moveSpeed;
            GetComponent<NavMeshAgent>().stoppingDistance = 0;
        }

        return true;

    }
}
