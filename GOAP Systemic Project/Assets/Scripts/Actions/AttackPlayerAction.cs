using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackPlayerAction : GoapAction
{
    private bool playerDead = true;

    private LayerMask playerLayer = 1 << 7;

    private float attackDelay = 0.0f;

    private Vector3 prevPosition;

    public AttackPlayerAction()
    {
        addEffect("attackPlayer", true);

        movingActionText = "Moving towards Player";
        performingActionText = "Attacking Player with Bare Hands";
    }

    public override void reset()
    {
        // Reset
        playerDead = false;

    }

    public override void secondaryReset() { }

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
        target = null;

        Collider[] player = Physics.OverlapSphere(transform.position, 15.0f, playerLayer);

        if (player.Length > 0)
            target = player[0].gameObject;

        return target != null;
    }

    public override bool movementPass(GameObject agent)
    {
        return true;
    }

    public override bool perform(GameObject agent)
    {
        PlayerBehaviour player = FindObjectOfType<PlayerBehaviour>();

        if (agent.GetComponent<EnemyBehaviour>().isHealthy())
        {
            if (Vector3.Distance(transform.position, player.gameObject.transform.position) <= 1.5f)
            {
                if (attackDelay <= 0)
                {
                    player.GetComponent<PlayerBehaviour>().InflictDamage(2f);

                    if (player.GetComponent<PlayerBehaviour>().GetHealth() <= 0)
                    {
                        playerDead = true;
                    }

                    attackDelay = 1.0f;
                }
                else
                    attackDelay -= Time.deltaTime;
            }
            else
            {
                GetComponent<NavMeshAgent>().SetDestination(player.gameObject.transform.position);
                GetComponent<NavMeshAgent>().speed = agent.GetComponent<GeneralEnemy>().moveSpeed;
                GetComponent<NavMeshAgent>().stoppingDistance = minimumDistance;

                Vector3 faceDir = (transform.position - prevPosition).normalized;

                Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1800 * Time.deltaTime);

                attackDelay = 0.2f;

                prevPosition = transform.position;
            }

            return true;
        }
        else
            return false;
    }
}
