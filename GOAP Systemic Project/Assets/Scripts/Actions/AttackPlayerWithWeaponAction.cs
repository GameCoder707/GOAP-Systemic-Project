using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerWithWeaponAction : GoapAction
{
    private bool playerDead = true;

    private LayerMask playerLayer = 1 << 7;

    private float attackDelay = 0.0f;

    public AttackPlayerWithWeaponAction()
    {
        addPrecondition("hasWeapon", true);

        addEffect("attackPlayerWithWeapon", true);
    }

    public override void reset()
    {
        // Reset
        playerDead = false;

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
        EnemyStats stats = agent.GetComponent<EnemyStats>();
        PlayerBehaviour player = FindObjectOfType<PlayerBehaviour>();

        if (GetComponentInChildren<Weapon>() != null && agent.GetComponent<EnemyBehaviour>().isHealthy())
        {
            if (Vector3.Distance(transform.position, player.gameObject.transform.position) <= 1.5f)
            {
                if (attackDelay <= 0)
                {
                    if (player.GetHealth() > 0)
                    {
                        GetComponentInChildren<Weapon>().GetComponent<Animator>().SetBool("isSwinging", true);

                        if (player.GetHealth() <= 0)
                            playerDead = true;
                    }

                    attackDelay = 1.0f;
                }
                else
                    attackDelay -= Time.deltaTime;
            }
            else
            {
                Vector3 prevPosition = transform.position;

                transform.position = Vector3.MoveTowards(transform.position, player.gameObject.transform.position,
                    agent.GetComponent<GeneralEnemy>().moveSpeed * Time.deltaTime);

                Vector3 faceDir = (transform.position - prevPosition).normalized;

                Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1800 * Time.deltaTime);

                attackDelay = 0.2f;
            }

            return true;
        }
        else
        {
            attackDelay = 0.0f;
            return false;
        }

    }

}
