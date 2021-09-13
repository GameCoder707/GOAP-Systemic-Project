using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeaponAction : GoapAction
{
    private LayerMask interactableLayer = 1 << 6;

    private bool hasWeapon;

    public PickUpWeaponAction()
    {
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
        target = null;

        Collider[] interactables = Physics.OverlapSphere(transform.position, 15.0f, interactableLayer);

        if (interactables.Length > 0)
        {
            bool flag = false;

            for (int i = 0; i < interactables.Length; i++)
            {

                if (interactables[i].gameObject.ToString().ToLower().Contains("weapon"))
                {
                    switch (agent.GetComponent<GeneralEnemy>().type)
                    {
                        case GeneralEnemy.ENEMY_TYPE.HEAVY: // Heavy enemies checks for non-heavy enemies
                            if (agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(interactables, "weapon", false, GeneralEnemy.ENEMY_TYPE.HEAVY))
                            {
                                if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                                    interactables[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() == null)
                                {
                                    if (agent.GetComponent<GeneralEnemy>().goalName == "attackPlayerWithStatWeapon")
                                    {
                                        if (interactables[i].gameObject.GetComponent<Weapon>().flammable)
                                        {
                                            interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                            target = interactables[i].gameObject;
                                            flag = true;
                                        }
                                    }
                                    else
                                    {
                                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                        target = interactables[i].gameObject;
                                        flag = true;
                                    }
                                }
                            }

                            break;

                        case GeneralEnemy.ENEMY_TYPE.MEDIUM: // Medium enemy checks for light enemies
                            if (agent.GetComponent<GeneralEnemy>().CanEnemyInteractWithObject(interactables, "weapon", true, GeneralEnemy.ENEMY_TYPE.LIGHT))
                            {
                                if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                                    interactables[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() == null)
                                {
                                    if (agent.GetComponent<GeneralEnemy>().goalName == "attackPlayerWithStatWeapon")
                                    {
                                        if (interactables[i].gameObject.GetComponent<Weapon>().flammable)
                                        {
                                            interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                            target = interactables[i].gameObject;
                                            flag = true;
                                        }
                                    }
                                    else
                                    {
                                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                        target = interactables[i].gameObject;
                                        flag = true;
                                    }

                                }
                            }

                            break;

                        case GeneralEnemy.ENEMY_TYPE.LIGHT: // Light enemies don't check for anything

                            if (interactables[i].gameObject.GetComponent<Weapon>().isOwned == false &&
                                interactables[i].gameObject.transform.parent.gameObject.GetComponent<GeneralEnemy>() == null)
                            {
                                if (agent.GetComponent<GeneralEnemy>().goalName == "attackPlayerWithStatWeapon")
                                {
                                    if (interactables[i].gameObject.GetComponent<Weapon>().flammable)
                                    {
                                        Debug.Log(interactables[i].gameObject.name);
                                        interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                        target = interactables[i].gameObject;
                                        flag = true;
                                    }
                                }
                                else
                                {
                                    Debug.Log(agent.GetComponent<GeneralEnemy>().goalName);
                                    interactables[i].gameObject.GetComponent<Weapon>().isOwned = true;
                                    target = interactables[i].gameObject;
                                    flag = true;
                                }
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

        hasWeapon = true;
        stats.hasWeapon = true;
        target.GetComponent<Weapon>().WeaponPickedUp();


        target.transform.parent = agent.transform;
        target.transform.position = agent.transform.position + (agent.transform.right * 0.5f);
        target.transform.rotation = Quaternion.identity;



        return true;
    }
}
