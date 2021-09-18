using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attacking player with a status applied weapon: Burn, Shock, etc
public class AttackPlayerWithStatWeaponAction : GoapAction
{
    private bool playerDead = true;

    private LayerMask playerLayer = 1 << 7;

    private float attackDelay = 0.0f;

    public AttackPlayerWithStatWeaponAction()
    {
        addPrecondition("hasWeapon", true);
        addPrecondition("isStatusAppliedToWeapon", true);

        addEffect("attackPlayerWithStatWeapon", true);
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
        Collider[] player = Physics.OverlapSphere(transform.position, 15.0f, playerLayer);

        if (GetComponentInChildren<Weapon>() != null && agent.GetComponent<EnemyBehaviour>().isHealthy())
        {
            if(player.Length > 0)
            {
                if (Vector3.Distance(transform.position, player[0].gameObject.transform.position) <= 1.5f)
                {
                    if (attackDelay <= 0)
                    {
                        //GetComponentInChildren<Weapon>().DamagePlayer(player[0].GetComponent<PlayerBehaviour>());

                        GetComponentInChildren<Weapon>().GetComponent<Animator>().SetBool("isSwinging", true);

                        if (player[0].GetComponent<PlayerBehaviour>().GetHealth() <= 0)
                        {
                            playerDead = true;
                            //stats.hasWeapon = false;
                            //stats.combatReady = false;
                            //player[0].GetComponent<PlayerBehaviour>().health = 100;
                        }

                        attackDelay = 1.0f;
                    }
                    else
                        attackDelay -= Time.deltaTime;

                }
                else
                {
                    Vector3 prevPosition = transform.position;

                    transform.position = Vector3.MoveTowards(transform.position, player[0].gameObject.transform.position,
                        agent.GetComponent<GeneralEnemy>().moveSpeed * Time.deltaTime);

                    Vector3 faceDir = (transform.position - prevPosition).normalized;

                    Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1800 * Time.deltaTime);

                    attackDelay = 0.2f;
                }
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
