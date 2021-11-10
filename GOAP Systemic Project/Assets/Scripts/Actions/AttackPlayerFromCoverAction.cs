using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerFromCoverAction : GoapAction
{
    private bool playerDead = true;

    private LayerMask interactableLayer = 1 << 6;

    private ObjectPool stonePool;

    private float attackDelay;

    public AttackPlayerFromCoverAction()
    {
        addEffect("attackPlayerFromCover", true);

        movingActionText = "Moving towards Cover";
        performingActionText = "Throwing Stones at Player from Cover";
    }

    private void Start()
    {
        stonePool = GameObject.Find("Stone Pool").GetComponent<ObjectPool>();
        attackDelay = 0.0f;
    }

    public override void reset()
    {
        // Reset
        playerDead = false;
    }

    public override void secondaryReset()
    {
        if (target != null)
        {
            if (target.GetComponentInParent<Barrier>().owner.GetInstanceID() == gameObject.GetInstanceID())
            {
                target.GetComponentInParent<Barrier>().occupied = false;
                target.GetComponentInParent<Barrier>().owner = null;
                target = null;
            }
        }
    }

    public override bool isDone()
    {
        return playerDead;
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
                if (interactables[i].gameObject.name.ToLower().Contains("cover"))
                {
                    if(target != null)
                    {
                        if(target.transform.parent.gameObject == interactables[i].gameObject)
                        {
                            target = interactables[i].gameObject.GetComponent<Barrier>().GetCoverPos();
                            break;
                        }
                    }
                    else if (!interactables[i].gameObject.GetComponent<Barrier>().occupied)
                    {
                        target = interactables[i].gameObject.GetComponent<Barrier>().GetCoverPos();

                        interactables[i].gameObject.GetComponent<Barrier>().occupied = true;
                        interactables[i].gameObject.GetComponent<Barrier>().owner = agent;
                        break;
                    }
                }
            }
        }

        return target != null;
    }

    public override bool movementPass(GameObject agent)
    {
        if (target.GetComponentInParent<Barrier>().owner.GetInstanceID() == agent.GetInstanceID())
            return true;
        else
            return false;
    }

    public override bool perform(GameObject agent)
    {
        Transform player = FindObjectOfType<PlayerBehaviour>().gameObject.transform;

        if (Vector3.Distance(player.position, transform.position) >= 6f && target.GetComponentInParent<Barrier>().GetCoverStatus())
        {
            target.GetComponentInParent<Barrier>().occupied = true;

            Vector3 faceDir = (player.position - agent.transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, lookRotation, 1200 * Time.deltaTime);

            if (attackDelay <= 0)
            {
                GameObject obj = stonePool.GetPooledObject();
                obj.transform.position = agent.transform.position + (Vector3.up * 1.2f);
                obj.transform.rotation = agent.transform.rotation;
                obj.SetActive(true);

                Vector3 aimDir = (player.position - obj.transform.position).normalized;

                obj.GetComponent<Rigidbody>().AddForce(aimDir * 10.0f, ForceMode.Impulse);

                attackDelay = 1.25f;
            }
            else
                attackDelay -= Time.deltaTime;

            return true;
        }
        else
        {
            target.GetComponentInParent<Barrier>().occupied = false;
            return false;
        }
    }
}